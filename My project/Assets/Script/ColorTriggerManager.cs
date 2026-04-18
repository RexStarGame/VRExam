using UnityEngine;
using System.Collections.Generic;

public class ColorTriggerManager : MonoBehaviour
{
    public static ColorTriggerManager instance;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public float sensitivity = 50f;
    public float threshold = 0.1f;

    [Header("Pulse Control")]
    [Range(0.01f, 1f)]
    public float beatCooldown = 0.15f;
    private float nextBeatTime = 0f;

    [Header("Color Settings")]
    public List<Color> pulseColors;
    private List<int> colorShuffleList = new List<int>();
    private int listPointer = 0;

    public delegate void OnBeatAction(Color color);
    public event OnBeatAction OnMusicBeat;

    private float[] samples = new float[256];

    void Awake()
    {
        if (instance == null) instance = this;
        CreateShuffledList();
    }

    void Update()
    {
        if (audioSource == null || pulseColors.Count == 0 || !audioSource.isPlaying) return;

        if (Time.time < nextBeatTime) return;

        audioSource.GetOutputData(samples, 0);
        float sum = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        float rmsValue = Mathf.Sqrt(sum / samples.Length);
        float finalVolume = rmsValue * sensitivity;

        if (finalVolume > threshold)
        {
            nextBeatTime = Time.time + beatCooldown;

            OnMusicBeat?.Invoke(pulseColors[colorShuffleList[listPointer]]);

            listPointer++;

            if (listPointer >= colorShuffleList.Count)
            {
                CreateShuffledList();
            }
        }
    }

    void CreateShuffledList()
    {
        colorShuffleList.Clear();

        for (int i = 0; i < pulseColors.Count; i++)
        {
            colorShuffleList.Add(i);
        }

        for (int i = 0; i < colorShuffleList.Count; i++)
        {
            int temp = colorShuffleList[i];
            int randomIndex = Random.Range(i, colorShuffleList.Count);
            colorShuffleList[i] = colorShuffleList[randomIndex];
            colorShuffleList[randomIndex] = temp;
        }

        listPointer = 0;
    }
}