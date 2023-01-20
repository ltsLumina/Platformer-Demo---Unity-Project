using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(Misc_DebugTools))]
public class Misc_DebugToolsEditor : Editor
{
    override public void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Misc_DebugTools misc_DebugTools = (Misc_DebugTools)target;

        GUILayout.Label("Debug Tools", EditorStyles.boldLabel);

        GUILayout.Space(25);

        GUILayout.Label("Set Fire Mode", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Semi")) misc_DebugTools.SetFireMode(Gun.FireMode.Semi);
        if (GUILayout.Button("Shotgun")) misc_DebugTools.SetFireMode(Gun.FireMode.Shotgun);
        GUILayout.EndHorizontal();
    }
}