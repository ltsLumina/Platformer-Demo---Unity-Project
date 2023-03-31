#region
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using static UnityEngine.Debug;
#endregion

public class CollisionCheck : MonoBehaviour
{
    #region Collision Variables
    [Header("Collision Variables")] [SerializeField]
    string[] hazardTags = { "Enemy", "Killplane", "Spike", "Hazard" };
    [SerializeField] float colDuration = 0.5f;
    [SerializeField] bool destroyOnCollision = true;

    // Private Variables
    SceneLoader sceneLoader;

    void Awake()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    public bool IsColliding { get; private set; }
    #endregion

    IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (hazardTags.Contains(other.gameObject.tag) && !IsColliding)
        {
            GameObject otherGameObject = other.gameObject;
            Log($"Collision Detected with {otherGameObject.name} \n Tag: {otherGameObject.tag}");
            IsColliding = true;

            yield return new WaitForSeconds(colDuration);

            Log($"Collision Ended with {other.gameObject.name}");

            if (destroyOnCollision)
            {
                Destroy(gameObject);
                sceneLoader.ReloadScene();
            }
        }
    }

    IEnumerator OnCollisionEnter2D(Collision2D other)
    {
        if (hazardTags.Contains(other.gameObject.tag) && !IsColliding)
        {
            GameObject otherGameObject = other.gameObject;
            Log($"Collision Detected with {otherGameObject.name} \n Tag: {otherGameObject.tag}");
            IsColliding = true;

            yield return new WaitForSeconds(colDuration);

            Log($"Collision Ended with {other.gameObject.name}");

            if (destroyOnCollision)
            {
                Destroy(gameObject);
                sceneLoader.ReloadScene();
            }
        }
    }
}