using System.Collections;
using UnityEngine;

public class Semaforo : MonoBehaviour
{
    public enum EstadoSemaforo
    {
        Verde,
        Amarelo,
        Vermelho
    }

    [Header("Duracao")]
    [Min(0.1f)] public float duracaoVerde = 8f;
    [Min(0.1f)] public float duracaoAmarelo = 2f;
    [Min(0.1f)] public float duracaoVermelho = 8f;
    public EstadoSemaforo estadoInicial = EstadoSemaforo.Vermelho;
    public EstadoSemaforo estadoAtual;

    [Header("Anchors Opcionais")]
    public Transform ancoraVermelha;
    public Transform ancoraAmarela;
    public Transform ancoraVerde;

    [Header("Luzes")]
    public bool criarLuzesAutomaticamente = true;
    [Min(0f)] public float intensidadeLuz = 8f;
    [Min(0.01f)] public float alcanceLuz = 4f;
    [Min(0f)] public float deslocamentoFrontalLente = 0.15f;
    public Light luzVermelha;
    public Light luzAmarela;
    public Light luzVerde;

    [Header("Emissao Opcional")]
    public Renderer[] renderizadoresEmissaoVermelha;
    public Renderer[] renderizadoresEmissaoAmarela;
    public Renderer[] renderizadoresEmissaoVerde;
    public Color corEmissaoInativa = Color.black;
    public Color corEmissaoVermelha = new(2f, 0f, 0f);
    public Color corEmissaoAmarela = new(2f, 1.4f, 0f);
    public Color corEmissaoVerde = new(0f, 2f, 0f);

    private static readonly int IdCorEmissao = Shader.PropertyToID("_EmissionColor");
    private static readonly int IdCorEmissiva = Shader.PropertyToID("_EmissiveColor");

    private Coroutine rotinaCiclo;
    private Renderer renderizadorEmCache;

    private void Awake()
    {
        renderizadorEmCache = GetComponentInChildren<Renderer>();
        GarantirLuzes();
        AplicarEstado(estadoInicial);
    }

    private void OnEnable()
    {
        rotinaCiclo = StartCoroutine(CiclarLuzes());
    }

    private void OnDisable()
    {
        if (rotinaCiclo != null)
        {
            StopCoroutine(rotinaCiclo);
            rotinaCiclo = null;
        }
    }

    private void Reset()
    {
        renderizadorEmCache = GetComponentInChildren<Renderer>();
        GarantirLuzes();
        AplicarEstado(estadoInicial);
    }

    private IEnumerator CiclarLuzes()
    {
        estadoAtual = estadoInicial;

        while (true)
        {
            AplicarEstado(estadoAtual);
            yield return new WaitForSeconds(ObterDuracao(estadoAtual));
            estadoAtual = ObterProximoEstado(estadoAtual);
        }
    }

    private float ObterDuracao(EstadoSemaforo estado)
    {
        return estado switch
        {
            EstadoSemaforo.Verde => duracaoVerde,
            EstadoSemaforo.Amarelo => duracaoAmarelo,
            _ => duracaoVermelho
        };
    }

    private static EstadoSemaforo ObterProximoEstado(EstadoSemaforo estado)
    {
        return estado switch
        {
            EstadoSemaforo.Verde => EstadoSemaforo.Amarelo,
            EstadoSemaforo.Amarelo => EstadoSemaforo.Vermelho,
            _ => EstadoSemaforo.Verde
        };
    }

    private void AplicarEstado(EstadoSemaforo estado)
    {
        ConfigurarLuz(luzVermelha, estado == EstadoSemaforo.Vermelho, Color.red);
        ConfigurarLuz(luzAmarela, estado == EstadoSemaforo.Amarelo, Color.yellow);
        ConfigurarLuz(luzVerde, estado == EstadoSemaforo.Verde, Color.green);

        ConfigurarEmissao(renderizadoresEmissaoVermelha, estado == EstadoSemaforo.Vermelho ? corEmissaoVermelha : corEmissaoInativa);
        ConfigurarEmissao(renderizadoresEmissaoAmarela, estado == EstadoSemaforo.Amarelo ? corEmissaoAmarela : corEmissaoInativa);
        ConfigurarEmissao(renderizadoresEmissaoVerde, estado == EstadoSemaforo.Verde ? corEmissaoVerde : corEmissaoInativa);
    }

    private void ConfigurarLuz(Light luzAlvo, bool estadoLigado, Color cor)
    {
        if (luzAlvo == null)
            return;

        luzAlvo.color = cor;
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

    private void GarantirLuzes()
    {
        if (!criarLuzesAutomaticamente)
            return;

        ancoraVermelha = ancoraVermelha != null ? ancoraVermelha : CriarAncora("Ancora Vermelha", 0.75f);
        ancoraAmarela = ancoraAmarela != null ? ancoraAmarela : CriarAncora("Ancora Amarela", 0.5f);
        ancoraVerde = ancoraVerde != null ? ancoraVerde : CriarAncora("Ancora Verde", 0.25f);

        luzVermelha = luzVermelha != null ? luzVermelha : CriarLuz("Luz Vermelha", ancoraVermelha, Color.red);
        luzAmarela = luzAmarela != null ? luzAmarela : CriarLuz("Luz Amarela", ancoraAmarela, Color.yellow);
        luzVerde = luzVerde != null ? luzVerde : CriarLuz("Luz Verde", ancoraVerde, Color.green);
    }

    private Transform CriarAncora(string nomeAncora, float fatorAltura)
    {
        var ancoraExistente = transform.Find(nomeAncora);
        if (ancoraExistente != null)
            return ancoraExistente;

        var objetoAncora = new GameObject(nomeAncora);
        Transform transformAncora = objetoAncora.transform;
        transformAncora.SetParent(transform, false);
        transformAncora.localPosition = EstimarPosicaoLocal(fatorAltura);
        transformAncora.localRotation = Quaternion.identity;
        return transformAncora;
    }

    private Light CriarLuz(string nomeLuz, Transform ancoraPai, Color cor)
    {
        if (ancoraPai == null)
            return null;

        Transform transformExistente = ancoraPai.Find(nomeLuz);
        Light luzAlvo = transformExistente != null ? transformExistente.GetComponent<Light>() : null;
        if (luzAlvo == null)
        {
            var objetoLuz = new GameObject(nomeLuz);
            objetoLuz.transform.SetParent(ancoraPai, false);
            luzAlvo = objetoLuz.AddComponent<Light>();
            luzAlvo.type = LightType.Point;
        }

        luzAlvo.color = cor;
        luzAlvo.range = alcanceLuz;
        luzAlvo.intensity = 0f;
        luzAlvo.enabled = false;
        return luzAlvo;
    }

    private Vector3 EstimarPosicaoLocal(float fatorAltura)
    {
        if (renderizadorEmCache == null)
            renderizadorEmCache = GetComponentInChildren<Renderer>();

        if (renderizadorEmCache == null)
            return new Vector3(0f, fatorAltura * 2f, deslocamentoFrontalLente);

        Bounds limites = renderizadorEmCache.bounds;
        float x = limites.center.x;
        float y = Mathf.Lerp(limites.min.y, limites.max.y, fatorAltura);
        float z = limites.max.z + deslocamentoFrontalLente;
        return transform.InverseTransformPoint(new Vector3(x, y, z));
    }
}
