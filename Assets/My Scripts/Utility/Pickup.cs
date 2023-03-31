#region
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;
#endregion

/// <summary>
/// Pickup that the player can collect to gain a boost, time, or points.
/// </summary>

[RequireComponent(typeof(Collider2D))]
public class Pickup : MonoBehaviour
{
    [Header("Pickup Values")]
    [SerializeField] bool addBoost;
    [SerializeField] float boostForce;
    [Space(5)]
    [SerializeField] bool addTime;
    [SerializeField] float addedTime;
    [Space(5)]
    [SerializeField] bool addPoints;

    [Header("Cached References")]
    CountdownTimer timer;
    Rigidbody2D playerRB;

    void Start()
    {
        timer    = FindObjectOfType<CountdownTimer>();
        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        // Set the pickup's collider to a trigger so that the player can walk over it.
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        switch (addTime || addBoost || addPoints)
        {
            case true when addTime:
                timer.AddTime(addedTime);
                Destroy(gameObject);
                break;

            //TODO: if the player falls onto the pickup, they don't get boosted as high as they should. This is because of the adjustable jump height.
            case true when addBoost:
                playerRB.velocity = Vector2.zero;
                playerRB.AddForce(Vector2.up * boostForce, ForceMode2D.Impulse);
                Destroy(gameObject);
                break;

            case true when addPoints:
                FindObjectOfType<Gauge>().CurrentGauge += 10;
                Destroy(gameObject);
                break;
        }
    }
}