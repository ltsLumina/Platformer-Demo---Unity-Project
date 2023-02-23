using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedlineEffect : MonoBehaviour
{
    #region Speedline Variables
    ParticleSystem speedline;
    Rigidbody2D playerRB;
    [Header("Emission Variables")]
    [SerializeField] float emissionRate;
    #endregion

    void Start()
    {
        playerRB    = FindObjectOfType<PlayerMovement>().RB;
        speedline = FindObjectOfType<ParticleSystem>();

        AdjustLineSpeed(0);
    }

    void Update() => SpeedlineAdjustment();

    // ReSharper disable once MemberCanBePrivate.Global // Can be made private, but I want to keep it public for now.
    public void SpeedlineAdjustment()
    {
        switch (playerRB.velocity.x)
        {
            case > 30:
                AdjustLineSpeed(emissionRate);
                break;

            case < -30:
                AdjustLineSpeed(emissionRate);
                break;

            case < 30:
                AdjustLineSpeed(0);
                break;
        }
    }

    void AdjustLineSpeed(float acceleration)
    {
        ParticleSystem.EmissionModule emissionMod = speedline.emission;
        emissionMod.rateOverTime      = acceleration;
    }
}