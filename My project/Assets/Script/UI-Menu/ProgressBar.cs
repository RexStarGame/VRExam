using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] float maxValue = 1;
    [SerializeField] GameObject Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Player == null) { Player = GameObject.FindWithTag("Player"); }
        gameObject.GetComponent<Slider>().maxValue = maxValue;
        gameObject.GetComponent<Slider>().minValue = Player.transform.position.x;
    }
    private void Update()
    {
        float value = Player.transform.position.x;
        if (value > gameObject.GetComponent<Slider>().maxValue) { value = gameObject.GetComponent<Slider>().maxValue; }
        else if (value < gameObject.GetComponent<Slider>().minValue) { value = gameObject.GetComponent<Slider>().minValue; }
        gameObject.GetComponent<Slider>().value = value;
    }
}
