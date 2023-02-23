using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEditor.EditorTools;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider2D))]
public class CollectibleManager : MonoBehaviour
{
    /// <summary>
    /// Drag and drop any GameObject that can be collected into the list in the inspector.
    /// (!!!) Make sure the GameObjects have the tag "Collectible" in the inspector.
    /// Then set what ammo value is added to the player's ammo count when that object is collected.
    /// </summary>
    [FormerlySerializedAs("ammoVales")]
    [Header("Ammo List and Adjustments"), Tooltip("The ammo types that can be collected"), Space(5)]
    [SerializeField] List<GameObject> ammoValues = new ();
    [Header("List of Collectible GameObjects"), Tooltip("The list of all collectible GameObjects in the scene."), Space(5)]
    [SerializeField] List<GameObject> collectibles = new ();
    [SerializeField] List<CollectibleObject> betterList = new ();

    [Header("Ammo Text and Count"), Tooltip("The text that displays the player's ammo count")]
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] int ammoStartCount;
    [SerializeField] int ammoCount; // Alternatively, use your own ammo count variable.

    [Header("Debugging"), Tooltip("Used for debugging in the editor")]
    [SerializeField] bool debug;

    void Start()
    {
        ammoCount = Mathf.Max(
            ammoCount, ammoStartCount); // Ensures that the ammo count is never less than the starting ammo count.

        if (ammoText != null) ammoText.text = $"Ammo: {ammoCount}";

        foreach (GameObject collectible in GameObject.FindGameObjectsWithTag("Collectible"))
        {
            collectibles.Add(collectible);
        }
    }

    void Update() { DEBUG_Tools(); }

    void DEBUG_Tools()
    {
        #region Debugging
        if (!debug) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // if the shift key is pressed, add 10 ammo instead.
            if (Input.GetKey(KeyCode.LeftShift)) ammoCount += 10;
            else ammoCount++;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // if the shift key is pressed, remove 10 ammo instead.
            if (Input.GetKey(KeyCode.LeftShift)) ammoCount -= 10;
            else ammoCount--;
        }
        #endregion
    }

    /// <summary>
    /// Depending on what object is collected, the player's ammo count will be adjusted.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        try
        {
            GameObject otherObject;

            switch ((otherObject = other.gameObject).CompareTag("Collectible") && collectibles.Contains(otherObject))
            {
                case true:
                    Debug.Log("Collectible Detected!");
                    AmmoAdjustment(other);
                    break;
            }
        } catch (ArgumentOutOfRangeException error)
        {
            Debug.LogWarning("Error Detected! Pausing the game.");
            Debug.LogError($"There are no GameObjects in the list. \n Error: {error.Message} ");
        }
    }

    /// <summary>
    /// Adjusts the player's ammo count.
    /// Updates the ammo text if it exists.
    /// Then finally, destroys the collected object.
    /// </summary>
    /// <param name="ammoToAdd"></param>
    /// <param name="other"></param>
    void AmmoAdjustment(Component other)
    {
        ammoCount += other.gameObject.GetComponent<Collectible>().ammoToAdd;
        if (ammoText != null) ammoText.text = $"Ammo: {ammoCount}";
        collectibles.Remove(other.gameObject);
        Destroy(other.gameObject);
    }
}

[Serializable]
public struct CollectibleObject
{
    public string name;
    public string description;
    public int ammoToAdd;
}