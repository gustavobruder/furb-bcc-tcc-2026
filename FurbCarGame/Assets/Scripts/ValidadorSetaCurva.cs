using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ValidadorSetaCurva : MonoBehaviour
{
    [Header("Carro")]
    private Rigidbody _rbCarroJogador;
    private CarroSetas _carroSetas;
    public SpawnerCarroSelecionado spawnerCarroSelecionado;

    [Header("Validacao")]
    public bool ValidarCurvaDireita = true;

    [Header("Infracao")]
    public Notificacao notificacao;
    public ContadorInfracoes contadorInfracoes;

    private const string MensagemInfracao = "Infração grave por fazer uma curva sem dar seta.";

    private bool _validarInfracao = false;
    private bool _infracaoAplicada = false;

    private void Awake()
    {
        var areaCollider = GetComponent<Collider>();
        areaCollider.isTrigger = true;
    }

    private void Start()
    {
        DefinirCarroSelecionado();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ColliderPertenceAoCarroJogador(other))
            return;

        _validarInfracao = true;
    }

    private void Update()
    {
        if (!_validarInfracao || _infracaoAplicada)
            return;

        if (SetaCorretaEstaLigada())
            return;

        notificacao.MostrarNotificacaoAviso(MensagemInfracao);
        contadorInfracoes.AdicionarInfracaoGrave(MensagemInfracao);

        _infracaoAplicada = true;
    }

    public void ResetarValidacoes()
    {
        _validarInfracao = false;
        _infracaoAplicada = false;
    }

    private void DefinirCarroSelecionado()
    {
        if (_rbCarroJogador != null && _carroSetas != null)
            return;

        if (spawnerCarroSelecionado == null || spawnerCarroSelecionado.CarrosSelecionado == null)
            return;

        _rbCarroJogador = spawnerCarroSelecionado.CarrosSelecionado.GetComponent<Rigidbody>();
        _carroSetas = ObterCarroSetas(spawnerCarroSelecionado.CarrosSelecionado);
    }

    private bool ColliderPertenceAoCarroJogador(Collider other)
    {
        if (_rbCarroJogador == null)
        {
            var carroJogador = other.GetComponentInParent<CarroJogador>();
            if (carroJogador == null)
                return false;

            _rbCarroJogador = carroJogador.GetComponent<Rigidbody>();
            _carroSetas = ObterCarroSetas(carroJogador.gameObject);
        }
        else if (_carroSetas == null)
        {
            _carroSetas = ObterCarroSetas(_rbCarroJogador.gameObject);
        }

        return other.attachedRigidbody == _rbCarroJogador || other.transform.IsChildOf(_rbCarroJogador.transform);
    }

    private bool SetaCorretaEstaLigada()
    {
        if (_carroSetas == null)
            return false;

        return ValidarCurvaDireita ? _carroSetas.SetaDireitaLigada : _carroSetas.SetaEsquerdaLigada;
    }

    private CarroSetas ObterCarroSetas(GameObject carro)
    {
        if (carro == null)
            return null;

        var carroJogador = carro.GetComponent<CarroJogador>();
        if (carroJogador != null && carroJogador.carroSetas != null)
            return carroJogador.carroSetas;

        var carroSetas = carro.GetComponent<CarroSetas>();
        if (carroSetas != null)
            return carroSetas;

        carroSetas = carro.GetComponentInChildren<CarroSetas>();
        if (carroSetas != null)
            return carroSetas;

        return carro.GetComponentInParent<CarroSetas>();
    }
}
