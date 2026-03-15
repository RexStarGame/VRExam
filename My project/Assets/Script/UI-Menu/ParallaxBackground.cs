using UnityEngine;
using UnityEngine.UI;

public class ParallaxBackground : MonoBehaviour
{
    public float parallaxEffect; // Juster denne værdi for at styre parallax-effektens styrke
    public float backgroundSpeed; // Juster denne værdi for at styre baggrundens hastighed

    private Image image; // Brug Image i stedet for RawImage
    private RectTransform rectTransform;
    private float backgroundWidth;

    void Start()
    {
        image = GetComponent<Image>(); // Hent Image komponenten
        rectTransform = GetComponent<RectTransform>();
        // backgroundWidth er ikke nødvendig for Image komponenten på samme måde
    }

    void Update()
    {
        // Beregn baggrundens forskydning baseret på parallax-effekten og hastigheden
        float offset = Time.time * backgroundSpeed * parallaxEffect;

        // For Image komponenter fungerer det lidt anderledes.
        // Vi justerer `rectTransform.anchoredPosition` i stedet for `uvRect`.
        // Dette flytter selve billedet.
        Vector2 currentPos = rectTransform.anchoredPosition;
        currentPos.x = offset; // Flyt baggrunden horisontalt
        rectTransform.anchoredPosition = currentPos;
    }
}