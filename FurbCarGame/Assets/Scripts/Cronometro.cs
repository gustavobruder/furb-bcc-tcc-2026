using UnityEngine;
using UnityEngine.UI;

public class Cronometro : MonoBehaviour
{
    public Text textoCronometro;
    public Text textoCronometroMelhorTempo;

    private float _tempo = 0f;
    private float _melhorTempo = 0f;
    private bool _cronometrando = true;

    private void Update()
    {
        if (_cronometrando)
        {
            _tempo += Time.deltaTime;
            AtualizarCronometroTempo();
        }
    }

    private void AtualizarCronometroTempo()
    {
        if (textoCronometro == null) return;

        var minutos = Mathf.FloorToInt(_tempo / 60);
        var segundos = Mathf.FloorToInt(_tempo % 60);
        textoCronometro.text = $"Tempo: {minutos:00}:{segundos:00}";
    }

    private void AtualizarCronometroMelhorTempo()
    {
        if (textoCronometroMelhorTempo == null) return;

        var minutos = Mathf.FloorToInt(_melhorTempo / 60);
        var segundos = Mathf.FloorToInt(_melhorTempo % 60);
        textoCronometroMelhorTempo.text = $"Melhor: {minutos:00}:{segundos:00}";
    }

    public void IniciarCronometro()
    {
        _tempo = 0f;
        _cronometrando = true;
    }

    public void PararCronometro()
    {
        if (_cronometrando)
        {
            _cronometrando = false;

            if (_melhorTempo == 0 || _tempo < _melhorTempo)
            {
                _melhorTempo = _tempo;
                AtualizarCronometroMelhorTempo();
            }
        }
    }
}
