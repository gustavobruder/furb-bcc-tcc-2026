using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ValidadorSemaforo : MonoBehaviour
{
    [Header("Carro")]
    private Rigidbody _rbCarroJogador;
    public SpawnerCarroSelecionado spawnerCarroSelecionado;

    [Header("Semaforo")]
    public Semaforo semaforo;

    [Header("Infracao")]
    public Notificacao notificacao;
    public ContadorInfracoes contadorInfracoes;

    private const string MensagemAvisoSinalAmarelo = "Cuidado ao avançar o sinal amarelo.";
    private const string MensagemInfracaoSinalVermelho = "Infração gravíssima por avançar o sinal vermelho.";

    private void Awake()
    {
        var areaCollider = GetComponent<Collider>();
        areaCollider.isTrigger = true;

        DefinirSemaforo();
    }

    private void Start()
    {
        DefinirRigidbodyCarroSelecionado();
        DefinirSemaforo();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ColliderPertenceAoCarroJogador(other))
            return;

        ValidarAvancoSemaforo();
    }

    private void ValidarAvancoSemaforo()
    {
        if (semaforo == null)
            return;

        if (semaforo.estadoAtual == Semaforo.EstadoSemaforo.Amarelo)
        {
            notificacao.MostrarNotificacaoAviso(MensagemAvisoSinalAmarelo);
            return;
        }

        if (semaforo.estadoAtual == Semaforo.EstadoSemaforo.Vermelho)
        {
            notificacao.MostrarNotificacaoAviso(MensagemInfracaoSinalVermelho);
            contadorInfracoes.AdicionarInfracaoGravissima(MensagemInfracaoSinalVermelho);
            return;
        }
    }

    private void DefinirRigidbodyCarroSelecionado()
    {
        if (_rbCarroJogador != null)
            return;

        if (spawnerCarroSelecionado == null || spawnerCarroSelecionado.CarrosSelecionado == null)
            return;

        _rbCarroJogador = spawnerCarroSelecionado.CarrosSelecionado.GetComponent<Rigidbody>();
    }

    private void DefinirSemaforo()
    {
        if (semaforo != null)
            return;

        semaforo = GetComponentInParent<Semaforo>();
    }

    private bool ColliderPertenceAoCarroJogador(Collider other)
    {
        if (_rbCarroJogador == null)
        {
            var carroJogador = other.GetComponentInParent<CarroJogador>();
            if (carroJogador == null)
                return false;

            _rbCarroJogador = carroJogador.GetComponent<Rigidbody>();
        }

        return other.attachedRigidbody == _rbCarroJogador || other.transform.IsChildOf(_rbCarroJogador.transform);
    }
}
