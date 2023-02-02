#region namespaces
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Debug;
using static UnityEngine.Input;
using static UnityEngine.Screen;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
#endregion

/// <summary>
///     TODO: Add a description.
///     TODO: bullets should be pooled.
///     TODO: bullets and pellets rotate towards mouse position, but don't move towards it.
///         Reminder: Game is based on time. Being able to use the shotgun knockback as a movement tool is very strong.
/// </summary>
public class Gun : MonoBehaviour
{
    public enum FireMode
    {
        Semi,
        Shotgun,
    }

    [Header("Serialized References"), Tooltip("These are the serialized references for the texts on screen.")]
    [SerializeField] TextMeshProUGUI fireModeText;    // The text that displays the current fire mode.
    [SerializeField] TextMeshProUGUI shootDelayText;  // Displays when the player can shoot again.

    [Header("Object Serialization"), Tooltip("These are the serialized objects for the pellet and position of the barrel exit.)")]
    [SerializeField] GameObject pellet;     // The pellet prefab.
    [SerializeField] Transform barrelExit; // The position of the barrel exit.

    [Header("Bullet Parameters"), Tooltip("These are the parameters for the bullet.")]
    [SerializeField] GameObject bullet;       // The bullet prefab.
    [SerializeField] float bulletFireVel;    // How fast the bullet is fired.
    [SerializeField] float lifeTime;         // [unused] How long the bullet lasts before it is destroyed.
    [SerializeField] float semiDelay = 0.2f; // How long it takes to shoot the SEMI again.

    [Header("Shotgun Parameters"), Tooltip("These are the parameters for the shotgun.")]
    [SerializeField] int pelletCount;               // How many pellets are fired.
    [SerializeField] [Range(0, 8)] int minPellets;  // The minimum amount of pellets that can be fired.
    [SerializeField] [Range(0, 8)] int maxPellets;  // The maximum amount of pellets that can be fired.
    [SerializeField] float spread;                  // How wide the spread is.
    [SerializeField] float pelletFireVel;          // How fast the pellet is fired.
    [SerializeField] float shotgunDelay;           // How long it takes to shoot the SHOTGUN again.

    [Header("Knockback Related Parameters"), Tooltip("These are the parameters for knockback.")]
    [SerializeField] float knockbackForce; // How much knockback the pellet does.
    [SerializeField] bool dealKnockback;   // [Serialized for debugging purposes] Whether or not the shotgun deals knockback.
    [SerializeField] [Range(0, 50)] float movingKbOffset;   // How much knockback the pellet does when the player is moving.
    [SerializeField] [Range(0, 50)] float groundedKbOffset; // How much knockback the pellet does when the player is grounded.
    [SerializeField] [Range(0, 50)] float airborneKbOffset; // How much knockback the pellet does when the player is airborne.

    [Header("Debugging"), Tooltip("This is only for debugging, do not touch.")]
    [SerializeField] bool debug;

    GameObject iBullet;    // The instantiated bullet.
    Rigidbody2D iBulletRb; // The instantiated bullet's Rigidbody2D.
    GameObject iPellet;    // The instantiated pellet.
    Rigidbody2D iPelletRb; // The instantiated pellet's Rigidbody2D.

    [Header("Private Parameters"), Space(25), Tooltip("These are the private parameters for the bullet and pellet.")]
    List<Quaternion> pellets; // The list of pellets.

    [Header("Cached References")]
    Rigidbody2D playerRb;   // The player's Rigidbody2D.
    PlayerMovement player;  // The player's movement script. (Main player script)
    Gauge gauge;            // The gauge that is used to determine when the player can shoot again.

    float shootElapsedTime; // The elapsed time since the last shot.
    Vector3 mousePosition;  // The position of the mouse.
    int height;             // The height of the screen.
    int halfHeight;         // The upper half of the screen.

    public FireMode CurrentFireMode { get; set; }

    /// <summary>
    ///     Awake handles the initialization of the list of pellets and the reference to the player's rigidbody.
    /// </summary>
    void Awake()
    {
        // Reference to the player's rigidbody.
        playerRb = GetComponentInParent<Rigidbody2D>();
        player   = GetComponentInParent<PlayerMovement>();
        gauge    = FindObjectOfType<Gauge>();

        pellets = new List<Quaternion>(pelletCount);
        mousePosition = Input.mousePosition;
        height        = Screen.height;
        halfHeight    = height / 2;

        // Initialize the list of pellets.
        for (int i = 0; i < pelletCount; i++) { pellets.Add(Quaternion.Euler(Vector3.zero)); }
    }

    /// <summary>
    ///     Update handles the shooting of the gun, switching the fire mode, and updating the text on screen that displays said
    ///     text as well as the elapsed time before the next shot.
    /// </summary>
    void Update()
    {
        TEMP_gauge(); //TODO: FOR DEBUGGING, REMOVE LATER

        // Update the elapsed time each frame to determine when to shoot again.
        shootElapsedTime += Time.deltaTime;

        // Displays the elapsed time. //TODO: Make this look better in game. i.e, make it countdown towards the next shot.
        shootDelayText.text = $"Shoot Delay: {-shootElapsedTime:F2}";
        if (shootElapsedTime >= semiDelay) shootDelayText.text = "Shoot Delay: 0";
        fireModeText.text = $"Fire Mode: {CurrentFireMode}"; // Displays the current fire mode.

        if (GetMouseButtonDown(0))
        {
            // If the left mouse button is pressed and the elapsed time is greater than the shoot delay, shoot.
            if (shootElapsedTime >= semiDelay)
            {
                shootElapsedTime = 0;
                Shoot(CurrentFireMode);
                //TODO: check the player's facing direction and decide the FireDirection from that.
            }
        }

        // If the right mouse button is pressed and the elapsed time is greater than the shoot delay, change the fire mode.
        if (GetMouseButtonDown(1) && shootElapsedTime >= semiDelay)
        {
            shootElapsedTime = 0;
            CurrentFireMode  = CurrentFireMode == FireMode.Semi ? FireMode.Shotgun : FireMode.Semi;
            // Play sound effect.
            Log($"Current Fire mode changed to {CurrentFireMode}");
        }
    }

