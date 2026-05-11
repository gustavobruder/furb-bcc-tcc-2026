using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LinhaChegadaCorrida : MonoBehaviour
{
    [Header("Carro")]
    private Rigidbody _rbCarroJogador;
    public SpawnerCarroSelecionado spawnerCarroSelecionado;

    [Header("Cronometro")]
    private Cronometro _cronometro;

    [Header("Notificacao")]
    public Notificacao notificacao;

    private void Awake()
    {
        var areaCollider = GetComponent<BoxCollider>();
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

        _cronometro.PararCronometro();
        notificacao.MostrarNotificacaoSucesso("Volta concluída com sucesso!");
        _cronometro.IniciarCronometro();
    }

    private void DefinirRigidbodyCarroSelecionado()
    {
        if (_rbCarroJogador != null)
            return;

        if (spawnerCarroSelecionado == null || spawnerCarroSelecionado.CarrosSelecionado == null)
            return;

        _rbCarroJogador = spawnerCarroSelecionado.CarrosSelecionado.GetComponent<Rigidbody>();
        _cronometro = spawnerCarroSelecionado.CarrosSelecionado.GetComponent<Cronometro>();
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
