using UnityEngine;

public class CarroRodas : MonoBehaviour
{
    [Header("Rodas Colliders")]
    public WheelCollider rodaColliderFL;
    public WheelCollider rodaColliderFR;
    public WheelCollider rodaColliderRL;
    public WheelCollider rodaColliderRR;

    [Header("Rodas Meshs")]
    public Transform rodaMeshFL;
    public Transform rodaMeshFR;
    public Transform rodaMeshRL;
    public Transform rodaMeshRR;

    [Header("Aderencia sob chuva")]
    public Rigidbody rb;
    [Min(0f)] public float velocidadeInicioDeslizamentoKmH = 0f;
    [Min(0f)] public float velocidadeAderenciaMinimaKmH = 100f;
    [Range(0f, 0.95f)] public float reducaoMaximaAderenciaLateral = 0.65f;
    [Range(0f, 0.95f)] public float reducaoMaximaAderenciaLongitudinal = 0.35f;
    [Min(0.01f)] public float suavizacaoAderencia = 5f;

    private float _l;
    private float _w;
    private float _raioBase = 6f;
    private float _anguloMaxDirecao = 40f;
    private WheelCollider[] _rodas;
    private WheelFrictionCurve[] _atritosLateraisBase;
    private WheelFrictionCurve[] _atritosLongitudinaisBase;
    private float _fatorAderenciaLateralAtual = 1f;
    private float _fatorAderenciaLongitudinalAtual = 1f;

    private void Start()
    {
        if (!rb) rb = GetComponentInParent<Rigidbody>();

        _rodas = new[] { rodaColliderFL, rodaColliderFR, rodaColliderRL, rodaColliderRR };
        _atritosLateraisBase = new WheelFrictionCurve[_rodas.Length];
        _atritosLongitudinaisBase = new WheelFrictionCurve[_rodas.Length];

        for (int i = 0; i < _rodas.Length; i++)
        {
            if (!_rodas[i]) continue;

            _atritosLateraisBase[i] = _rodas[i].sidewaysFriction;
            _atritosLongitudinaisBase[i] = _rodas[i].forwardFriction;
        }

        var distanciaEntreEixos = Mathf.Abs(
            rodaColliderFL.transform.localPosition.z - rodaColliderRL.transform.localPosition.z
        );

        var larguraDianteira = Mathf.Abs(
            rodaColliderFL.transform.localPosition.x - rodaColliderFR.transform.localPosition.x
        );

        _l = distanciaEntreEixos;
        _w = larguraDianteira;
    }

    private void FixedUpdate()
    {
        AtualizarAderenciaChuva();
    }

    private void OnDisable()
    {
        if (_rodas == null) return;

        _fatorAderenciaLateralAtual = 1f;
        _fatorAderenciaLongitudinalAtual = 1f;
        AplicarAderencia(1f, 1f);
    }

    public void AplicarDirecaoVolante(float anguloDirecao)
    {
        if (anguloDirecao == 0)
        {
            if (rodaColliderFL) rodaColliderFL.steerAngle = 0;
            if (rodaColliderFR) rodaColliderFR.steerAngle = 0;
            return;
        }

        // geometria de Ackermann
        var r = _raioBase / Mathf.Abs(anguloDirecao);

        var anguloInterno = Mathf.Rad2Deg * Mathf.Atan(_l / (r - (_w / 2f)));
        var anguloExterno = Mathf.Rad2Deg * Mathf.Atan(_l / (r + (_w / 2f)));

        anguloInterno = Mathf.Clamp(anguloInterno, -_anguloMaxDirecao, _anguloMaxDirecao);
        anguloExterno = Mathf.Clamp(anguloExterno, -_anguloMaxDirecao, _anguloMaxDirecao);

        var virandoParaDireita = anguloDirecao > 0f;

        if (virandoParaDireita)
        {
            if (rodaColliderFL) rodaColliderFL.steerAngle = anguloExterno;
            if (rodaColliderFR) rodaColliderFR.steerAngle = anguloInterno;
        }
        else
        {
            if (rodaColliderFL) rodaColliderFL.steerAngle = -anguloInterno;
            if (rodaColliderFR) rodaColliderFR.steerAngle = -anguloExterno;
        }
    }

