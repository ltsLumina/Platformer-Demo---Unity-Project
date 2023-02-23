    #region namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static UnityEngine.Debug;
using static UnityEngine.Input;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
#endregion

    /// <summary>
    ///     TODO: Add a description.
    ///     TODO: bullets should be pooled.
    ///      Reminder: Game is based on time. Being able to use the shotgun knockback as a movement tool is very strong.
    /// </summary>
    public class Gun : MonoBehaviour
    {
        public enum FireMode
        {
            Semi,
            Shotgun,
        }

        #region Serialized Variables
        [Header("Serialized References"), Tooltip("These are the serialized references for the texts on screen.")]
        [SerializeField]
        TextMeshProUGUI fireModeText;                    // The text that displays the current fire mode.
        [SerializeField] TextMeshProUGUI shootDelayText; // Displays when the player can shoot again.

        [Header("Object Serialization"),
         Tooltip("These are the serialized objects for the pellet and position of the barrel exit.)")]
        [SerializeField]
        GameObject pellet;                     // The pellet prefab.
        [SerializeField] Transform barrelExit; // The position of the barrel exit.

        [Header("Bullet Parameters"), Tooltip("These are the parameters for the bullet.")] [SerializeField]
        GameObject bullet;                        // The bullet prefab.
        [SerializeField] float bulletSpread;      // How wide the spread is for the bullet.
        [SerializeField] float bulletFireVel;     // How fast the bullet is fired.
        [SerializeField] float lifeTime;          // [unused] How long the bullet lasts before it is destroyed.
        [SerializeField] float shootDelay = 0.2f; // How long it takes to shoot the SEMI again.

        [Header("Shotgun Parameters"), Tooltip("These are the parameters for the shotgun.")] [SerializeField]
        int pelletCount;                      // How many pellets are fired.
        [SerializeField] float pelletSpread;  // How wide the spread is for the shotgun.
        [SerializeField] float pelletFireVel; // How fast the pellet is fired.
        [SerializeField] float shotgunDelay;  // How long it takes to shoot the SHOTGUN again.

        [Header("Knockback Related Parameters"), Tooltip("These are the parameters for knockback.")] [SerializeField]
        float knockbackForce; // How much knockback the pellet does.
        [SerializeField]
        bool dealKnockback; // [Serialized for debugging purposes] Whether or not the shotgun deals knockback.
        [SerializeField] float knockbackDuration;
        [SerializeField] [Range(0, 50)]
        float movingKbOffset; // How much knockback the pellet does when the player is moving.
        [SerializeField] [Range(0, 50)]
        float stationaryKbOffset; // How much knockback the pellet does when the player is stationary.
        [SerializeField] [Range(0, 50)]
        float airborneKbOffset; // How much knockback the pellet does when the player is airborne.

        [Header("Debugging"), Tooltip("This is only for debugging, do not touch.")] [SerializeField] bool debug;
        [SerializeField]
        float shootElapsedTime; // [serialized for debugging purposes] The elapsed time since the last shot.

        GameObject iBullet;    // The instantiated bullet.
        Rigidbody2D iBulletRb; // The instantiated bullet's Rigidbody2D.
        GameObject iPellet;    // The instantiated pellet.
        Rigidbody2D iPelletRb; // The instantiated pellet's Rigidbody2D.

        [Header("Private Parameters"), Space(25),
         Tooltip("These are the private parameters for the bullet and pellet.")]
        List<Quaternion> pellets; // The list of pellets.

        [Header("Cached References")] Rigidbody2D playerRb; // The player's Rigidbody2D.
        PlayerMovement player; // The player's movement script. (Main player script)
        Gauge gauge; // The gauge that is used to determine when the player can shoot again.
        #endregion

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

            pellets = new (pelletCount);

            // Initialize the list of pellets.
            for (int i = 0; i < pelletCount; i++) { pellets.Add(Quaternion.Euler(Vector3.zero)); }
        }

        /// <summary>
        ///     Update handles the shooting of the gun, switching the fire mode, and updating the text on screen that displays said
        ///     text as well as the elapsed time before the next shot.
        /// </summary>
        void Update()
        {
            TEMP_inf_Gauge(); //TODO: DEBUGGING, REMOVE LATER

            // Update the elapsed time each frame to determine when to shoot again.
            shootElapsedTime += Time.deltaTime;

            // Displays the elapsed time. //TODO: Make this look better in game. i.e, make it countdown towards the next shot.
            shootDelayText.text = $"Shoot Delay: {-shootElapsedTime:F2}";
            if (shootElapsedTime >= shootDelay) shootDelayText.text = "Shoot Delay: 0";
            fireModeText.text = $"Fire Mode: {CurrentFireMode}"; // Displays the current fire mode.

            // If the arrow keys are pressed and the elapsed time is greater than the shoot delay, shoot.
            if (GetKeyDown(KeyCode.RightArrow) && shootElapsedTime >= shootDelay)
            {
                Shoot(CurrentFireMode, Vector2.right);
                //if (debug) Log("Right Arrow");
            }

            if (GetKeyDown(KeyCode.LeftArrow) && shootElapsedTime >= shootDelay)
            {
                Shoot(CurrentFireMode, Vector2.left);
                //if (debug) Log("Left Arrow");
            }

            {
                if (GetKeyDown(KeyCode.Mouse0) && shootElapsedTime >= shootDelay)
                {
                    Shoot(CurrentFireMode, Vector2.left);
                    //if (debug) Log("Left Mouse");
                }
            }

            if (GetKeyDown(KeyCode.F) && shootElapsedTime >= shootDelay)
            {
                shootElapsedTime = 0;
                CurrentFireMode  = CurrentFireMode == FireMode.Semi ? FireMode.Shotgun : FireMode.Semi;
                // Play sound effect.
                Log($"Current Fire mode changed to {CurrentFireMode}");
            }
        }

        /// <summary>
        ///     Method that controls shooting. Includes one parameter which determines which way to fire the gun.
        /// </summary>
        /// <param name="fireMode">The shooting mode to use. Either Semi or Shotgun. </param>
        /// <param name="fireDir">The direction the player is shooting towards. Determined by either the left or right arrow keys.</param>
        void Shoot(Enum fireMode, Vector2 fireDir) //TODO: Optimize to be an object pool.
        {
            switch (fireMode)
            {
                case FireMode.Semi:
                    // Reset the elapsed time, reset the knockback, and reset the rotation of the gun.
                    dealKnockback                         = false;
                    gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);

                    // Instantiate the bullet and save its rigidbody as a reference.
                    iBullet = Instantiate(bullet, barrelExit.position, barrelExit.rotation);

                    iBullet.transform.localRotation =
                        Quaternion.Euler(
                            0, 0, iBullet.transform.eulerAngles.z + Random.Range(-bulletSpread, bulletSpread));

                    iBulletRb = iBullet.GetComponent<Rigidbody2D>();
                    ApplyProjForce(CurrentFireMode, fireDir);

                    break;

                case FireMode.Shotgun when gauge.CurrentGauge >= 10:
                    for (int i = 0; i < pelletCount; i++)
                    {
                        // Reset the elapsed time, instantiate the pellet and save its rigidbody as a reference.
                        // Then apply the spread angle to the gun gameobject.
                        shootElapsedTime                      = -shotgunDelay;
                        gameObject.transform.localEulerAngles = new Vector3(0, 0, 7.5f);

                        // Instantiate the pellet and save its rigidbody as a reference.
                        // Then apply a random rotation to the pellet.
                        iPellet = Instantiate(pellet, barrelExit.position, barrelExit.rotation);

                        iPellet.transform.localRotation =
                            Quaternion.Euler(
                                0, 0, iPellet.transform.eulerAngles.z + Random.Range(-pelletSpread, pelletSpread));

                        iPelletRb = iPellet.GetComponent<Rigidbody2D>();
                        ApplyProjForce(CurrentFireMode, fireDir);
                    }

                    switch (debug)
                    {
                        case true:
                            gauge.CurrentGauge += 100;
                            dealKnockback      =  true;
                            ApplyKnockback(fireDir);
                            break;

                        default:
                            gauge.CurrentGauge -= 10;
                            dealKnockback      =  true;
                            ApplyKnockback(fireDir);
                            break;
                    }

                    break;

                default:
                    Log("Not enough gauge to fire shotgun.");
                    break;
            }
        }

        /// <summary>
        /// Decides which direction to fire the bullet in.
        /// </summary>
        /// <param name="fireMode">The shooting mode to use. Either Semi or Shotgun. </param>
        /// <param name="fireDir">The direction the player is shooting towards. Determined by either the left or right arrow keys.</param>
        void ApplyProjForce(FireMode fireMode, Vector2 fireDir)
        {
            switch (fireMode)
            {
                case FireMode.Semi:
                    if (fireDir == Vector2.right)
                    {
                        iBulletRb.AddForce(iBullet.transform.right * bulletFireVel);
                        player.Flip(Vector2.right);
                    }
                    else if (fireDir == Vector2.left)
                    {
                        iBulletRb.AddForce(-iBullet.transform.right * bulletFireVel);
                        player.Flip(Vector2.left);
                    }

                    break;

                case FireMode.Shotgun:
                    if (fireDir == Vector2.right)
                    {
                        iPelletRb.AddForce(iPellet.transform.right * pelletFireVel);
                        player.Flip(Vector2.right);
                    }
                    else if (fireDir == Vector2.left)
                    {
                        iPelletRb.AddForce(-iPelletRb.transform.right * pelletFireVel);
                        player.Flip(Vector2.left);
                    }

                    break;

                default: // uhh idk why this is here but it's here. (copilot)
                    throw new ArgumentOutOfRangeException(nameof(fireMode), fireMode, null);
            }
        }

        /// <summary>
        ///     Method that applies knockback to the player.
        ///     IMPORTANT: Apply force in the opposite direction of the Vector2.(right/left)
        /// </summary>
        /// <param name="fireDir">The direction the player is shooting towards. Determined by either the left or right arrow keys.</param>
        /// <param name="applyDebugging">Determines whether or not to debug the values of ApplyKnockback() to the console.</param>
        void ApplyKnockback(Vector2 fireDir)
        {
            switch (dealKnockback)
            {
                // [old] retired code that I might use later.
                // case true when playerRb.velocity.x == 0 && playerRb.velocity.y == 0:
                //     if (fireDir      == Vector2.right) ApplyKnockbackForce(-transform.right, stationaryKbOffset);
                //     else if (fireDir == Vector2.left) ApplyKnockbackForce(transform.right, stationaryKbOffset);
                //
                //     if (applyDebugging) LogWarning("Player is stationary");
                //     break;

                case true when playerRb.velocity is not { y: 0 }:
                    if (fireDir == Vector2.right)
                        StartCoroutine(ApplyKnockbackForce(-transform.right, airborneKbOffset));
                    else if (fireDir == Vector2.left)
                        StartCoroutine(ApplyKnockbackForce(transform.right, airborneKbOffset));

                    if (debug) LogWarning("Player is airborne");
                    break;

                case true when playerRb.velocity.x is > 0 or < 0: //TODO: doesnt work
                    if (fireDir      == Vector2.right) ApplyKnockbackForce(-transform.right, movingKbOffset);
                    else if (fireDir == Vector2.left) ApplyKnockbackForce(transform.right, movingKbOffset);

                    if (debug) LogWarning("Player is moving on the ground");
                    break;

                case true when playerRb.velocity is { x: 0, y: 0 }: //TODO: doesnt work
                    if (fireDir      == Vector2.right) ApplyKnockbackForce(-transform.right, stationaryKbOffset);
                    else if (fireDir == Vector2.left) ApplyKnockbackForce(transform.right, stationaryKbOffset);

                    if (debug) LogWarning("Player is stationary");
                    break;

                case false and false: // Alternatively: case false when dealKnockback == false:
                    LogError("(DEBUG) \n Knockback is disabled.");
                    break;
            }
        }

        /// <summary>
        /// Applies force to the player in the opposite direction that they are facing when used in conjunction with ApplyKnockback().
        /// </summary>
        /// <param name="knockbackDir"> transform.right or negative transform.right (right/left) </param>
        /// <param name="knockbackOffset"> The variable that decides what knockbackOffset to use. </param>
        IEnumerator ApplyKnockbackForce(Vector2 knockbackDir, float knockbackOffset)
        {
            playerRb.constraints = RigidbodyConstraints2D.FreezePositionY;
            playerRb.AddForce(knockbackDir * knockbackForce / knockbackOffset, ForceMode2D.Impulse);
            playerRb.velocity = new Vector2(playerRb.velocity.x, 0);

            yield return new WaitForSeconds(knockbackDuration);
            ReleaseConstraints();
        }

        /// <summary>
        ///     Method that releases the player's constraints and resets the player's rotation.
        /// </summary>
        public void ReleaseConstraints()
        {
            playerRb.constraints = RigidbodyConstraints2D.None;
            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            playerRb.rotation    = 0;
        }

        void TEMP_inf_Gauge()
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
            Gizmos.DrawRay(barrelExit.position, Quaternion.Euler(0, 0, pelletSpread)  * barrelExit.right * 2);
            Gizmos.DrawRay(barrelExit.position, Quaternion.Euler(0, 0, -pelletSpread) * barrelExit.right * 2);
        }
    }