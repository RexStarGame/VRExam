using UnityEngine;
using System.Collections;

public class ColiderCheck : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            PlayerEvents.instance.DeathEvent.Invoke(); 
        }
    }


    /*
    [Header("Lyd Indstillinger")]
    
    public AudioSource backgroundMusic;
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        HitDirection hitDir = ReturnDirection(collision);

        if (hitDir != HitDirection.Top && hitDir != HitDirection.Bottom && hitDir != HitDirection.None)
        {
            CubeMovement cubeMovement = collision.collider.GetComponentInParent<CubeMovement>();
            if (cubeMovement != null)
                cubeMovement.enabled = false;

            Rigidbody playerRb = collision.collider.GetComponentInParent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
            }
            StopMusic();
            PlayerEvents.instance.DeathEvent.Invoke();

        }
    }
    private void StopMusic()
    {
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }
    }

    private enum HitDirection { None, Top, Bottom, Forward, Back, Left, Right }

    private HitDirection ReturnDirection(Collision collision)
    {

        HitDirection hitDirection = HitDirection.None;

        Vector3 normal = collision.GetContact(0).normal;

        if (Vector3.Angle(normal, transform.up) < 45f) { hitDirection = HitDirection.Top; }
        else if (Vector3.Angle(normal, -transform.up) < 45f) { hitDirection = HitDirection.Bottom; }
        else if (Vector3.Angle(normal, transform.forward) < 45f) { hitDirection = HitDirection.Forward; }
        else if (Vector3.Angle(normal, -transform.forward) < 45f) { hitDirection = HitDirection.Back; }
        else if (Vector3.Angle(normal, transform.right) < 45f) { hitDirection = HitDirection.Right; }
        else if (Vector3.Angle(normal, -transform.right) < 45f) { hitDirection = HitDirection.Left; }

        return hitDirection;
    }
    */
}