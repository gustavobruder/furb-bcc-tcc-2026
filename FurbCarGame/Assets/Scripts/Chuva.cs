using DigitalRuby.RainMaker;
using UnityEngine;

public class Chuva : MonoBehaviour
{
    private const string SimpleSkyMaterialName = "SimpleSky";
    private const string MainTextureProperty = "_MainTex";
    private const float DawnDuskMinRotation = 90.0f;
    private const float DawnDuskMaxRotation = 270.0f;
    private const float SkyDomeMinOffset = 0.0f;
    private const float SkyDomeMaxOffset = 0.5f;
    private const float SunMinIntensity = 0.0f;
    private const float SunMaxIntensity = 1.0f;

    public RainScript RainScript;
    public UnityEngine.UI.Slider RainSlider;
    public GameObject Sun;

    public static float IntensidadeAtual { get; private set; }

    [SerializeField]
    private Renderer skyDomeRenderer;

    private Material skyDomeMaterial;
    private Light sunLight;


    private void Start()
    {
        AtualizarIntensidade(RainSlider.value);
        RainScript.EnableWind = true;
    }

    public void RainSliderChanged(float valor)
    {
        AtualizarIntensidade(valor);
    }

    private void OnDisable()
    {
        IntensidadeAtual = 0.0f;
    }

    private void AtualizarIntensidade(float valor)
    {
        IntensidadeAtual = Mathf.Clamp01(valor);
        RainScript.RainIntensity = IntensidadeAtual;
    }

    public void DawnDuskSliderChanged(float valor)
    {
        float normalizedValue = Mathf.Clamp01(valor);
        float rotation = Mathf.Lerp(DawnDuskMinRotation, DawnDuskMaxRotation, normalizedValue);
        Sun.transform.rotation = Quaternion.Euler(rotation, 0.0f, 0.0f);
        UpdateSunIntensity(normalizedValue);
        UpdateSkyDomeOffset(normalizedValue);
    }

    private void UpdateSunIntensity(float valor)
    {
        if (sunLight == null)
        {
            sunLight = Sun.GetComponent<Light>();
        }

        if (sunLight == null)
        {
            Debug.LogWarning("The Sun object does not have a Light component.");
            return;
        }

        sunLight.intensity = Mathf.Lerp(SunMaxIntensity, SunMinIntensity, valor);
    }

    private void UpdateSkyDomeOffset(float valor)
    {
        Material material = GetSkyDomeMaterial();

        if (material == null)
        {
            return;
        }

        Vector2 offset = material.GetTextureOffset(MainTextureProperty);
        offset.x = Mathf.Lerp(SkyDomeMinOffset, SkyDomeMaxOffset, valor);
        material.SetTextureOffset(MainTextureProperty, offset);
    }

    private Material GetSkyDomeMaterial()
    {
        if (skyDomeMaterial != null)
        {
            return skyDomeMaterial;
        }

        if (skyDomeRenderer == null)
        {
            skyDomeRenderer = FindSkyDomeRenderer();
        }

        if (skyDomeRenderer == null)
        {
            Debug.LogWarning("SkyDome renderer with SimpleSky material was not found.");
            return null;
        }

        skyDomeMaterial = GetSimpleSkyMaterialInstance(skyDomeRenderer);
        return skyDomeMaterial;
    }

    private Renderer FindSkyDomeRenderer()
    {
        Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (HasSimpleSkyMaterial(renderers[i]))
            {
                return renderers[i];
            }
        }

        GameObject skyDome = GameObject.Find("SkyDome");

        if (skyDome == null)
        {
            return null;
        }

        renderers = skyDome.GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (HasSimpleSkyMaterial(renderers[i]))
            {
                return renderers[i];
            }
        }

        return renderers.Length > 0 ? renderers[0] : null;
    }

    private bool HasSimpleSkyMaterial(Renderer renderer)
    {
        Material[] materials = renderer.sharedMaterials;

        for (int i = 0; i < materials.Length; i++)
        {
            Material material = materials[i];

            if (material != null && material.name == SimpleSkyMaterialName)
            {
                return true;
            }
        }

        return false;
    }

    private Material GetSimpleSkyMaterialInstance(Renderer renderer)
    {
        Material[] sharedMaterials = renderer.sharedMaterials;

        for (int i = 0; i < sharedMaterials.Length; i++)
        {
            Material sharedMaterial = sharedMaterials[i];

            if (sharedMaterial != null && sharedMaterial.name == SimpleSkyMaterialName)
            {
                Material[] materials = renderer.materials;
                return i < materials.Length ? materials[i] : renderer.material;
            }
        }

        return renderer.material;
    }
}
