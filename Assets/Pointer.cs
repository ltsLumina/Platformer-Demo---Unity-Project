using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Pointer : MonoBehaviour
{
    //Configuration parameters
    [SerializeField] Transform playerPos;
    [SerializeField] Vector2 offset;
    [SerializeField] float smoothing = 0.4f;
    [SerializeField] float length = 0.75f;

    //Private variables
    Vector3 velocity = Vector3.zero;
    Vector3 targetPosition;

    void Start()
    {
        transform.position = new Vector3(playerPos.position.x, playerPos.position.y + offset.y, transform.position.z);
    }

    void LateUpdate()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        try
        {
            targetPosition =
                new Vector3(playerPos.position.x + offset.x, playerPos.position.y + offset.y,
                            transform.position.z);
        } catch (MissingReferenceException exception) { throw new Exception($"Error Detected! {exception}"); }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothing);

        if (playerPos.GetComponent<Rigidbody2D>().velocity.x < 0)
        {
            Oscillate();
        }
    }

    void Oscillate()
    {
        transform.position = new Vector3(offset.x, Mathf.PingPong(Time.time, length), Quaternion.identity.z);
    }
}