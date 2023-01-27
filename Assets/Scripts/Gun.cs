using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using static UnityEngine.Debug;
using Random = UnityEngine.Random;

/// <summary>
/// TODO: Add a description.
///     TODO: bullets still only move to the right.
/// TODO: make the gun aimable.
/// Reminder: Game is based on time. Being able to use the shotgun knockback as a movement tool is very strong. Possible use for the gauge here. (i.e, shotgun can only be used when gauge is filled)
/// </summary>
public class Gun : MonoBehaviour
{
    public enum FireMode
    {
        Semi,
        Shotgun
    }

    public FireMode CurrentFireMode { get; set; }

    [Header("Serialized References"), Tooltip("These are the serialized references for the texts on screen.")]
    [SerializeField] BulletScript bulletScript; // [unused] The bullet script.
    [SerializeField]
    GameObject collection; // [unused] The collection(gameobject) of which bullets and pellets should be childed to.
    [SerializeField] TextMeshProUGUI fireModeText; // [unused] The text that displays the current fire mode.
    [SerializeField] TextMeshProUGUI shootDelayText; // Displays when the player can shoot again.

    [Header("Object Serialization"),
     Tooltip("These are the serialized objects for the pellet and position of the barrel exit.)")]
    [SerializeField] GameObject pellet;    // The pellet prefab.
    [SerializeField] Transform barrelExit; // The position of the barrel exit.

    [Header("Bullet Parameters"), Tooltip("These are the parameters for the bullet.")]
    [SerializeField] GameObject bullet;      // The bullet prefab.
    [SerializeField] float bulletFireVel;    // How fast the bullet is fired.
    [SerializeField] float lifeTime;         // [unused] How long the bullet lasts before it is destroyed.
    [SerializeField] float semiDelay = 0.2f; // How long it takes to shoot the SEMI again.

    [Header("Shotgun Parameters"), Tooltip("These are the parameters for the shotgun.")]
    [SerializeField] int pelletCount;             // How many pellets are fired.
    [SerializeField, Range(0, 8)] int minPellets; // The minimum amount of pellets that can be fired.
    [SerializeField, Range(0, 8)] int maxPellets; // The maximum amount of pellets that can be fired.
    [SerializeField] float spreadAngle;           // How wide the spread is.
    [SerializeField] float pelletFireVel;         // How fast the pellet is fired.
    [SerializeField] float shotgunDelay;          // How long it takes to shoot the SHOTGUN again.

    [Header("Knockback Related Parameters"), Tooltip("These are the parameters for knockback.")]
    [SerializeField] float knockbackForce; // How much knockback the pellet does.
    [SerializeField]
    bool dealKnockback; // [Serialized for debugging purposes] Whether or not the shotgun deals knockback.
    [SerializeField, Range(0, 50)]
    float movingKbOffset; // How much knockback the pellet does when the player is moving.
    [SerializeField, Range(0, 50)]
    float groundedKbOffset; // How much knockback the pellet does when the player is grounded.
    [SerializeField, Range(0, 50)]
    float airborneKbOffset; // How much knockback the pellet does when the player is airborne.

    [Header("Private Parameters"), Space(25), Tooltip("These are the private parameters for the bullet and pellet.")]
    List<Quaternion> pellets; // The list of pellets.
    //List<GameObject> bulletList; // [unused] Special list for modifying position and name in hierarchy.
    //List<GameObject> pelletList; // [unused] Special list for modifying position and name in hierarchy.
    float shootElapsedTime; // The elapsed time since the last shot.

    GameObject iBullet; // The instantiated bullet.
    GameObject iPellet; // The instantiated pellet.

    [Header("Cached References")]
    Rigidbody2D player;    // The player's Rigidbody2D.
    Rigidbody2D iBulletRb; // The instantiated bullet's Rigidbody2D.
    Rigidbody2D iPelletRb; // The instantiated pellet's Rigidbody2D.

    [Header("Debugging"), Tooltip("This is only for debugging, do not touch.")]
    [SerializeField] bool debug;

    /// <summary>
    /// Awake handles the initialization of the list of pellets and the reference to the player's rigidbody.
    /// </summary>
    void Awake()
    {
        // Reference to the player's rigidbody.
        player  = GetComponent<Rigidbody2D>();
        pellets = new List<Quaternion>(pelletCount);

        // Initialize the list of pellets.
        for (int i = 0; i < pelletCount; i++) { pellets.Add(Quaternion.Euler(Vector3.zero)); }
    }

