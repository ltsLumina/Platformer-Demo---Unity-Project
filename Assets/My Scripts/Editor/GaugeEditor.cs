#region
using UnityEditor;
using static UnityEditor.EditorGUILayout;
#endregion

[CustomEditor(typeof(Gauge))] public class GaugeEditor : Editor
{
    override public void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Gauge gauge = (Gauge)target;
        gauge.CurrentGauge = IntSlider("Current Gauge", (int)gauge.CurrentGauge, 0, (int)gauge.MaxGauge);
    }
}
