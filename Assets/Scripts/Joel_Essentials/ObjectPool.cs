#region
using System.Collections.Generic;
using UnityEngine;
#endregion

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject objectPrefab;
    [SerializeField] int startAmount;

    List<GameObject> pooledObjects = new List<GameObject>();

    void Start() { InstantiateStartAmount(); }

    public void SetUpPool(GameObject objectPrefab, int startAmount)
    {
        this.objectPrefab = objectPrefab;
        this.startAmount  = startAmount;

        gameObject.name  = objectPrefab.name + " Pool";
        transform.parent = ObjectPoolManager.Instance.transform;
    }

    /// <summary>
    ///     Instantiates the specified number of starting objects for the pool.
    /// </summary>
    void InstantiateStartAmount()
    {
        for (int i = 0; i < startAmount; i++) { CreatePooledObject(); }
    }

    /// <summary>
    ///     Instantiates an object, adds it to the pool and makes it inactive.
    /// </summary>
    /// <returns>The object that was created.</returns>
    public GameObject CreatePooledObject()
    {
        GameObject newObject = Instantiate(objectPrefab);
        newObject.transform.parent = transform;
        newObject.SetActive(false);
        pooledObjects.Add(newObject);
        return newObject;
    }

    /// <summary>
    ///     Returns an object from the pool.
    /// </summary>
    /// <param name="setActive">Depicts if the object should be active on return.</param>
    /// <returns></returns>
    public GameObject GetPooledObject(bool setActive = false)
    {
        GameObject objectToReturn = null;

        foreach (GameObject pooledObject in pooledObjects)
        {
            if (!pooledObject.activeInHierarchy) objectToReturn = pooledObject;
        }

        if (objectToReturn == null) objectToReturn = CreatePooledObject();

        objectToReturn.SetActive(setActive);
        return objectToReturn;
    }

    /// <summary>
    ///     Returns the prefab of the object this pool contains.
    /// </summary>
    /// <returns></returns>
    public GameObject GetPooledObjectPrefab() { return objectPrefab; }
}
