#region
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Debug;
#endregion

[ExecuteInEditMode]
public class Gauge : MonoBehaviour
{
    [Header("Gauge Settings")]
    [Tooltip("The maximum value of the gauge. \n The current gauge slider will clamp to this value.")]
    [SerializeField, Range(0, 100)] float maxGauge;
    [HideInInspector, Range(0, 100)] float currentGauge;
    
    public float CurrentGauge
    {
        get => currentGauge;
        set => currentGauge = value;
    }
    public float MaxGauge
    {
        get => maxGauge;
        set => maxGauge = value;
    }

    [Header("Cached References")]
    Slider gaugeSlider;

    void Start()
    {
        gaugeSlider  = GetComponent<Slider>();
        maxGauge     = 100;
        currentGauge = 0;
    }

    void Update()
    {
        gaugeSlider.value    = currentGauge;
        gaugeSlider.maxValue = maxGauge;
    }

    //TODO: should i use lower-case "maxGauge" or is upper-case "MaxGauge" (a property) fine?
    public void DebugGauge() // Used for debugging in the editor.
    {
        Log($"Current Gauge: {CurrentGauge}");
    }
}