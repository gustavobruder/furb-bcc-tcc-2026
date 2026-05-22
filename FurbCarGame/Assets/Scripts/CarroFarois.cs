using UnityEngine;

public class CarroFarois : MonoBehaviour
{
    [Header("Estado")]
    public bool FaroisLigados;

    [Header("Luzes Dianteiras")]
    public Light luzDianteiraDireita;
    public Light luzDianteiraEsquerda;

    [Header("Configuracao das Luzes")]
    [Min(0f)] public float intensidadeLuz = 500f;
    [Min(0.01f)] public float alcanceLuz = 20f;
    public Color corFarol = new(1f, 0.9782649f, 0.7783019f);

    [Header("Emissao Opcional")]
    public Renderer[] renderizadoresEmissao;
    public Color corEmissaoFarolInativo = Color.black;
    public Color corEmissaoFarolAtivo = new(1f, 0.9782649f, 0.7783019f);

    private static readonly int IdCorEmissao = Shader.PropertyToID("_EmissionColor");
    private static readonly int IdCorEmissiva = Shader.PropertyToID("_EmissiveColor");

    private void Awake()
    {
        AplicarEstadoAtual();
    }

    private void Reset()
    {
        FaroisLigados = false;
        AplicarEstadoAtual();
    }

    public void AlternarFarois()
    {
        FaroisLigados = !FaroisLigados;
        AplicarEstadoAtual();
    }

    private void AplicarEstadoAtual()
    {
        ConfigurarFarol(luzDianteiraDireita, renderizadoresEmissao);
        ConfigurarFarol(luzDianteiraEsquerda, renderizadoresEmissao);
    }

    private void ConfigurarFarol(Light luzDianteira, Renderer[] renderizadores)
    {
        ConfigurarLuz(luzDianteira);
        ConfigurarEmissao(renderizadores);
    }

    private void ConfigurarLuz(Light luzAlvo)
    {
        if (luzAlvo == null)
            return;

        luzAlvo.color = corFarol;
        luzAlvo.intensity = FaroisLigados ? intensidadeLuz : 0f;
        luzAlvo.range = alcanceLuz;
        luzAlvo.enabled = FaroisLigados;
    }

    private void ConfigurarEmissao(Renderer[] renderizadores)
    {
        if (renderizadores == null)
            return;

        foreach (Renderer renderizadorAlvo in renderizadores)
        {
            if (renderizadorAlvo == null)
                continue;

            var materiais = renderizadorAlvo.sharedMaterials;
            if (materiais == null || materiais.Length == 0)
                continue;

            var blocoPropriedades = new MaterialPropertyBlock();
            renderizadorAlvo.GetPropertyBlock(blocoPropriedades);

            bool alterou = false;
            foreach (Material material in materiais)
            {
                if (material == null)
                    continue;

                if (material.HasProperty(IdCorEmissao))
                {
                    blocoPropriedades.SetColor(IdCorEmissao, FaroisLigados ? corEmissaoFarolAtivo : corEmissaoFarolInativo);
                    alterou = true;
                }

                if (material.HasProperty(IdCorEmissiva))
                {
                    blocoPropriedades.SetColor(IdCorEmissiva, FaroisLigados ? corEmissaoFarolAtivo : corEmissaoFarolInativo);
                    alterou = true;
                }
            }

            if (alterou)
                renderizadorAlvo.SetPropertyBlock(blocoPropriedades);
        }
    }
}
