#region
using System;
using UnityEngine;
using static UnityEngine.Debug;
#endregion

[RequireComponent(typeof(Camera))]
public class CameraScript : MonoBehaviour
{
    //Configuration parameters
    [SerializeField] Transform targetToFollow;
    [SerializeField] float smoothing = 0.4f;
    [SerializeField] float yOffset, xOffset;

    //Private variables
    Vector3 velocity = Vector3.zero;
    Vector3 targetPosition;

    void Start()
    {
        transform.position = new Vector3(targetToFollow.position.x, targetToFollow.position.y + yOffset, transform.position.z);
    }

    void LateUpdate()
    {
        try
        {
            targetPosition =
                new Vector3(targetToFollow.position.x + xOffset, targetToFollow.position.y + yOffset,
                            transform.position.z);
        } catch (MissingReferenceException exception)
        {
            throw new Exception($"Error Detected! {exception}");
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothing);
    }
}