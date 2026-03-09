using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private static Menu instance;
    [SerializeField] private float timeToSwitchScenes = 5;
    private void Awake()
    {
        if (instance == null) { instance = this; }
    }
    private void Start()
    {
        PlayerEvents.instance.DeathEvent.AddListener(OnDeath);
    }
    public void OnDeath()
    {
        // a death animation maybe?
        Invoke(nameof(MainMenu), timeToSwitchScenes);
    }
    private void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
