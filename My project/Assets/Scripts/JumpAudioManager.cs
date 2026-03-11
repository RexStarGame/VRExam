using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class JumpAudioManager : MonoBehaviour
{
    [Header("Lydfiler")]
    public AudioClip normalJumpClip;
    public AudioClip boostJumpClip;

    [Header("Lyd Indstillinger")]
    [Range(0.05f, 0.7f)]
    public float pitchVariation = 0.15f;

    private AudioSource audioSource;
    private bool isInBoostZone = false;

    private float lastPlayedTime = -100f;
    private float cooldownTime = 0.1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.priority = 0;
    }

    // Update() er fjernet! Vi lytter kun til direkte ordrer fra CubeMovement nu.

    public void PlayJumpSound()
    {
        // Tids-skjold mod utilsigtede fysik-kald
        if (Time.time < lastPlayedTime + cooldownTime) return;
        lastPlayedTime = Time.time;

        audioSource.Stop();

        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);

        // Vælger klip, tvinger nålen til 0, og afspiller
        if (isInBoostZone && boostJumpClip != null)
        {
            audioSource.clip = boostJumpClip;
            audioSource.time = 0f;
            audioSource.Play();
            isInBoostZone = false;
        }
        else if (normalJumpClip != null)
        {
            audioSource.clip = normalJumpClip;
            audioSource.time = 0f;
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<OneTimeJumpBoost>() != null) isInBoostZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<OneTimeJumpBoost>() != null) isInBoostZone = false;
    }
}
