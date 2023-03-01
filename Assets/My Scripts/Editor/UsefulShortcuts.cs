#region
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
#endregion

internal static class UsefulShortcuts
{
    // Alt + C to clear the console.
    [Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Alt)]
    public static void ClearConsole()
    {
        Assembly   assembly = Assembly.GetAssembly(typeof(SceneView));
        Type       type     = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo method   = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
