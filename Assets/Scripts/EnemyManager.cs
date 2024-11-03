using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private GameObject objectToSpawn;
    [Tooltip("How many of the object should be pooled on start. This will be the maximum number that can be active/on screen at once at the start of the game.")]
    [SerializeField]
    private int numToPool;
    public GameObject[] objectPool;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        objectPool = ObjectPooler.CreateObjectPool(objectToSpawn, numToPool);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
