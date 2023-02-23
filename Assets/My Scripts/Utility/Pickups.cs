using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    [SerializeField] float addedTime = 102f;
    public float AddedTime { get; set; }

    CountdownTimer timer;

    void Start()
    {
        timer = FindObjectOfType<CountdownTimer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timer.AddTime(addedTime);
            Destroy(gameObject);
        }
    }
}