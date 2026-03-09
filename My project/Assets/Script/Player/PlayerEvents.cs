using UnityEngine;
using UnityEngine.Events;

public class PlayerEvents : MonoBehaviour
{
    public static PlayerEvents instance;
    public UnityEvent DeathEvent;
    private void Awake()
    {
        if (instance == null) { instance = this; }
    }
    private void Start()
    {
        if (DeathEvent == null)
        {
            DeathEvent = new UnityEvent();
        }
        DeathEvent.AddListener(OnDeath);
    }
    private void OnDeath()
    {
        Debug.Log("Player died");
    }
}
