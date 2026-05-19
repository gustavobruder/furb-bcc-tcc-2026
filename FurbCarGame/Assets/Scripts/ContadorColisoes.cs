using System.Collections;
using Logitech;
using UnityEngine;
using UnityEngine.UI;

public class ContadorColisoes : MonoBehaviour
{
    public Text textoColisoes;

    [Header("Infracao")]
    public Notificacao notificacao;
    public ContadorInfracoes contadorInfracoes;

    private const string TagPedestre = "Pedestre";
    private const string MensagemInfracaoPedestre = "Infração gravíssima por atropelar pedestre.";

    private int _quantidadeColisoes = 0;
    private Coroutine _coroutineAtiva;

    private void Start()
    {
        DefinirReferenciasInfracao();
    }

    private void OnCollisionEnter(Collision collision)
    {
        _quantidadeColisoes++;

        DefinirTextoColisoes();

        AplicarEfeitoColisao();

        ValidarColisaoPedestre(collision);
    }

    public bool TemColisaoAtiva()
    {
        return _coroutineAtiva != null;
    }

    private void DefinirTextoColisoes()
    {
        textoColisoes.text = $"Colisões: {_quantidadeColisoes}";
    }

    private void AplicarEfeitoColisao()
    {
        if (_coroutineAtiva != null)
            StopCoroutine(_coroutineAtiva);

        _coroutineAtiva = StartCoroutine(AplicarEfeitoColisaoCoroutine());
    }

    private void ValidarColisaoPedestre(Collision collision)
    {
        if (!PossuiTagPedestre(collision.transform))
            return;

        DefinirReferenciasInfracao();

        if (notificacao != null)
            notificacao.MostrarNotificacaoAviso(MensagemInfracaoPedestre);

        if (contadorInfracoes != null)
            contadorInfracoes.AdicionarInfracaoGravissima(MensagemInfracaoPedestre);
    }

    private void DefinirReferenciasInfracao()
    {
        if (notificacao == null)
            notificacao = FindFirstObjectByType<Notificacao>();

        if (contadorInfracoes == null)
            contadorInfracoes = FindFirstObjectByType<ContadorInfracoes>();
    }

    private bool PossuiTagPedestre(Transform transform)
    {
        while (transform != null)
        {
            if (transform.CompareTag(TagPedestre))
                return true;

            transform = transform.parent;
        }

        return false;
    }

    private IEnumerator AplicarEfeitoColisaoCoroutine()
    {
        LogitechGSDK.LogiPlayDirtRoadEffect(0, 50);

        yield return new WaitForSeconds(0.5f);

        LogitechGSDK.LogiStopDirtRoadEffect(0);

        _coroutineAtiva = null;
    }
}
