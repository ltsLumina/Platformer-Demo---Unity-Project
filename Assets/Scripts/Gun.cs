using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Gun : MonoBehaviour
{
    [Header("Configurable Parameters")]

    [SerializeField] GameObject bullet;
    [FormerlySerializedAs("projectileScript")]
    [SerializeField] bulletScript bulletScript;
    [SerializeField] PlayerMovement playerMovement;

    void Start()
    {
    }

    void Update()
    {
        Shoot(bulletType: bullet, bulletSpeed: bulletScript.bulletSpeed, lifeTime: bulletScript.bulletLifeTime);
    }

    public void Shoot(GameObject bulletType, float bulletSpeed, float lifeTime)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Instantiate(bulletType, new Vector3(transform.position.x + 1, transform.position.y), transform.rotation);
            bulletType.transform.Translate(transform.right * (bulletSpeed * Time.deltaTime));

            //Destroy(bulletType, lifeTime);
        }
    }
}