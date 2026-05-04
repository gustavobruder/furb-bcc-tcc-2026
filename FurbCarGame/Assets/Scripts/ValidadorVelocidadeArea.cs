using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ValidadorVelocidadeArea : MonoBehaviour
{
    [Header("Carro")]
    private Rigidbody _rbCarroJogador;
    public SpawnerCarroSelecionado spawnerCarroSelecionado;

    [Header("Validacao")]
    [Min(0f)] public float limiteVelocidadeKmH = 30f;

    [Header("Infracao")]
    public Notificacao notificacao;
    public ContadorInfracoes contadorInfracoes;

    private const string MensagemInfracao = "Multas por excesso de velocidade.";

    private readonly HashSet<Collider> _collidersCarroDentroDaArea = new HashSet<Collider>();
    private bool _infracaoAplicadaNestaEntrada = false;

    private void Awake()
    {
        var areaCollider = GetComponent<Collider>();
        areaCollider.isTrigger = true;
    }

    private void Start()
    {
        DefinirRigidbodyCarroSelecionado();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ColliderPertenceAoCarroJogador(other))
            return;

        if (_collidersCarroDentroDaArea.Count == 0)
            _infracaoAplicadaNestaEntrada = false;

        _collidersCarroDentroDaArea.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!ColliderPertenceAoCarroJogador(other))
            return;

        _collidersCarroDentroDaArea.Remove(other);

        if (_collidersCarroDentroDaArea.Count == 0)
            _infracaoAplicadaNestaEntrada = false;
    }

    private void Update()
    {
        if (_infracaoAplicadaNestaEntrada || !CarroEstaDentroDaArea())
            return;

        if (ObterVelocidadeCarroKmH() <= limiteVelocidadeKmH)
            return;

        notificacao.MostrarNotificacaoAviso(MensagemInfracao);
        contadorInfracoes.AdicionarInfracaoMedia(MensagemInfracao);
        _infracaoAplicadaNestaEntrada = true;
    }

    private void DefinirRigidbodyCarroSelecionado()
    {
        if (_rbCarroJogador != null)
            return;

        if (spawnerCarroSelecionado == null || spawnerCarroSelecionado.CarrosSelecionado == null)
            return;

        _rbCarroJogador = spawnerCarroSelecionado.CarrosSelecionado.GetComponent<Rigidbody>();
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

    private bool CarroEstaDentroDaArea()
    {
        return _collidersCarroDentroDaArea.Count > 0;
    }

    private float ObterVelocidadeCarroKmH()
    {
        return _rbCarroJogador.linearVelocity.magnitude * 3.6f;
    }
}
