using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public List<GameObject> allPrefabsToPool = new List<GameObject>();
    public static Dictionary<string, Queue<GameObject>> dictAllPrefabs;

    public const int NUMBER_OF_PREFABS_TO_SPAWN = 10;

    #region Singleton

    public static PrefabManager singleton;

    void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        singleton = this;
        DontDestroyOnLoad(gameObject);

        dictAllPrefabs = new Dictionary<string, Queue<GameObject>>();
        for (int i = 0; i < allPrefabsToPool.Count; i++)
        {
            Queue<GameObject> goQueue = new Queue<GameObject>();
            for (int x = 0; x < NUMBER_OF_PREFABS_TO_SPAWN; x++)
            {
                GameObject go = Instantiate(allPrefabsToPool[i], transform);
                go.SetActive(false);
                goQueue.Enqueue(go);
            }
            dictAllPrefabs.Add(allPrefabsToPool[i].name, goQueue);
        }
    }

    #endregion

    public static GameObject GetPrefab(string refName)
    {
        if (!dictAllPrefabs.ContainsKey(refName))
        {
            Debug.Log("Prefab " + refName + " does not exist.");
            return null;
        }
        if (dictAllPrefabs[refName].Count == 0)
        {
            Debug.Log("Prefab Queue " + refName + " is empty.");
            return null;
        }

        GameObject obj = dictAllPrefabs[refName].Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public static void ReturnPrefabToPool(string refName, GameObject prefab)
    {
        if (!dictAllPrefabs.ContainsKey(refName))
        {
            Debug.Log("Prefab Queue " + refName + " does not exist.");
            return;
        }

        prefab.transform.SetParent(singleton.transform);
        prefab.SetActive(false);
        dictAllPrefabs[refName].Enqueue(prefab);
    }

    public static void WaitThenReturnPrefabToPool(string refName, GameObject prefab, float timeToWait)
    {
        if (!dictAllPrefabs.ContainsKey(refName))
        {
            Debug.Log("Prefab Queue " + refName + " does not exist.");
            return;
        }

        singleton.StartCoroutine(singleton.WaitThenReturnPrefabToPoolCoroutine(refName, prefab, timeToWait));
    }

    private IEnumerator WaitThenReturnPrefabToPoolCoroutine(string refName, GameObject prefab, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        prefab.SetActive(false);
        dictAllPrefabs[refName].Enqueue(prefab);
    }
}