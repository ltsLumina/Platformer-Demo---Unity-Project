using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FixCamera : MonoBehaviour
{
    FollowPlayer playerCamera;
    [SerializeField] bool area1;
    [SerializeField] bool area2;
    [SerializeField] bool area3;
    [SerializeField] bool area4;
    [SerializeField] bool start;

    void Awake() { playerCamera = FindObjectOfType<FollowPlayer>(); }

    void OnTriggerEnter2D(Collider2D col)
    {
        switch (area1 || area2 || area3 || area4)
        {
            case true when col.gameObject.CompareTag("Player") && start:
                playerCamera.XOffset = 3.5f;
                playerCamera.YOffset = 5;
                break;

            case true when col.gameObject.CompareTag("Player") && area1:
                playerCamera.XOffset = 4;
                playerCamera.YOffset = -3;
                break;

            case true when col.gameObject.CompareTag("Player") && area2:
                playerCamera.XOffset = 3.5f;
                playerCamera.YOffset = 5;
                break;

            case true when col.gameObject.CompareTag("Player") && area3:
                playerCamera.XOffset = 2;
                playerCamera.YOffset = -6.5f;
                break;

            case true when col.gameObject.CompareTag("Player") && area4:
                playerCamera.XOffset = 3.5f;
                playerCamera.YOffset = 5;
                break;


        }
    }
}