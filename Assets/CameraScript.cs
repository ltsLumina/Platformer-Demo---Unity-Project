#region
using UnityEngine;
#endregion

public class CameraScript : MonoBehaviour
{
    //Configuration parameters
    [SerializeField] Transform targetToFollow;
    [SerializeField] float smoothing = 0.6f;

    //Private variables
    Vector3 velocity = Vector3.zero;

    void Start()
    {
        transform.position = new Vector3(targetToFollow.position.x, targetToFollow.position.y, transform.position.z);
    }

    void LateUpdate()
    {
        Vector3 targetPosition =
            new Vector3(targetToFollow.position.x, targetToFollow.position.y, transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothing);
    }
}