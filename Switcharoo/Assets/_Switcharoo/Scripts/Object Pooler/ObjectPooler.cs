using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour, IPauseable
{
    #region Variables
    public Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    public List<GameObject> pooledObjects;
    public List<Transform> pooledObjectParents;

    public int growthRate = 5;
    public int initalPoolSize = 5;

    public static ObjectPooler instance { get; private set; }










    List<Rigidbody2D> m_pauseRbs = new List<Rigidbody2D>();
    List<IPauseable> m_pauseableObjectsIPause = new List<IPauseable>();
    #endregion

    void Awake()
    {
        instance = this;
        InitialGrowth();
    }


    private void Start()
    {
        PauseMenuController.instance.m_pauseables.Add(this);
    }

    ///<Summary>
    //returns an object from the pooled objects
    ///needs the name of the pool to access it
    ///returns an object from the pool
    ///Called from the script that needs the object ie. the gun scripts
    ///<summary>
    public GameObject NewObject(GameObject requestedObject, Transform spawner, bool moveToTransform = true, bool rotateToTransform = true, bool isActive = true)
    {
        return GetNewObject(requestedObject.name, requestedObject, spawner, moveToTransform, rotateToTransform, isActive);
    }

    GameObject GetNewObject(string poolName, GameObject requestedObject, Transform spawner, bool moveToTransform = true, bool rotateToTransform = true, bool isActive = true)
    {
        if (!objectPool.ContainsKey(poolName))
        {
            CreateNewPool(requestedObject);
        }

        GameObject newObject = objectPool[poolName].Dequeue();
        if (objectPool[poolName].Count == 0)
        {
            IncreasePool(poolName, newObject, newObject.transform.parent.gameObject);
        }

        if (spawner != null)
        {

            if (moveToTransform)
            {
                newObject.transform.position = spawner.position;
            }

            if (rotateToTransform)
            {
                newObject.transform.rotation = spawner.rotation;
            }
        }
        else
        {
            if (moveToTransform)
            {
                newObject.transform.position = transform.position;
            }

            if (rotateToTransform)
            {
                newObject.transform.rotation = spawner.rotation;
            }
        }
        newObject.SetActive(isActive);
        return newObject;
    }


    public GameObject NewObject(GameObject requestedObject, Vector3 spawnPosition, Quaternion angle, bool isActive = true)
    {
        return GetNewObject(requestedObject.name, requestedObject, spawnPosition, angle, isActive);
    }

    GameObject GetNewObject(string poolName, GameObject requestedObject, Vector3 spawnPostion, Quaternion angle, bool isActive)
    {
        if (!objectPool.ContainsKey(poolName))
        {
            CreateNewPool(requestedObject);
        }

        GameObject newObject = objectPool[poolName].Dequeue();
        if (objectPool[poolName].Count == 0)
        {
            IncreasePool(poolName, newObject, newObject.transform.parent.gameObject);
        }

        newObject.transform.position = spawnPostion;
        newObject.transform.rotation = angle;
        newObject.SetActive(isActive);
        return newObject;
    }

    ///<summary>
    ///When the pool is equal to zero, increase the pool
    ///called in the NewObject function
    ///<summary>
    void IncreasePool(string poolName, GameObject pooledObject, GameObject poolParent)
    {
        for (int i = 0; i < growthRate; i++)
        {
            GameObject newObj = Instantiate(pooledObject);
            newObj.transform.parent = poolParent.transform;
            newObj.SetActive(false);
            newObj.name = pooledObject.name;
            objectPool[poolName].Enqueue(newObj);



            IPauseable pauseMe = newObj.GetComponent<IPauseable>();
            if (pauseMe != null)
            {
                m_pauseableObjectsIPause.Add(pauseMe);
                return;
            }

            Rigidbody2D rb = newObj.GetComponent<Rigidbody2D>();
            if (rb!= null)
            {
                m_pauseRbs.Add(rb);
            }
        }
    }

    ///<summary>
    ///Returns the object to it's designated pool
    ///Called from the object
    ///<summary>
    public void ReturnToPool(GameObject pooledObject)
    {
        if (!objectPool.ContainsKey(pooledObject.name))
        {
            CreateNewPool(pooledObject);
        }
        objectPool[pooledObject.name].Enqueue(pooledObject);
        pooledObject.SetActive(false);
    }



    public void AddObjectToPooler(GameObject pooledObject)
    {


        IPauseable pauseMe = pooledObject.GetComponent<IPauseable>();
        if (pauseMe != null)
        {
            m_pauseableObjectsIPause.Add(pauseMe);
            return;
        }

        Rigidbody2D rb = pooledObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            m_pauseRbs.Add(rb);
        }


    }


    ///<summary>
    ///This function is only called at start
    ///creates all the pools, and puts then under the right transform
    ///<summary>
    void InitialGrowth()
    {
        int indexNumbers = 0;
        foreach (GameObject newPool in pooledObjects)
        {
            Queue<GameObject> currentPool = new Queue<GameObject>();
            pooledObjectParents[indexNumbers].name = newPool.name + " Pool";
            for (int i = 0; i < initalPoolSize; i++)
            {
                GameObject newObj = Instantiate(newPool);
                newObj.transform.parent = pooledObjectParents[indexNumbers];
                currentPool.Enqueue(newObj);
                newObj.name = newPool.gameObject.name;
                newObj.SetActive(false);
            }

            string poolName = newPool.name;
            objectPool.Add(poolName, currentPool);
            indexNumbers += 1;
        }
    }

    void CreateNewPool(GameObject newPool)
    {
        GameObject newParent = new GameObject(newPool.name + "s");

        pooledObjectParents.Add(newParent.transform);
        pooledObjects.Add(newPool);
        newParent.transform.parent = this.transform;
        newParent.name = newPool.name;
        Queue<GameObject> newQueue = new Queue<GameObject>();
        objectPool.Add(newPool.name, newQueue);
        IncreasePool(newPool.name, newPool, newParent);

    }

    public void SetPauseState(bool p_isPaused)
    {
        foreach (IPauseable pauseMe in m_pauseableObjectsIPause)
        {
            pauseMe.SetPauseState(p_isPaused);
        }
        foreach(Rigidbody2D rb in m_pauseRbs)
        {
            rb.simulated = !p_isPaused;
        }
    }
}
