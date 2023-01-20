using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    public enum FireMode
    {
        Semi,
        Shotgun
    }
    FireMode currentFireMode;

    public FireMode CurrentFireMode
    {
        get => currentFireMode;
        set => currentFireMode = value;
    }

    [Header("Configurable Parameters")]
    [SerializeField] GameObject bullet;
    [SerializeField] BulletScript bulletScript;
    [SerializeField] float lifeTime;

    [SerializeField] int pelletCount;
    [SerializeField] float spreadAngle;
    [SerializeField] float pelletFireVel;
    [SerializeField] GameObject pellet;
    [SerializeField] Transform barrelExit;
    List<Quaternion> pellets;

    float shootElapsedTime = 0;
    [SerializeField] float shootDelay = 0.2f;

    [SerializeField] bool isSemi_Automatic;
    [SerializeField] bool isShotgun;

    void Awake()
    {
        pellets = new List<Quaternion>(pelletCount);

        for (int i = 0; i < pelletCount; i++) { pellets.Add(Quaternion.Euler(Vector3.zero)); }
    }

    void Update()
    {
        shootElapsedTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Mouse0) && shootElapsedTime >= shootDelay)
        {
            shootElapsedTime = 0;
            Shoot(currentFireMode);
        }

        Debug.Log("Current Fire Mode: " + currentFireMode);

        
    }

    void Shoot(Enum fireMode)
    {
        switch (fireMode)
        {
            case FireMode.Semi:
                GameObject b = Instantiate(bullet, barrelExit.position, barrelExit.rotation);
                b.GetComponent<Rigidbody2D>().AddForce(b.transform.forward * bulletScript.bulletSpeed);
                Destroy(b, lifeTime);

                break;

            case FireMode.Shotgun:
                for (int i = 0; i < pelletCount; i++)
                {
                    pellets[i] = Random.rotation;
                    GameObject p = Instantiate(pellet, barrelExit.position, barrelExit.rotation);
                    p.transform.rotation = Quaternion.RotateTowards(p.transform.rotation, pellets[i], spreadAngle);
                    p.GetComponent<Rigidbody2D>().AddForce(p.transform.forward * pelletFireVel);
                    i++;
                }

                break;
        }
    }
}