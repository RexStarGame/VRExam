using UnityEngine;

public class PulseObject : MonoBehaviour
{
    private Renderer rend;
    private Material mat;
    private Color originalColor;
    public float fadeSpeed = 5f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            mat = rend.material;
            originalColor = mat.GetColor("_BaseColor");
        }
    }

    void OnEnable()
    {
        InvokeRepeating("CheckForManager", 0.1f, 0.5f);
    }

    void CheckForManager()
    {
        if (ColorTriggerManager.instance != null)
        {
            ColorTriggerManager.instance.OnMusicBeat += Pulse;
            CancelInvoke("CheckForManager");
        }
    }

    void OnDisable()
    {
        if (ColorTriggerManager.instance != null)
            ColorTriggerManager.instance.OnMusicBeat -= Pulse;
    }

    void Pulse(Color newColor)
    {
        if (mat != null)
        {
            mat.SetColor("_BaseColor", newColor);
        }
    }

    void Update()
    {
        if (mat != null)
        {
            Color current = mat.GetColor("_BaseColor");
            mat.SetColor("_BaseColor", Color.Lerp(current, originalColor, fadeSpeed * Time.deltaTime));
        }
    }
}