using UnityEngine;

public class PortalScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerEvents.instance.GravityEvent.Invoke();
        }
    }
}