    public void AplicarTorqueMotor(float torqueMotor)
    {
        // tração dianteira (FWD)
        if (rodaColliderFL) rodaColliderFL.motorTorque = torqueMotor;
        if (rodaColliderFR) rodaColliderFR.motorTorque = torqueMotor;
        if (rodaColliderRL) rodaColliderRL.motorTorque = 0f;
        if (rodaColliderRR) rodaColliderRR.motorTorque = 0f;
    }

    public void AplicarFreioDianteiro(float freioDianteiro)
    {
        if (rodaColliderFL) rodaColliderFL.brakeTorque = freioDianteiro;
        if (rodaColliderFR) rodaColliderFR.brakeTorque = freioDianteiro;
    }

    public void AplicarFreioTraseiro(float freioTraseiro)
    {
        if (rodaColliderRL) rodaColliderRL.brakeTorque = freioTraseiro;
        if (rodaColliderRR) rodaColliderRR.brakeTorque = freioTraseiro;
    }

    public void AtualizarRodas()
    {
        AtualizarRoda(rodaColliderFL, rodaMeshFL);
        AtualizarRoda(rodaColliderFR, rodaMeshFR);
        AtualizarRoda(rodaColliderRL, rodaMeshRL);
        AtualizarRoda(rodaColliderRR, rodaMeshRR);
    }

    private void AtualizarAderenciaChuva()
    {
        if (!rb || _rodas == null) return;

        var velocidadeKmH = rb.linearVelocity.magnitude * 3.6f;
        var intervaloVelocidade = Mathf.Max(
            velocidadeAderenciaMinimaKmH - velocidadeInicioDeslizamentoKmH,
            0.01f
        );
        var fatorVelocidade = Mathf.Clamp01(
            (velocidadeKmH - velocidadeInicioDeslizamentoKmH) / intervaloVelocidade
        );
        var intensidadeDeslizamento = Chuva.IntensidadeAtual * fatorVelocidade;

        var aderenciaLateralAlvo = 1f - (reducaoMaximaAderenciaLateral * intensidadeDeslizamento);
        var aderenciaLongitudinalAlvo = 1f - (reducaoMaximaAderenciaLongitudinal * intensidadeDeslizamento);
        var fatorSuavizacao = 1f - Mathf.Exp(-suavizacaoAderencia * Time.fixedDeltaTime);

        _fatorAderenciaLateralAtual = Mathf.Lerp(
            _fatorAderenciaLateralAtual,
            aderenciaLateralAlvo,
            fatorSuavizacao
        );
        _fatorAderenciaLongitudinalAtual = Mathf.Lerp(
            _fatorAderenciaLongitudinalAtual,
            aderenciaLongitudinalAlvo,
            fatorSuavizacao
        );

        AplicarAderencia(_fatorAderenciaLateralAtual, _fatorAderenciaLongitudinalAtual);
    }

    private void AplicarAderencia(float fatorLateral, float fatorLongitudinal)
    {
        for (int i = 0; i < _rodas.Length; i++)
        {
            var roda = _rodas[i];
            if (!roda) continue;

            var atritoLateral = _atritosLateraisBase[i];
            atritoLateral.stiffness *= fatorLateral;
            roda.sidewaysFriction = atritoLateral;

            var atritoLongitudinal = _atritosLongitudinaisBase[i];
            atritoLongitudinal.stiffness *= fatorLongitudinal;
            roda.forwardFriction = atritoLongitudinal;
        }
    }

    private static void AtualizarRoda(WheelCollider rodaCollider, Transform rodaMesh)
    {
        Vector3 posicao;
        Quaternion rotacao;
        rodaCollider.GetWorldPose(out posicao, out rotacao);
        rodaMesh.position = posicao;
        rodaMesh.rotation = rotacao;
    }
}
