using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WaypointCorrida : MonoBehaviour
{
    [Header("Waypoint")]
    public GameObject proximoWaypoint;

    [Header("Carro")]
    private Rigidbody _rbCarroJogador;
    public SpawnerCarroSelecionado spawnerCarroSelecionado;

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

        if (proximoWaypoint != null)
            proximoWaypoint.SetActive(true);

        gameObject.SetActive(false);
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
