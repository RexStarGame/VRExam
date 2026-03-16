using UnityEngine;
using UnityEngine.Events;

public class PlayerEvents : MonoBehaviour
{
    public static PlayerEvents instance;
    public UnityEvent DeathEvent, WinEvent;
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
        if (WinEvent == null)
        {
            WinEvent = new UnityEvent();
        }
        DeathEvent.AddListener(OnDeath);
        WinEvent.AddListener(OnWin);
    }
    private void OnDeath()
    {
        Debug.Log("Player died");
    }
    private void OnWin()
    {
        // make win animation
        Debug.Log("Player has won");
    }
}
