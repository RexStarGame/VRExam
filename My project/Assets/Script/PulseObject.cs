using UnityEngine;

public class PulseTarget : MonoBehaviour
{
    [Header("What should pulse?")]
    public bool pulseObject = true;
    public bool pulseSkybox = false;

    [Header("Object Settings")]
    public string objectColorProperty = "_BaseColor";
    public float objectFadeSpeed = 5f;

    [Header("Skybox Settings")]
    public string skyboxColorProperty = "_Tint";
    public float skyboxFadeSpeed = 2f;

    private Renderer rend;
    private Material objectMat;
    private Color originalObjectColor;
    private bool hasObjectColor = false;

    private Skybox skyboxComponent;
    private Material skyboxMat;
    private Color originalSkyboxColor;
    private bool hasSkyboxColor = false;

    private bool subscribed = false;

    void Awake()
    {
        SetupObjectMaterial();
        SetupSkyboxMaterial();
    }

    void OnEnable()
    {
        TrySubscribe();

        if (!subscribed)
            InvokeRepeating(nameof(CheckForManager), 0.1f, 0.5f);
    }

    void OnDisable()
    {
        Unsubscribe();
        CancelInvoke(nameof(CheckForManager));
    }

    void CheckForManager()
    {
        TrySubscribe();

        if (subscribed)
            CancelInvoke(nameof(CheckForManager));
    }

    void TrySubscribe()
    {
        if (ColorTriggerManager.instance != null && !subscribed)
        {
            ColorTriggerManager.instance.OnMusicBeat += Pulse;
            subscribed = true;
        }
    }

    void Unsubscribe()
    {
        if (ColorTriggerManager.instance != null && subscribed)
        {
            ColorTriggerManager.instance.OnMusicBeat -= Pulse;
            subscribed = false;
        }
    }

    void SetupObjectMaterial()
    {
        if (!pulseObject) return;

        rend = GetComponent<Renderer>();

        if (rend == null) return;

        objectMat = rend.material;

        if (objectMat.HasProperty(objectColorProperty))
        {
            originalObjectColor = objectMat.GetColor(objectColorProperty);
            hasObjectColor = true;
        }
    }

    void SetupSkyboxMaterial()
    {
        if (!pulseSkybox) return;

        skyboxComponent = GetComponent<Skybox>();

        if (skyboxComponent != null && skyboxComponent.material != null)
        {
            skyboxMat = new Material(skyboxComponent.material);
            skyboxComponent.material = skyboxMat;
        }
        else if (RenderSettings.skybox != null)
        {
            skyboxMat = new Material(RenderSettings.skybox);
            RenderSettings.skybox = skyboxMat;
        }

        if (skyboxMat != null && skyboxMat.HasProperty(skyboxColorProperty))
        {
            originalSkyboxColor = skyboxMat.GetColor(skyboxColorProperty);
            hasSkyboxColor = true;
        }
    }

    void Pulse(Color newColor)
    {
        if (pulseObject && objectMat != null && hasObjectColor)
        {
            objectMat.SetColor(objectColorProperty, newColor);
        }

        if (pulseSkybox && skyboxMat != null && hasSkyboxColor)
        {
            skyboxMat.SetColor(skyboxColorProperty, newColor);
            DynamicGI.UpdateEnvironment();
        }
    }

    void Update()
    {
        if (pulseObject && objectMat != null && hasObjectColor)
        {
            Color current = objectMat.GetColor(objectColorProperty);
            objectMat.SetColor(objectColorProperty,
                Color.Lerp(current, originalObjectColor, objectFadeSpeed * Time.deltaTime));
        }

        if (pulseSkybox && skyboxMat != null && hasSkyboxColor)
        {
            Color current = skyboxMat.GetColor(skyboxColorProperty);
            skyboxMat.SetColor(skyboxColorProperty,
                Color.Lerp(current, originalSkyboxColor, skyboxFadeSpeed * Time.deltaTime));
        }
    }
}