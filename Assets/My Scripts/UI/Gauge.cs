#region
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Debug;
#endregion

[ExecuteInEditMode] public class Gauge : MonoBehaviour
{
    [Header("Gauge Settings")]
    [Tooltip("The maximum value of the gauge. \n The current gauge slider will clamp to this value.")]
    [SerializeField]
    [Range(0, 100)]
    float maxGauge;

    [Header("Cached References")] Slider gaugeSlider;

    [field: HideInInspector] [field: Range(0, 100)] public float CurrentGauge { get; set; }

    public float MaxGauge
    {
        get => maxGauge;
        set => maxGauge = value;
    }

    void Start()
    {
        gaugeSlider  = GetComponent<Slider>();
        maxGauge     = 100;
        CurrentGauge = 0;
    }

    void Update()
    {
        gaugeSlider.value    = CurrentGauge;
        gaugeSlider.maxValue = maxGauge;
    }

    //TODO: should i use lower-case "maxGauge" or is upper-case "MaxGauge" (a property) fine?
    public void DebugGauge() // Used for debugging in the editor.
    {
        Log($"Current Gauge: {CurrentGauge}");
    }
}
