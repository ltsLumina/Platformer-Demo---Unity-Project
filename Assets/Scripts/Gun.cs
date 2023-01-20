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

    [SerializeField] bool isSemi_Automatic;
    [SerializeField] bool isShotgun;

    void Awake()
    {
        pellets = new List<Quaternion>(pelletCount);

        for (int i = 0; i < pelletCount; i++) { pellets.Add(Quaternion.Euler(Vector3.zero)); }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) Shoot(currentFireMode);
    }

    #region MyRegion
    // void AlternateShoot()
    // {
    //     int i=0;
    //     foreach (Quaternion quat in pellets)
    //     {
    //         pellets[i] = Random.rotation;
    //         GameObject p = Instantiate(pellet, barrelExit.position, barrelExit.rotation);
    //         p.transform.rotation = Quaternion.RotateTowards(p.transform.rotation, pellets[i], spreadAngle);
    //         p.GetComponent<Rigidbody2D>().AddForce(p.transform.right * pelletFireVel);
    //         i++;
    //     }
    // }
    #endregion

    void Shoot(Enum fireMode)
    {
        switch (fireMode)
        {
            case FireMode.Semi:
                GameObject b = Instantiate(bullet, barrelExit.position, barrelExit.rotation);
                b.GetComponent<Rigidbody2D>().AddForce(b.transform.right * bulletScript.bulletSpeed);
                Destroy(b, lifeTime);
                break;

            case FireMode.Shotgun:
                int i = 0;

                foreach (Quaternion quat in pellets)
                {
                    pellets[i] = Random.rotation;
                    GameObject p = Instantiate(pellet, barrelExit.position, barrelExit.rotation);
                    p.transform.rotation = Quaternion.RotateTowards(p.transform.rotation, pellets[i], spreadAngle);
                    p.GetComponent<Rigidbody2D>().AddForce(p.transform.right * pelletFireVel);
                    i++;
                }

                break;
        }
    }
}