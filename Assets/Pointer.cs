#region
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

public class Pointer : MonoBehaviour
{
    //Configuration parameters
    [SerializeField] Vector2 offset;
    [SerializeField] float smoothing = 0.4f;
    [SerializeField] float enableAfterTime = 2.5f;

    //Private variables
    Vector3 velocity = Vector3.zero;
    Vector3 targetPosition;
    Transform playerPos;
    Rigidbody2D playerRB;
    SceneLoader sceneLoader;
    SpriteRenderer spriteRenderer;

    float timer;
    bool testBool;

    void Start()
    {
        playerPos = GameObject.FindWithTag("Player").transform;
        playerRB  = FindObjectOfType<PlayerMovement>().RB;
        sceneLoader = FindObjectOfType<SceneLoader>();

        transform.position = new Vector3(playerPos.position.x, playerPos.position.y + offset.y, transform.position.z);
        GetComponentInChildren<SpriteRenderer>().enabled = false;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void LateUpdate() => FollowPlayer();

    void FollowPlayer()
    {
        try
        {
            targetPosition =
                new Vector3(playerPos.position.x + offset.x, playerPos.position.y + offset.y,
                            transform.position.z);
        } catch (MissingReferenceException exception)
        {
            throw new Exception($"Error Detected! {exception} \n There is nothing to follow!");
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothing);

        EnablePointerAfterTime(enableAfterTime);
    }

    void EnablePointerAfterTime(float timeToCountTo)
    {
        if (playerRB.velocity.x == 0) timer += Time.deltaTime;
        else timer -= Time.deltaTime;

        GetComponentInChildren<SpriteRenderer>().enabled = timer >= timeToCountTo;
    }
}