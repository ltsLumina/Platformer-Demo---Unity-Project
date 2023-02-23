using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Collectible : MonoBehaviour
{
    [Header("Name")]
    public string collectibleName;
    public string description;

    [Header("Values")]
    public int ammoToAdd;

    public CollectibleObject RetrieveAsStruct()
    {
        return new CollectibleObject
        {
            name = collectibleName,
            description = description,
            ammoToAdd = ammoToAdd
        };
    }
}