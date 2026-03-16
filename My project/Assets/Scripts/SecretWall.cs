using UnityEngine;

public class SecretWall : MonoBehaviour
{
    public MeshRenderer wallRenderer;
    public float transparency = 0.3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Color c = wallRenderer.material.color;
            c.a = transparency;
            wallRenderer.material.color = c;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Color c = wallRenderer.material.color;
            c.a = 1f;
            wallRenderer.material.color = c;
        }
    }
}