    /// <summary>
    /// Update handles the shooting of the gun, switching the fire mode, and updating the text on screen that displays said text as well as the elapsed time before the next shot.
    /// </summary>
    void Update()
    {
        // Update the elapsed time each frame to determine when to shoot again.
        shootElapsedTime += Time.deltaTime;

        shootDelayText.text =
            $"Shoot Delay: {-shootElapsedTime:F2}"; // Displays the elapsed time. //TODO: Make this look better in game. i.e, make it countdown towards the next shot.

        if (shootElapsedTime >= semiDelay) shootDelayText.text = "Shoot Delay: 0";

        fireModeText.text = $"Fire Mode: {CurrentFireMode}"; // Displays the current fire mode.

        // If the left mouse button is pressed and the elapsed time is greater than the shoot delay, shoot the gun.
        if (Input.GetKeyDown(KeyCode.Mouse0) && shootElapsedTime >= semiDelay)
        {
            shootElapsedTime = 0;
            pelletCount      = Random.Range(minPellets, maxPellets);
            Shoot(CurrentFireMode);
        }

        // If the right mouse button is pressed and the elapsed time is greater than the shoot delay, change the fire mode.
        if (Input.GetKeyDown(KeyCode.Mouse1) && shootElapsedTime >= semiDelay)
        {
            shootElapsedTime = 0;
            CurrentFireMode  = CurrentFireMode == FireMode.Semi ? FireMode.Shotgun : FireMode.Semi;
            // Play sound effect.
            Log($"Current Fire mode changed to {CurrentFireMode}");
        }
    }

    /// <summary>
    /// Method that controls shooting. Includes one parameter which determines in what way to fire the gun.
    /// </summary>
    /// <param name="fireMode">The shooting mode to use. Either Semi or Shotgun. </param>
    void Shoot(Enum fireMode) //TODO: Optimize to be an object pool.
    {
        switch (fireMode)
        {
            case FireMode.Semi:
                dealKnockback = false;

                // Instantiate the bullet.
                iBullet   = Instantiate(bullet, barrelExit.position, barrelExit.rotation);
                iBulletRb = iBullet.GetComponent<Rigidbody2D>();

                #region [archived/unused] Hierarchy organization
                // bulletList.Add(iBullet);
                // iBullet.name = $"Bullet {bulletList.Count}";
                // iBullet.transform.SetParent(collection.transform.GetChild(0).transform);
                #endregion

                iBulletRb.AddForce(iBullet.transform.right * bulletFireVel); //TODO: Optimize

                break;

            case FireMode.Shotgun: //TODO: Rework the spread function.
                for (int i = 0; i < pelletCount; i++)
                {
                    // Reset the elapsed time, instantiate the pellet, and add force to the pellet.
                    shootElapsedTime = -shotgunDelay;
                    pellets[i]       = Random.rotation;
                    iPellet          = Instantiate(pellet, barrelExit.position, barrelExit.rotation);
                    iPelletRb        = iPellet.GetComponent<Rigidbody2D>();

                    #region [archived/unused] Hierarchy organization
                    // pelletList.Add(iPellet);
                    // iPellet.name = $"Pellet {pelletList.Count}";
                    // iPellet.transform.SetParent(collection.transform.GetChild(1).transform); // Child number 1 = Pellet list.
                    #endregion

                    iPellet.transform.rotation =
                        Quaternion.RotateTowards(iPellet.transform.rotation, pellets[i], spreadAngle);

                    iPelletRb.AddForce(iPellet.transform.right * pelletFireVel); // TODO: Optimize

                    i++;
                    dealKnockback = true;
                }

                ApplyKnockback(debug);

                break;
        }
    }

    /// <summary>
    /// Method that releases the player's constraints and resets the player's rotation.
    /// </summary>
    void ReleaseConstraints()
    {
        player.constraints = RigidbodyConstraints2D.None;
        player.constraints = RigidbodyConstraints2D.FreezeRotation;
        player.rotation    = 0;
    }

    /// <summary>
    /// Method that applies knockback to the player.
    /// </summary>
    /// <param name="applyDebugging">Determines whether or not to debug the values of ApplyKnockback() to the console.</param>
    void ApplyKnockback(bool applyDebugging)
    {
        switch (dealKnockback)
        {
            case true when player.velocity.x > 1:
                player.AddForce(-transform.right * knockbackForce / movingKbOffset, ForceMode2D.Impulse);
                ReleaseConstraints();
                if (applyDebugging) LogWarning("Player is moving");
                break;

            case true when player.velocity.y == 0:
                player.AddForce(-transform.right * knockbackForce / groundedKbOffset, ForceMode2D.Impulse);
                ReleaseConstraints();
                if (applyDebugging) LogWarning("Player is grounded");
                break;

            case true when player.velocity.y > 0:
                player.AddForce(-transform.right * knockbackForce / airborneKbOffset, ForceMode2D.Impulse);
                ReleaseConstraints();
                if (applyDebugging) LogWarning("Player is airborne");
                break;

            case false and false: // Alternatively: case false when dealKnockback == false:
                LogError("(DEBUG) \n Knockback is disabled.");
                break;
        }
    }
}