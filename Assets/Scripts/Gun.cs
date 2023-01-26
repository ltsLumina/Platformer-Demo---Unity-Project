using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using static UnityEngine.Debug;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    public enum FireMode
    {
        Semi,
        Shotgun
    }

    public FireMode CurrentFireMode { get; set; }

    [Header("Serialized References")]
    [SerializeField] BulletScript bulletScript;
    [SerializeField] GameObject collection;

    [Header("Bullet Parameters")]
    [SerializeField] GameObject bullet;
    [SerializeField] float lifeTime;

    [Header("Shotgun Parameters")]
    [SerializeField] int pelletCount;
    [SerializeField] float spreadAngle;
    [SerializeField] float pelletFireVel;
    [SerializeField] float shotgunDelay;

    [Header("Object Serialization")]
    [SerializeField] GameObject pellet;
    [SerializeField] Transform barrelExit;

    [SerializeField] float shootDelay = 0.2f;
    [SerializeField] float knockbackForce;

    [Header("Private Parameters")]
    List<Quaternion> pellets;
    [SerializeField] List<GameObject> bulletList;
    List<GameObject> pelletList;
    float shootElapsedTime;

    GameObject iBullet;
    GameObject iPellet;

    [Header("Cached References")]
    Rigidbody2D player;

    void Awake()
    {
        player = GetComponent<Rigidbody2D>();

        pellets = new List<Quaternion>(pelletCount);

        for (int i = 0; i < pelletCount; i++) { pellets.Add(Quaternion.Euler(Vector3.zero)); }
    }

    void Update()
    {
        shootElapsedTime += Time.deltaTime;

        if (!Input.GetKeyDown(KeyCode.Mouse0) || !(shootElapsedTime >= shootDelay)) return;
        shootElapsedTime = 0;
        Shoot(CurrentFireMode);

        if (!Input.GetKeyDown(KeyCode.Mouse1) || !(shootElapsedTime >= shootDelay)) return;
        //TODO: Change firemode with right click.
    }

    /// <summary>
    /// Method that controls shooting. Includes one parameter which determines in what way to fire the gun.
    /// </summary>
    /// <param name="fireMode">The shooting mode to use. Either Semi or Shotgun. </param>
    /// <param name="iBullet">The gameobject that becomes instantiated.</param>
    /// <param name="iPellet"> Same as above, but for the shotgun pellets. </param>
    void Shoot(Enum fireMode) //TODO: Optimize to be an object pool.
    {
        switch (fireMode)
        {
            case FireMode.Semi:
                iBullet = Instantiate(bullet, barrelExit.position, barrelExit.rotation);
                bulletList.Add(iBullet);
                iBullet.name = $"Bullet {bulletList.Count}";

                iBullet.transform.SetParent(collection.transform.GetChild(0).transform);
                iBullet.GetComponent<Rigidbody2D>().AddForce(iBullet.transform.forward * bulletScript.bulletSpeed); //TODO: Optimize

                break;

            case FireMode.Shotgun:
                for (int i = 0; i < pelletCount; i++)
                {
                    shootElapsedTime = -shotgunDelay;
                    pellets[i]       = Random.rotation;
                    iPellet = Instantiate(pellet, barrelExit.position, barrelExit.rotation);

                    pelletList.Add(iPellet);
                    iPellet.name = $"Pellet {pelletList.Count}";
                    iPellet.transform.SetParent(collection.transform.GetChild(1).transform); // Child number 1 = Pellet list.

                    iPellet.transform.rotation = Quaternion.RotateTowards(iPellet.transform.rotation, pellets[i], spreadAngle);
                    iPellet.GetComponent<Rigidbody2D>().AddForce(iPellet.transform.forward * pelletFireVel);

                    i++;
                }
                player.AddForce(-transform.right * knockbackForce, ForceMode2D.Impulse);

                break;
        }
        Destroy(iBullet, lifeTime);
        Destroy(iPellet, lifeTime);
    }
}