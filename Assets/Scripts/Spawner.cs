using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // can be used to spawn enemies, vertebrae, or any additional items appearing throughout play

    [SerializeField]
    private GameObject objectToSpawn;
    [Tooltip("How many of the object should be pooled on start. This will be the maximum number that can be active/on screen at once at the start of the game.")]
    [SerializeField]
    private int numToPool;

    [Space]
    [SerializeField]
    [Tooltip("How many enemies will spawn within one minute (ex: 10 = one enemy spawn every 6 seconds")]
    private float spawnRate;
    [SerializeField]
    [Tooltip("Radius of 0 will have objects always spawning exactly at the location of the spawner.")]
    private float spawnAreaRadius;
    [SerializeField]
    [Tooltip("Minimum distance the object will spawn from the center of the spawner. Can be used to create ring-shaped spawn areas.")]
    private float minimumDistanceFromCenter;

    [Space]
    [Header("Editor Fields")]
    [SerializeField]
    [Tooltip("Only affects radius gizmo in editor view - for object differentiation/organization in scene.")]
    private Color spawnAreaVisualColor;

    private bool isActive;
    private float spawnTimer;

    private int currentObjectCap; // how many objects can currently spawned at once
    private int numObjectsActive; // how many objects are currently spawned in the scene
    private GameObject[] objectPool;

    // Start is called before the first frame update
    void Start()
    {
        objectPool = ObjectPooler.CreateObjectPool(objectToSpawn, numToPool);
        numObjectsActive = 0;
        currentObjectCap = numToPool;
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        if (spawnTimer > 60 / spawnRate)
        {
            SpawnObject(GetRandomSpawnLocation());
            spawnTimer = 0;
        }

        spawnTimer += Time.deltaTime;
    }

    private void SpawnObject(Vector3 spawnLocation)
    {
        if (numObjectsActive < currentObjectCap)
        {
            foreach (GameObject obj in objectPool)
            {
                if (!obj.activeInHierarchy)
                {
                    obj.transform.position = spawnLocation;
                    obj.SetActive(true);

                    numObjectsActive++;

                    if (numObjectsActive >= currentObjectCap)
                    {
                        EnableSpawner(false);
                    }
                    return;
                }
            }
        }
        else
        {
            Debug.Log(objectToSpawn + " Spawner tried to spawn object but is at its cap!");
        }
    }

    private void DespawnObject(GameObject objectToDespawn)
    {
        objectToDespawn.SetActive(false);
        numObjectsActive--;

        if (!isActive && numObjectsActive < currentObjectCap)
        {
            EnableSpawner(true);
        }
    }

    private Vector3 GetRandomSpawnLocation()
    {
        Quaternion rotation = Quaternion.identity;
        rotation.eulerAngles = new Vector3(0, 0, Random.value * 360);
        Vector3 randPos = new Vector3();
        randPos.x = Random.Range(transform.position.x + minimumDistanceFromCenter, transform.position.x + spawnAreaRadius);

        randPos = rotation * randPos;

        return randPos;
    }

    public void EnableSpawner(bool enable)
    {
        isActive = enable;
        spawnTimer = 0;
    }

    public void SetSpawnRadius(float newSpawnAreaRadius)
    {
        spawnAreaRadius = newSpawnAreaRadius;
    }

    public void SetSpawnRate(float newSpawnRate)
    {
        spawnRate = newSpawnRate;
    }

    public void SetObjectCap(int newCap)
    {
        currentObjectCap = newCap;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = spawnAreaVisualColor;
        Gizmos.DrawWireSphere(transform.position, spawnAreaRadius);
        Gizmos.DrawWireSphere(transform.position, minimumDistanceFromCenter);
    }

    private void OnValidate()
    {
        if (spawnAreaRadius < 0)
        {
            spawnAreaRadius = 0;
        }

        if (spawnRate  < 0)
        {
            spawnRate = 0;
        }

        if (minimumDistanceFromCenter <  0)
        {
            minimumDistanceFromCenter = 0;
        }
        else if (minimumDistanceFromCenter > spawnAreaRadius)
        {
            minimumDistanceFromCenter = spawnAreaRadius;
        }
    }
}
