using UnityEngine;
using UnityEngine.Events;

public class PlayerEvents : MonoBehaviour
{
    [SerializeField] public static PlayerEvents instance;
    [SerializeField] public UnityEvent DeathEvent, WinEvent, GravityEvent;
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
        GravityEvent.AddListener(OnGravity);
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
    private void OnGravity()
    {
        Debug.Log("Gravity event triggered")
    }
}
