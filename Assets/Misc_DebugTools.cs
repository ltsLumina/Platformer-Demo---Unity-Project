using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Misc_DebugTools : MonoBehaviour
{
    Gun gun;

    void Awake()
    {
        gun = FindObjectOfType<Gun>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) ReloadScene(0);
    }

    public void SetFireMode(Gun.FireMode fireMode)
    {
        gun.CurrentFireMode = fireMode;
    }

    public void ReloadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}