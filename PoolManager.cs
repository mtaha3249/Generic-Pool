using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct PoolData
{
    public GameObject prefab;
    public int initialSpawnAmount;
}

[Serializable]
public class SpawnedInfo
{
    public List<GameObject> spawned = new List<GameObject>();
    public int lastIndex = 0;
    public Transform parent;

    public SpawnedInfo()
    {
    }

    public SpawnedInfo(List<GameObject> spawned)
    {
        this.spawned = spawned;
    }

    public SpawnedInfo(List<GameObject> spawned, Transform parent)
    {
        this.spawned = spawned;
        this.parent = parent;
    }

    public SpawnedInfo(List<GameObject> spawned, int lastIndex)
    {
        this.spawned = spawned;
        this.lastIndex = lastIndex;
    }
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    public bool DontDestroyOnLoad;

    [SerializeField] private PoolData[] PoolData;

    private Dictionary<int, SpawnedInfo> prefabData = new Dictionary<int, SpawnedInfo>();

    public delegate void GameObjectDelegate(GameObject value);

    public GameObjectDelegate OnSpawned, OnDespawned;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.parent = null;
            if (DontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        for (int i = 0; i < PoolData.Length; i++)
        {
            List<GameObject> spawned = new List<GameObject>();
            GameObject parentOfPrefab = new GameObject();
            parentOfPrefab.name = PoolData[i].prefab.name;
            parentOfPrefab.transform.parent = transform;
            for (int j = 0; j < PoolData[i].initialSpawnAmount; j++)
            {
                GameObject reference = Instantiate(PoolData[i].prefab);
                reference.SetActive(false);
                reference.transform.parent = parentOfPrefab.transform;
                IDHandler id = reference.AddComponent<IDHandler>();
                id.id = PoolData[i].prefab.GetInstanceID();
                spawned.Add(reference);
                reference.name = PoolData[i].prefab.name + " " + j;
            }

            prefabData.Add(PoolData[i].prefab.GetInstanceID(), new SpawnedInfo(spawned, parentOfPrefab.transform));
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 spawnPosition, Quaternion spawnRotation,
        Transform parent = null)
    {
        SpawnedInfo spawnedInfo = prefabData[prefab.GetInstanceID()];

        GameObject objectToSpawn = spawnedInfo.spawned[spawnedInfo.lastIndex];
        spawnedInfo.lastIndex++;
        spawnedInfo.lastIndex = spawnedInfo.lastIndex % spawnedInfo.spawned.Count;

        objectToSpawn.transform.parent = parent != null ? parent : spawnedInfo.parent;
        objectToSpawn.transform.localPosition = spawnPosition;
        objectToSpawn.transform.localRotation = spawnRotation;
        objectToSpawn.transform.localScale = prefab.transform.localScale;

        objectToSpawn.SetActive(true);
        OnSpawned?.Invoke(objectToSpawn);

        return objectToSpawn;
    }

    public void Despawn(GameObject gameObjectToDespawn, bool cleanHierarchy)
    {
        SpawnedInfo spawnedInfo = prefabData[gameObjectToDespawn.GetComponent<IDHandler>().id];

        if (cleanHierarchy)
        {
            gameObjectToDespawn.transform.parent = spawnedInfo.parent;
        }

        gameObjectToDespawn.SetActive(false);

        OnDespawned?.Invoke(gameObjectToDespawn);
    }
}