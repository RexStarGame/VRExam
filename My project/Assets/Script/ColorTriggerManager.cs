using UnityEngine;
using System.Collections.Generic;

public class ColorTriggerManager : MonoBehaviour
{
    public static ColorTriggerManager instance;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public float sensitivity = 500f;
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    [Header("General Smoothing")]
    [Range(1f, 30f)]
    public float smoothingSpeed = 12f;

    [Header("Tier 1 - Rolige farver")]
    public float threshold1 = 0.02f;

    [Range(0.01f, 1f)]
    public float beatCooldown1 = 0.20f;

    public float minHz1 = 20f;
    public float maxHz1 = 250f;

    public List<Color> pulseColorsTier1 = new List<Color>();
    private List<int> colorShuffleList1 = new List<int>();
    private int listPointer1 = 0;
    private float nextBeatTime1 = 0f;
    private float smoothedValue1 = 0f;

    [SerializeField] private bool aktiv_1 = true;

    [Header("Tier 2 - Lidt stærkere farver")]
    public float threshold2 = 0.03f;

    [Range(0.01f, 1f)]
    public float beatCooldown2 = 0.12f;

    public float minHz2 = 250f;
    public float maxHz2 = 2000f;

    public List<Color> pulseColorsTier2 = new List<Color>();
    private List<int> colorShuffleList2 = new List<int>();
    private int listPointer2 = 0;
    private float nextBeatTime2 = 0f;
    private float smoothedValue2 = 0f;

    [SerializeField] private bool aktiv_2 = true;

    [Header("Tier 3 - Mest intense farver")]
    public float threshold3 = 0.04f;

    [Range(0.01f, 1f)]
    public float beatCooldown3 = 0.08f;

    public float minHz3 = 2000f;
    public float maxHz3 = 8000f;

    public List<Color> pulseColorsTier3 = new List<Color>();
    private List<int> colorShuffleList3 = new List<int>();
    private int listPointer3 = 0;
    private float nextBeatTime3 = 0f;
    private float smoothedValue3 = 0f;

    [SerializeField] private bool aktiv_3 = true;

    [Header("Tier 4 - Mest intense farver")]
    public float threshold4 = 0.04f;

    [Range(0.01f, 1f)]
    public float beatCooldown4 = 0.08f;

    public float minHz4 = 2000f;
    public float maxHz4 = 8000f;

    public List<Color> pulseColorsTier4 = new List<Color>();
    private List<int> colorShuffleList4 = new List<int>();
    private int listPointer4 = 0;
    private float nextBeatTime4 = 0f;
    private float smoothedValue4 = 0f;

    [SerializeField] private bool aktiv_4 = true;

    [Header("Tier 5 - Mest intense farver")]
    public float threshold5 = 0.04f;

    [Range(0.01f, 1f)]
    public float beatCooldown5 = 0.08f;

    public float minHz5 = 2000f;
    public float maxHz5 = 8000f;

    public List<Color> pulseColorsTier5 = new List<Color>();
    private List<int> colorShuffleList5 = new List<int>();
    private int listPointer5 = 0;
    private float nextBeatTime5 = 0f;
    private float smoothedValue5 = 0f;

    [SerializeField] private bool aktiv_5 = false;

    [Header("Debug Values")]
    public float currentValue1;
    public float currentValue2;
    public float currentValue3;
    public float currentValue4;
    public float currentValue5;

    public delegate void OnBeatAction(Color color);
    public event OnBeatAction OnMusicBeat;

    private float[] spectrum = new float[512];

    void Awake()
    {
        if (instance == null)
            instance = this;

        CreateShuffledList(pulseColorsTier1, colorShuffleList1, ref listPointer1);
        CreateShuffledList(pulseColorsTier2, colorShuffleList2, ref listPointer2);
        CreateShuffledList(pulseColorsTier3, colorShuffleList3, ref listPointer3);
        CreateShuffledList(pulseColorsTier4, colorShuffleList4, ref listPointer4);
        CreateShuffledList(pulseColorsTier5, colorShuffleList5, ref listPointer5);
    }

