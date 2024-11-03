using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Spawner : MonoBehaviour
{
    // can be used to spawn enemies, vertebrae, or any additional items appearing throughout play

    public enum SpawnerType
    {
        Enemy,
        Vertebrae,
        Other
    }

    [SerializeField]
    private SpawnerType spawnerType;

    [SerializeField]
    private GameObject objectToPool;

    [Space]
    [SerializeField]
    [Tooltip("How many enemies will spawn within one minute (ex: 10 = one enemy spawn every 6 seconds")]
    private float spawnRate;

    [SerializeField]
    [Tooltip("Radius of 0 will have objects always spawning exactly at the location of the spawner.")]
    private float spawnAreaRadius;

    [SerializeField]
    [Tooltip("How many objects can be currently spawned at once.")]
    private int objectCap;

    [SerializeField]
    [Tooltip("Minimum distance the object will spawn from the center of the spawner. Can be used to create ring-shaped spawn areas.")]
    private float minimumDistanceFromCenter;

    [SerializeField]
    [Tooltip("Minimum distance object can spawn from other spawned objects.")]
    private float minDistanceFromOtherObjects;

    [SerializeField]
    [Tooltip("Mask of layers to keep min distance from when spawning.")]
    private LayerMask objectMask;

    [SerializeField]
    [Tooltip("If checked, will only spawn when player is within the trigger collider.")]
    private bool onlySpawnWhenInTrigger;

    [Space]
    [Header("Editor Fields")]
    [SerializeField]
    [Tooltip("Only affects radius gizmo in editor view - for object differentiation/organization in scene.")]
    private Color spawnAreaVisualColor;

    private bool isActive;
    private float spawnTimer;
    private float maxNumSpawnAttemps = 8; // max number of times the game will try to spawn an object if it continues to be too close to other objects

    private int numObjectsActive; // how many objects are currently spawned in the scene
    private GameObject[] objectPool;

    // Start is called before the first frame update
    void Start()
    {
        if (spawnerType == SpawnerType.Enemy)
        {
            objectPool = EnemyManager.Instance.objectPool;
        }
        else if (spawnerType == SpawnerType.Vertebrae)
        {
            // need to create vertebrae pool manager script/object
            objectPool = ObjectPooler.CreateObjectPool(objectToPool, 50);
        }

        numObjectsActive = 0;
        if (onlySpawnWhenInTrigger)
        {
            isActive = false;
        }
        else
        {
            isActive = true;
        }

        // start w random offset for spawnTime
        spawnTimer = Random.Range(0, 60 / spawnRate);

        if (spawnerType == SpawnerType.Enemy)
        {
            UpdateEnemyObjectCap();
        }

        if (spawnerType == SpawnerType.Enemy)
            GameManager.Instance.houseDestroyedDel += UpdateEnemyObjectCap;
    }

/*    private void OnEnable()
    {
        if (spawnerType == SpawnerType.Enemy)
            GameManager.Instance.houseDestroyedDel += UpdateEnemyObjectCap;
    }    
    private void OnDisable()
    {
        if (spawnerType == SpawnerType.Enemy)
            GameManager.Instance?.houseDestroyedDel -= UpdateEnemyObjectCap;
    }*/

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        float amountOfTimeBtwSpawns = 60 / spawnRate;
        if (spawnerType == SpawnerType.Enemy)
        {
            amountOfTimeBtwSpawns /= GameManager.Instance.enemySpawnMod;
        }

        if (spawnTimer > amountOfTimeBtwSpawns)
        {
            Vector3 spawnPos = GetRandomSpawnLocation();
            
            // check if a valid spawn pos was found
            if (spawnPos != Vector3.zero)
            {
                SpawnObject(spawnPos);
                spawnTimer = 0;
            }
        }

        spawnTimer += Time.deltaTime;
    }

    private void SpawnObject(Vector3 spawnLocation)
    {
        if (numObjectsActive < objectCap)
        {
            foreach (GameObject obj in objectPool)
            {
                if (obj && !obj.activeInHierarchy)
                {
                    Debug.Log("enemy spawned");
                    obj.transform.position = spawnLocation;
                    obj.SetActive(true);

                    numObjectsActive++;

                    if (numObjectsActive >= objectCap)
                    {
                        EnableSpawner(false);
                    }
                    return;
                }
            }
        }
        else
        {
            Debug.Log("Spawner tried to spawn object but is at its cap!");
        }
    }

    private void DespawnObject(GameObject objectToDespawn)
    {
        objectToDespawn.SetActive(false);
        numObjectsActive--;

        if (!isActive && numObjectsActive < objectCap)
        {
            EnableSpawner(true);
        }
    }

    private Vector3 GetRandomSpawnLocation()
    {
        bool validSpawnPosFound = false;
        int spawnAttemptsCounter = 0;
        Vector3 spawnPos = Vector3.zero;

        while (!validSpawnPosFound && spawnAttemptsCounter < maxNumSpawnAttemps)
        {
            Quaternion rotation = new Quaternion();
            rotation.eulerAngles = new Vector3(0, 0, Random.value * 360);
            Vector3 randPos = Vector3.zero;
            randPos.x = Random.Range(minimumDistanceFromCenter, spawnAreaRadius);

            randPos = rotation * randPos;
            randPos += transform.position;
            randPos = new Vector3(randPos.x, randPos.y, 0);

            // check if other objects are nearby
            RaycastHit2D circleHit = Physics2D.CircleCast(randPos, minDistanceFromOtherObjects, Vector3.zero, 1, objectMask);

            if (circleHit.collider == null)
            {
                validSpawnPosFound = true;
                spawnPos = randPos;
            }
            else
            {
                spawnAttemptsCounter++;
            }
        }

        if (spawnPos == Vector3.zero) Debug.Log("couldn't find valid spawn point");
        return spawnPos;
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
        objectCap = newCap;
    }

    private void UpdateEnemyObjectCap()
    {
        SetObjectCap(GameManager.Instance.numDestructiblesLeft);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (onlySpawnWhenInTrigger && collision.gameObject.name == "Player")
        {
            isActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (onlySpawnWhenInTrigger && collision.gameObject.name == "Player")
        {
            isActive = false;
        }
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
