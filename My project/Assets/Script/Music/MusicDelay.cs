using UnityEngine;

public class MusicDelay : MonoBehaviour
{
    public AudioSource audioSource;
    public float delay = 1f; // seconds

    void Start()
    {
        audioSource.PlayDelayed(delay);
    }
}