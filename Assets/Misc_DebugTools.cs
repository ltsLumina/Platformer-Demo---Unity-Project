using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misc_DebugTools : MonoBehaviour
{
    Gun gun;

    void Awake()
    {
        gun = FindObjectOfType<Gun>();
    }

    public void SetFireMode(Gun.FireMode fireMode)
    {
        gun.CurrentFireMode = fireMode;
    }
}