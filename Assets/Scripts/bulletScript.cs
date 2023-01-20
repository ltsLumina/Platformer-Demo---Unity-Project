using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    Rigidbody2D bulletRigidbody2D;
    Collider2D collider2D;

    public float bulletSpeed = 10f;
    public float bulletLifeTime = 2f;

    void Start()
    {
        bulletRigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
    }

    void Update()
    {
        bulletRigidbody2D.AddForce(transform.right * bulletSpeed, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (collider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            bulletRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        if (collider2D.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}