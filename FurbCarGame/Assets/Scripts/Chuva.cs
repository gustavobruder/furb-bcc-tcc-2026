using DigitalRuby.RainMaker;
using UnityEngine;

public class Chuva : MonoBehaviour
{
    public RainScript RainScript;
    public UnityEngine.UI.Slider RainSlider;
    public GameObject Sun;

    private Quaternion originalRotation;

    private void Start()
    {
        originalRotation = transform.localRotation;
        RainScript.RainIntensity = RainSlider.value = 0.5f;
        RainScript.EnableWind = true;
    }

    public void RainSliderChanged(float valor)
    {
        RainScript.RainIntensity = valor;
    }

    public void DawnDuskSliderChanged(float valor)
    {
        Sun.transform.rotation = Quaternion.Euler(valor, 0.0f, 0.0f);
    }
}
