using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private static Menu instance;
    [SerializeField] private float timeToSwitchScenes = 5;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
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