    /// <summary>
    ///     Method that controls shooting. Includes one parameter which determines in what way to fire the gun.
    /// </summary>
    /// <param name="fireMode">The shooting mode to use. Either Semi or Shotgun. </param>
    void Shoot(Enum fireMode) //TODO: Optimize to be an object pool.
    {
        switch (fireMode)
        {
            case FireMode.Semi:
                // Reset the elapsed time, reset the knockback, and reset the rotation of the gun.
                dealKnockback = false;
                gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);

                // Instantiate the bullet and save its rigidbody as a reference.
                iBullet   = Instantiate(bullet, barrelExit.position, barrelExit.rotation);
                iBulletRb = iBullet.GetComponent<Rigidbody2D>();
                iBulletRb.AddForce(iBullet.transform.right * bulletFireVel); //TODO: Optimize

                break;

            case FireMode.Shotgun when gauge.CurrentGauge >= 10:
                for (int i = 0; i < pelletCount; i++)
                {
                    // Reset the elapsed time, instantiate the pellet and save its rigidbody as a reference.
                    // Then apply the spread angle to the gun gameobject.
                    shootElapsedTime = -shotgunDelay;
                    gameObject.transform.localEulerAngles = new Vector3(0, 0, 7.5f);

                    // Instantiate the pellet and save its rigidbody as a reference.
                    // Then apply a random rotation to the pellet.
                    iPellet = Instantiate(pellet, barrelExit.position, barrelExit.rotation);
                    iPellet.transform.localRotation =
                        Quaternion.Euler(0, 0, iPellet.transform.eulerAngles.z + Random.Range(-spread, spread));
                    iPelletRb = iPellet.GetComponent<Rigidbody2D>();
                    FireDirection();
                }

                switch (debug)
                {
                    case true:
                        gauge.CurrentGauge += 100;
                        dealKnockback      =  true;
                        ApplyKnockback(applyDebugging: debug);
                        break;

                    default:
                        gauge.CurrentGauge -= 10;
                        dealKnockback      =  true;
                        ApplyKnockback(false);
                        break;
                }

                break;

            default:
                Log("Not enough gauge to fire shotgun.");
                break;
        }
    }

    void FireDirection()
    {
        if (player.IsFacingRight) iPelletRb.AddForce(iPellet.transform.right * pelletFireVel);
        else iPelletRb.AddForce(-iPellet.transform.right * pelletFireVel);

        // if (mousePosition.y > halfHeight) iPelletRb.AddForce(iPellet.transform.up * pelletFireVel);
        // else iPelletRb.AddForce(-iPellet.transform.up * pelletFireVel);
    }
    
    /// <summary>
    ///     Method that releases the player's constraints and resets the player's rotation.
    /// </summary>
    void ReleaseConstraints()
    {
        playerRb.constraints = RigidbodyConstraints2D.None;
        playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerRb.rotation    = 0;
    }

    /// <summary>
    ///     Method that applies knockback to the player.
    /// </summary>
    /// <param name="applyDebugging">Determines whether or not to debug the values of ApplyKnockback() to the console.</param>
    void ApplyKnockback(bool applyDebugging)
    {
        switch (dealKnockback)
        {
            case true when playerRb.velocity.x > 1:
                playerRb.AddForce(-transform.right * knockbackForce / movingKbOffset, ForceMode2D.Impulse);
                ReleaseConstraints();
                if (applyDebugging) LogWarning("Player is moving");
                break;

            case true when playerRb.velocity.y == 0:
                playerRb.AddForce(-transform.right * knockbackForce / groundedKbOffset, ForceMode2D.Impulse);
                ReleaseConstraints();
                if (applyDebugging) LogWarning("Player is grounded");
                break;

            case true when playerRb.velocity.y > 0:
                playerRb.AddForce(-transform.right * knockbackForce / airborneKbOffset, ForceMode2D.Impulse);
                ReleaseConstraints();
                if (applyDebugging) LogWarning("Player is airborne");
                break;

            case false and false: // Alternatively: case false when dealKnockback == false:
                LogError("(DEBUG) \n Knockback is disabled.");
                break;
        }
    }

    void TEMP_gauge()
    {
        if (debug) gauge.CurrentGauge = 100;
    }

    void DestroyAllBullets()
    {
        //TODO: Destroy all objects in the objectpool (once there is one).
    }

    /// <summary>
    /// Draws a red line in the scene view to visualize the spread of the shotgun.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(barrelExit.position, Quaternion.Euler(0, 0, spread) * barrelExit.right * 2);
        Gizmos.DrawRay(barrelExit.position, Quaternion.Euler(0, 0, -spread) * barrelExit.right * 2);
    }
}