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
        PlayerEvents.instance.WinEvent.AddListener(OnWin);
    }
    private void OnDeath()
    {
        Invoke(nameof(MainMenu), timeToSwitchScenes);
    }
    private void OnWin()
    {
        Invoke(nameof(MainMenu), timeToSwitchScenes);
    }

    private void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
