#region
using System;
using UnityEngine;
#endregion

[RequireComponent(typeof(Camera))]
public class FollowPlayer : MonoBehaviour
{
    //Configuration parameters
    [SerializeField] float smoothing = 0.4f;
    [SerializeField] float yOffset, xOffset;

    [SerializeField] float shakeIntensity;

    public float ShakeIntensity
    {
        get => shakeIntensity;
        set => shakeIntensity = value;
    }

    //Private variables
    Vector3 velocity = Vector3.zero;
    Vector3 targetPosition;
    Transform targetToFollow;

    void Start()
    {
        targetToFollow = FindObjectOfType<PlayerMovement>().transform;

        transform.position = new Vector3(targetToFollow.position.x, targetToFollow.position.y + yOffset,
                                         transform.position.z);
    }

    void LateUpdate()
    {
        try
        {
            targetPosition =
                new Vector3(targetToFollow.position.x + xOffset, targetToFollow.position.y + yOffset,
                            transform.position.z);
        } catch (MissingReferenceException exception) { throw new Exception($"Error Detected! {exception}"); }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothing);
    }
}