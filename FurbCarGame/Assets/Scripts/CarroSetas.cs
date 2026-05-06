using UnityEngine;

public class CarroSetas : MonoBehaviour
{
    [Header("Estado")]
    public bool SetaDireitaLigada;
    public bool SetaEsquerdaLigada;

    [Header("Luzes Direita")]
    public Light luzDianteiraDireita;
    public Light luzTraseiraDireita;

    [Header("Luzes Esquerda")]
    public Light luzDianteiraEsquerda;
    public Light luzTraseiraEsquerda;

    [Header("Configuracao das Luzes")]
    [Min(0f)] public float intensidadeLuz = 2f;
    [Min(0.01f)] public float alcanceLuz = 1f;
    [Min(0.01f)] public float intervaloPisca = 0.5f;
    public Color corSeta = Color.yellow;

    [Header("Emissao Opcional")]
    public Renderer[] renderizadoresEmissaoDireita;
    public Renderer[] renderizadoresEmissaoEsquerda;
    public Color corEmissaoInativa = Color.black;
    public Color corEmissaoSeta = new(2f, 1.4f, 0f);

    private static readonly int IdCorEmissao = Shader.PropertyToID("_EmissionColor");
    private static readonly int IdCorEmissiva = Shader.PropertyToID("_EmissiveColor");

    private float _tempoPisca;
    private bool _luzesSetaLigadas = true;

    private void Awake()
    {
        AplicarEstadoAtual();
    }

    private void Update()
    {
        if (!SetaDireitaLigada && !SetaEsquerdaLigada)
            return;

        _tempoPisca += Time.deltaTime;
        if (_tempoPisca < intervaloPisca)
            return;

        _tempoPisca = 0f;
        _luzesSetaLigadas = !_luzesSetaLigadas;
        AplicarEstadoAtual();
    }

    private void Reset()
    {
        SetaDireitaLigada = false;
        SetaEsquerdaLigada = false;
        AplicarEstadoAtual();
    }

    public void AlternarSetaDireita()
    {
        if (SetaDireitaLigada)
            DesligarSetaDireita();
        else
            LigarSetaDireita();
    }

    public void AlternarSetaEsquerda()
    {
        if (SetaEsquerdaLigada)
            DesligarSetaEsquerda();
        else
            LigarSetaEsquerda();
    }

    private void LigarSetaDireita()
    {
        DesligarSetaEsquerda();
        SetaDireitaLigada = true;
        IniciarPiscaLigado();
        AplicarEstadoAtual();
    }

    private void DesligarSetaDireita()
    {
        SetaDireitaLigada = false;
        AplicarEstadoAtual();
    }

    private void LigarSetaEsquerda()
    {
        DesligarSetaDireita();
        SetaEsquerdaLigada = true;
        IniciarPiscaLigado();
        AplicarEstadoAtual();
    }

    private void DesligarSetaEsquerda()
    {
        SetaEsquerdaLigada = false;
        AplicarEstadoAtual();
    }

    private void AplicarEstadoAtual()
    {
        ConfigurarSeta(luzDianteiraDireita, luzTraseiraDireita, renderizadoresEmissaoDireita, SetaDireitaLigada && _luzesSetaLigadas);
        ConfigurarSeta(luzDianteiraEsquerda, luzTraseiraEsquerda, renderizadoresEmissaoEsquerda, SetaEsquerdaLigada && _luzesSetaLigadas);
    }

    private void IniciarPiscaLigado()
    {
        _tempoPisca = 0f;
        _luzesSetaLigadas = true;
    }

    private void ConfigurarSeta(Light luzDianteira, Light luzTraseira, Renderer[] renderizadores, bool estadoLigado)
    {
        ConfigurarLuz(luzDianteira, estadoLigado);
        ConfigurarLuz(luzTraseira, estadoLigado);
        ConfigurarEmissao(renderizadores, estadoLigado ? corEmissaoSeta : corEmissaoInativa);
    }

    private void ConfigurarLuz(Light luzAlvo, bool estadoLigado)
    {
        if (luzAlvo == null)
            return;

        luzAlvo.color = corSeta;
        luzAlvo.intensity = estadoLigado ? intensidadeLuz : 0f;
        luzAlvo.range = alcanceLuz;
        luzAlvo.enabled = estadoLigado;
    }

    private void ConfigurarEmissao(Renderer[] renderizadores, Color cor)
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
                    blocoPropriedades.SetColor(IdCorEmissao, cor);
                    alterou = true;
                }

                if (material.HasProperty(IdCorEmissiva))
                {
                    blocoPropriedades.SetColor(IdCorEmissiva, cor);
                    alterou = true;
                }
            }

            if (alterou)
                renderizadorAlvo.SetPropertyBlock(blocoPropriedades);
        }
    }
}
