using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;
using static UnityEngine.GUILayout;

[CustomEditor(typeof(Gauge))]
public class GaugeEditor : Editor
{
    override public void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Gauge gauge = (Gauge)target;
        gauge.CurrentGauge = IntSlider("Current Gauge", (int)gauge.CurrentGauge, 0, (int)gauge.MaxGauge);
    }
}