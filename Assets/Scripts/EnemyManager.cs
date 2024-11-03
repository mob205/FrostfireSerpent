using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [SerializeField]
    private GameObject objectToSpawn;
    [Tooltip("How many of the object should be pooled on start. This will be the maximum number that can be active/on screen at once at the start of the game.")]

    private int numToPool;

    [HideInInspector]
    public GameObject[] objectPool;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;

        numToPool = FindObjectsByType<Destructible>(FindObjectsSortMode.None).Length;
        objectPool = ObjectPooler.CreateObjectPool(objectToSpawn, numToPool);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
