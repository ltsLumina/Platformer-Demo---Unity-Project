#region
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#endregion

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    List<ObjectPool> objectPools = new List<ObjectPool>();

    /// <summary>
    ///     Gets the pre-made object pools and adds them to the list of object pools.
    /// </summary>
    void Start()
    {
        List<ObjectPool> premadeObjectPools = FindObjectsOfType<ObjectPool>().ToList();
        foreach (ObjectPool objectPool in premadeObjectPools) { objectPools.Add(objectPool); }
    }

    /// <summary>
    ///     Creates a new object pool as a new gameobject.
    /// </summary>
    /// <param name="objectPrefab"></param>
    /// <param name="startAmount"></param>
    /// <returns>The pool that was created.</returns>
    public ObjectPool CreateNewPool(GameObject objectPrefab, int startAmount = 20)
    {
        ObjectPool newObjectPool = new GameObject().AddComponent<ObjectPool>();
        newObjectPool.SetUpPool(objectPrefab, startAmount);

        objectPools.Add(newObjectPool);

        return newObjectPool;
    }

    /// <summary>
    ///     Returns the pool containing the specified object prefab.
    ///     Creates and returns a new pool if none is found.
    /// </summary>
    /// <param name="objectPrefab"></param>
    /// <returns></returns>
    public ObjectPool GetObjectPoolBasedOnPrefab(GameObject objectPrefab)
    {
        foreach (ObjectPool objectPool in objectPools.Where(objectPool => objectPool.GetPooledObjectPrefab() == objectPrefab))
            return objectPool;

        Debug.LogWarning("That object is NOT yet pooled! Creating a new pool...");
        return CreateNewPool(objectPrefab, 1);
    }
}