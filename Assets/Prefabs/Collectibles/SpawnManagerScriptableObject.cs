using UnityEngine;

[CreateAssetMenu(fileName = "Collectible", menuName = "ScriptableObjects/Collectible", order = 1)]
public class SpawnManagerScriptableObject : ScriptableObject
{
    public string prefabName;

    public int numberOfPrefabsToCreate;
    public Vector3[] spawnPoints;
}