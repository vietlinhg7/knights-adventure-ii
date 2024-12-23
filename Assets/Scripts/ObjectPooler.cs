using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        print(pools.Count);
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
        ShowPoolDictionary();
    }
    // Function to show the contents of poolDictionary
    public void ShowPoolDictionary()
    {
        Debug.Log("Pool Dictionary Contents:");

        foreach (var entry in poolDictionary)
        {
            Debug.Log($"Key: {entry.Key}, Value: Queue with {entry.Value.Count} objects");
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;

        GameObject objectToReuse = poolDictionary[tag].Dequeue();
        poolDictionary[tag].Enqueue(objectToReuse);
        return objectToReuse;
    }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
}
