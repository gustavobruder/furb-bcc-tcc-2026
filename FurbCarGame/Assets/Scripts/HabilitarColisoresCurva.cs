using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HabilitarColisoresCurva : MonoBehaviour
{
    [Header("Curvas")]
    public GameObject CurvaHabilitar;
    public GameObject CurvaDesabilitar;

    [Header("Carro")]
    private Rigidbody _rbCarroJogador;
    public SpawnerCarroSelecionado spawnerCarroSelecionado;

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

        if (CurvaHabilitar != null)
        {
            CurvaHabilitar.SetActive(true);
            CurvaHabilitar.GetComponent<ValidadorSetaCurva>().ResetarValidacoes();
        }

        if (CurvaDesabilitar != null)
            CurvaDesabilitar.SetActive(false);
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
}
