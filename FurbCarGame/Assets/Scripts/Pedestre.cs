using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Pedestre : MonoBehaviour
{
    NavMeshAgent agente;
    Animator animador;
    public Semaforo semaforo;
    [FormerlySerializedAs("goals")] public Transform[] objetivos = new Transform[2];
    private int proximoObjetivo = 0;
    public float tempoEspera = 3f;
    private float contadorEspera = 0f;
    private bool estaEsperando = false;
    private const string ParametroVelocidade = "Speed_f";
    public float VelocidadeAnimacaoAndando = 0.3f;
    private const float VelocidadeAnimacaoParado = 0f;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animador = GetComponent<Animator>();
        if (objetivos.Length > 0 && objetivos[proximoObjetivo] != null)
        {
            agente.destination = objetivos[proximoObjetivo].position;
            DefinirVelocidadeAnimacao(VelocidadeAnimacaoAndando);
        }
    }

    void Update()
    {
        if (objetivos.Length == 0 || objetivos[proximoObjetivo] == null)
        {
            DefinirVelocidadeAnimacao(VelocidadeAnimacaoParado);
            return;
        }

        if (estaEsperando)
        {
            DefinirVelocidadeAnimacao(VelocidadeAnimacaoParado);
            agente.isStopped = true;

            if (contadorEspera > 0f)
                contadorEspera -= Time.deltaTime;

            if (contadorEspera <= 0f && !SemaforoBloqueando())
            {
                estaEsperando = false;
                proximoObjetivo++;

                if (proximoObjetivo > objetivos.Length - 1)
                    proximoObjetivo = 0;

                if (objetivos[proximoObjetivo] != null)
                {
                    agente.isStopped = false;
                    agente.destination = objetivos[proximoObjetivo].position;
                    DefinirVelocidadeAnimacao(VelocidadeAnimacaoAndando);
                }
            }

            return;
        }

        agente.isStopped = false;
        DefinirVelocidadeAnimacao(VelocidadeAnimacaoAndando);
        float distancia = Vector3.Distance(agente.transform.position, objetivos[proximoObjetivo].position);
        if (distancia < 0.5f)
        {
            estaEsperando = true;
            contadorEspera = tempoEspera;
            agente.ResetPath();
            DefinirVelocidadeAnimacao(VelocidadeAnimacaoParado);
        }
    }

    bool SemaforoBloqueando()
    {
        return semaforo != null &&
            (semaforo.estadoAtual == Semaforo.EstadoSemaforo.Verde ||
             semaforo.estadoAtual == Semaforo.EstadoSemaforo.Amarelo);
    }

    void DefinirVelocidadeAnimacao(float valor)
    {
        if (animador != null)
            animador.SetFloat(ParametroVelocidade, valor);
    }
}
