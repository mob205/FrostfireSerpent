using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static GameObject[] CreateObjectPool(GameObject go, int numToPool)
    {
        GameObject[] pool = new GameObject[numToPool];
        GameObject parentObject = new GameObject(go.name + "_Pool");

        for (int i = 0; i < numToPool; i++)
        {
            pool[i] = Instantiate(go, parentObject.transform);
        }

        return pool;
    }
}
