using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGoal : MonoBehaviour
{
    [Header("UI Elementer")]
    public GameObject winMenuUI;

    [Header("Level Indstillinger")]
    public int nextLevelIndex;

    private bool canWin = false;

    void Awake()
    {
        if (winMenuUI != null) winMenuUI.SetActive(false);
    }

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Invoke("EnableWinning", 0.5f);
    }

    void EnableWinning() { canWin = true; }

    private void OnTriggerEnter(Collider other)
    {
        // DEBUG: Fortæl os hvad der rammer triggeren
        Debug.Log("Trigger ramt af: " + other.gameObject.name + " med Tag: " + other.tag);

        if (other.CompareTag("Player"))
        {
            if (canWin)
            {
                WinLevel();
            }
            else
            {
                Debug.Log("Spiller ramte målet, men canWin er falsk (for hurtig!)");
            }
        }
    }

    void WinLevel()
    {
        Debug.Log("--- WINLEVEL KØRER ---");

        if (winMenuUI != null)
        {
            winMenuUI.SetActive(true);
            Debug.Log("Menu aktiveret.");
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Tjek gemme-logikken
        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);
        Debug.Log("Gemt data før sejr: " + reachedLevel);
        Debug.Log("Prøver at låse op for: " + nextLevelIndex);

        if (nextLevelIndex > reachedLevel)
        {
            PlayerPrefs.SetInt("ReachedLevel", nextLevelIndex);
            PlayerPrefs.Save();
            Debug.Log("SUCCESS: Nyt level gemt i PlayerPrefs!");
        }
    }

    public void RetryLevel() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void NextLevel() { SceneManager.LoadScene(nextLevelIndex); }
    public void GoToMenu() { SceneManager.LoadScene(0); }
}