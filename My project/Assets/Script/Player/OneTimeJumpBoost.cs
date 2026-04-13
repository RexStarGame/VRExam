using UnityEngine;

public class OneTimeJumpBoost : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("Hvor meget fart (velocity) spilleren får opad.")]
    public float superJumpVelocity = 15f; // Omdøbt fra superJumpForce for at matche den nye logik

    private bool isPlayerInZone = false;
    private Rigidbody playerRb;

    private JumpAudioManager audioManager;

    private void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.Space))
        {
            if (playerRb != null)
            {
                if (audioManager != null)
                {
                    audioManager.PlayJumpSound();
                }

                // LØSNINGEN: Vi sætter hastigheden direkte i stedet for at bruge AddForce!
                // På denne måde er det ligegyldigt, om vi rører ved 1, 2 eller 5 boost-pads. 
                // Hastigheden bliver altid sat til præcis 'superJumpVelocity' (f.eks. 15).
                playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, superJumpVelocity, playerRb.linearVelocity.z);

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