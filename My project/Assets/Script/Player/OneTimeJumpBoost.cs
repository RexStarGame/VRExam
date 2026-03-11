using UnityEngine;

public class OneTimeJumpBoost : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("How much force to apply when the player jumps.")]
    public float superJumpForce = 15f;

    private bool isPlayerInZone = false;
    private Rigidbody playerRb;

    // Vi gemmer en reference til lyd-scriptet her
    private JumpAudioManager audioManager;

    private void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.Space))
        {
            if (playerRb != null)
            {
                // Vi beder lyd-scriptet spille lyden LIGE NU, før vi hopper!
                if (audioManager != null)
                {
                    audioManager.PlayJumpSound();
                }

                playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0, playerRb.linearVelocity.z);
                playerRb.AddForce(Vector3.up * superJumpForce, ForceMode.Impulse);

                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            playerRb = other.GetComponent<Rigidbody>();

            // Vi finder lyd-scriptet på din pixel art alien, når den træder ind
            audioManager = other.GetComponent<JumpAudioManager>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            playerRb = null;
            audioManager = null;
        }
    }
}