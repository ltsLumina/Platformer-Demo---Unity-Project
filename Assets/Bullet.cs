using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    new Collider2D collider2D;

    void Awake() => collider2D = GetComponent<Collider2D>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collider2D.IsTouchingLayers(LayerMask.GetMask("Ground"))) Destroy(gameObject);
    }
}