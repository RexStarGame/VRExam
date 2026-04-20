using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool Debug_Mode = false; // debug mode er slået fra som standard.
    public Button[] levelButtons;

    void Start()
    {
        // tjekker om brugeren er i debug mode, hvilket kun er muligt for os udviklere
        if (!Debug_Mode)
        {
            // Vi henter fremskridt.
            int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);

            // DEBUG: Dette vil fortælle os i Consolen hvad spillet husker
            Debug.Log("LevelSelector har indlæst: Spilleren har nået til Level " + reachedLevel);

            for (int i = 0; i < levelButtons.Length; i++)
            {
                // Hvis knappens index + 1 er større end hvad vi har nået, lås den.
                // i=0 (Level 1): 1 > reachedLevel?
                // i=1 (Level 2): 2 > reachedLevel?
                if (i + 1 > reachedLevel)
                {
                    levelButtons[i].interactable = false;
                }
                else
                {
                    levelButtons[i].interactable = true; // Sikrer os de er åbne
                }
            }
        }
        // DEBUG. ikke kommenteret.
        else
        {
            for (int i = 0; i < levelButtons.Length; i++)
            {
                    levelButtons[i].interactable = true; // Sikrer os de er åbne
            }
        }
    }
    public void ResetProgress()
    {
        // 1. Slet gemt data i PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Alt fremskridt er slettet!");

        // 2. Genindlæs den nuværende scene (Menuen) 
        // Så bliver knapperne låst med det samme uden du skal genstarte manuelt
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OpenLevel(int levelIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelIndex);
    }
}