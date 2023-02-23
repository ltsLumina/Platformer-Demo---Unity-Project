using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Debug;

public class CheckCollision : MonoBehaviour
{
    #region Collision Variables
    readonly string[] hazardTags = {"Enemy", "Killplane", "Spike", "Hazard"};

    [SerializeField] float colDuration = 0.5f;
    [SerializeField] bool destroyOnCollision = true;

    public bool IsColliding { get; private set; }
    #endregion

    IEnumerator OnCollisionEnter2D(Collision2D other)
    {
        if (hazardTags.Contains(other.gameObject.tag) && !IsColliding)
        {
            var otherGameObject = other.gameObject;
            Log($"Collision Detected with {otherGameObject.name} \n Tag: {otherGameObject.tag}");
            IsColliding = true;

            yield return new WaitForSeconds(colDuration);

            Log($"Collision Ended with {other.gameObject.name}");
            if (destroyOnCollision) Destroy(gameObject);
        }
    }
}