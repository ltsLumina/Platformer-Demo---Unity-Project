using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;
using static UnityEngine.GUILayout; //TODO: Why does this work/do what it does

[CustomEditor(typeof(Misc_DebugTools))]
public class Misc_DebugToolsEditor : Editor
{
    override public void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Misc_DebugTools misc_DebugTools = (Misc_DebugTools)target;
        Gun gun = (Gun)FindObjectOfType(typeof(Gun)); //TODO: joel look i did it

        Label("Debug Tools", EditorStyles.boldLabel);

            GUILayout.Space(25);

        Label("Set Fire Mode", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        if (Button("Semi")) misc_DebugTools.SetFireMode(Gun.FireMode.Semi);
        if (Button("Shotgun")) misc_DebugTools.SetFireMode(Gun.FireMode.Shotgun);
        GUILayout.EndHorizontal();

        Label($"Current Fire Mode: {gun.CurrentFireMode}", EditorStyles.miniBoldLabel);

            GUILayout.Space(25);
    }
}