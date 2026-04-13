using UnityEngine;
using System.Collections;

public class ColiderCheck : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool debugMode = true; // Sat til true som standard mens du tester

    private void OnTriggerEnter(Collider other)
    {
        // 1. Skriv i konsollen HVER gang vi rører ved NOGET, uanset hvad det er (hvis debug mode er på)
        if (debugMode)
        {
            Debug.Log($"<color=yellow>[DEBUG]</color> Spilleren ramte: <b>{other.gameObject.name}</b> | Tag: <i>{other.tag}</i>");
        }

        // 2. Tjek om det vi ramte er en "Wall" (Døds-logikken)
        if (other.gameObject.CompareTag("Wall"))
        {
            // Skriv en ekstra tydelig rød besked, når døden faktisk indtræffer
            if (debugMode)
            {
                Debug.Log($"<color=red>[DØD]</color> Døds-event udløst af: <b>{other.gameObject.name}</b>!");
            }

            PlayerEvents.instance.DeathEvent.Invoke();

            // 3. Farv objektet rødt
            if (debugMode)
            {
                Renderer otherRenderer = other.gameObject.GetComponent<Renderer>();

                if (otherRenderer != null)
                {
                    otherRenderer.material.color = Color.red;
                }
                // TILFØJET: Tjek at parent faktisk eksisterer, før vi beder om dens Renderer
                else if (other.transform.parent != null)
                {
                    otherRenderer = other.transform.parent.GetComponent<Renderer>();
                    if (otherRenderer != null)
                    {
                        otherRenderer.material.color = Color.red;
                    }
                }
            }
        }
    }
}