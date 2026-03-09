using UnityEngine;

public class OneTimeJumpBoost : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("How much force to apply when the player jumps.")]
    public float superJumpForce = 15f;

    // OnTriggerStay runs every frame the player is inside the collider
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Rigidbody playerRb = other.GetComponent<Rigidbody>();

                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0, playerRb.linearVelocity.z);

                    playerRb.AddForce(Vector3.up * superJumpForce, ForceMode.Impulse);

                    Destroy(gameObject);
                }
            }
        }
    }
}