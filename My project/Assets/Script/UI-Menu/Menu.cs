using UnityEngine;

public class Menu : MonoBehaviour
{
    private static Menu instance;
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

    }
}