    void Update()
    {
        if (audioSource == null || !audioSource.isPlaying)
            return;

        audioSource.GetSpectrumData(spectrum, 0, fftWindow);

        float rawValue1 = GetFrequencyRangeValue(minHz1, maxHz1) * sensitivity;
        float rawValue2 = GetFrequencyRangeValue(minHz2, maxHz2) * sensitivity;
        float rawValue3 = GetFrequencyRangeValue(minHz3, maxHz3) * sensitivity;
        float rawValue4 = GetFrequencyRangeValue(minHz4, maxHz4) * sensitivity;
        float rawValue5 = GetFrequencyRangeValue(minHz5, maxHz5) * sensitivity;

        smoothedValue1 = Mathf.Lerp(smoothedValue1, rawValue1, smoothingSpeed * Time.deltaTime);
        smoothedValue2 = Mathf.Lerp(smoothedValue2, rawValue2, smoothingSpeed * Time.deltaTime);
        smoothedValue3 = Mathf.Lerp(smoothedValue3, rawValue3, smoothingSpeed * Time.deltaTime);
        smoothedValue4 = Mathf.Lerp(smoothedValue4, rawValue4, smoothingSpeed * Time.deltaTime);
        smoothedValue5 = Mathf.Lerp(smoothedValue5, rawValue5, smoothingSpeed * Time.deltaTime);   

        currentValue1 = smoothedValue1;
        currentValue2 = smoothedValue2;
        currentValue3 = smoothedValue3;
        currentValue4 = smoothedValue4;
        currentValue5 = smoothedValue5;

        // Højeste tier først
        if (smoothedValue5 > threshold5 && aktiv_5 == true)
        {
            TryPlayTier(
                pulseColorsTier4,
                colorShuffleList4,
                ref listPointer4,
                ref nextBeatTime4,
                beatCooldown4
            );
            return;
        }
        if (smoothedValue4 > threshold4 && aktiv_4 == true)
        {
            TryPlayTier(
                pulseColorsTier4,
                colorShuffleList4,
                ref listPointer4,
                ref nextBeatTime4,
                beatCooldown4
            );
            return;
        }
        if (smoothedValue3 > threshold3 && aktiv_3 == true)
        {
            TryPlayTier(
                pulseColorsTier3,
                colorShuffleList3,
                ref listPointer3,
                ref nextBeatTime3,
                beatCooldown3
            );
            return;
        }

        if (smoothedValue2 > threshold2 && aktiv_2 == true)
        {
            TryPlayTier(
                pulseColorsTier2,
                colorShuffleList2,
                ref listPointer2,
                ref nextBeatTime2,
                beatCooldown2
            );
            return;
        }

        if (smoothedValue1 > threshold1 && aktiv_1 == true)
        {
            TryPlayTier(
                pulseColorsTier1,
                colorShuffleList1,
                ref listPointer1,
                ref nextBeatTime1,
                beatCooldown1
            );
        }
    }
    public void ResetData()
    {
        PlayerPrefs.DeleteAll();

    }

    float GetFrequencyRangeValue(float minHz, float maxHz)
    {
        float nyquist = AudioSettings.outputSampleRate * 0.5f;

        int minIndex = Mathf.FloorToInt((minHz / nyquist) * spectrum.Length);
        int maxIndex = Mathf.CeilToInt((maxHz / nyquist) * spectrum.Length);

        minIndex = Mathf.Clamp(minIndex, 0, spectrum.Length - 1);
        maxIndex = Mathf.Clamp(maxIndex, 0, spectrum.Length - 1);

        if (maxIndex < minIndex)
        {
            int temp = minIndex;
            minIndex = maxIndex;
            maxIndex = temp;
        }

        float sum = 0f;
        int count = 0;

        for (int i = minIndex; i <= maxIndex; i++)
        {
            sum += spectrum[i];
            count++;
        }

        if (count == 0)
            return 0f;

        return sum / count;
    }

    void TryPlayTier(
        List<Color> pulseColors,
        List<int> colorShuffleList,
        ref int listPointer,
        ref float nextBeatTime,
        float beatCooldown)
    {
        if (pulseColors == null || pulseColors.Count == 0)
            return;

        if (Time.time < nextBeatTime)
            return;

        if (colorShuffleList.Count != pulseColors.Count || colorShuffleList.Count == 0)
        {
            CreateShuffledList(pulseColors, colorShuffleList, ref listPointer);
        }

        OnMusicBeat?.Invoke(pulseColors[colorShuffleList[listPointer]]);

        listPointer++;

        if (listPointer >= colorShuffleList.Count)
        {
            CreateShuffledList(pulseColors, colorShuffleList, ref listPointer);
        }

        nextBeatTime = Time.time + beatCooldown;
    }

    void CreateShuffledList(List<Color> pulseColors, List<int> colorShuffleList, ref int listPointer)
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