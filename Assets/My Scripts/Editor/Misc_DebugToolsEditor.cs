#region
using UnityEditor;
using static UnityEngine.GUILayout;
#endregion

[CustomEditor(typeof(Misc_DebugTools))]
public class Misc_DebugToolsEditor : Editor
{
    override public void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Misc_DebugTools misc_DebugTools = (Misc_DebugTools)target;
        Gun             gun             = (Gun)FindObjectOfType(typeof(Gun)); //TODO: joel look i did it

        Label("Debug Tools", EditorStyles.boldLabel);

        Space(25);

        Label("Set Fire Mode", EditorStyles.boldLabel);
        BeginHorizontal();
        if (Button("Semi")) misc_DebugTools.SetFireMode(Gun.FireMode.Semi);
        if (Button("Shotgun")) misc_DebugTools.SetFireMode(Gun.FireMode.Shotgun);
        EndHorizontal();

        Label($"Current Fire Mode: {gun.CurrentFireMode}", EditorStyles.miniBoldLabel);

        Space(25);
    }
}