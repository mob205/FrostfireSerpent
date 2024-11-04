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
            pool[i].SetActive(false);
        }

        return pool;
    }

    public static T[] CreateObjectPool<T>(T go, int numToPool) where T : MonoBehaviour
    {
        T[] pool = new T[numToPool];
        GameObject parentObject = new GameObject(go.name + "_Pool");

        for (int i = 0; i < numToPool; i++)
        {
            pool[i] = Object.Instantiate(go, parentObject.transform);
            pool[i].gameObject.SetActive(false);
        }

        return pool;
    }
}
