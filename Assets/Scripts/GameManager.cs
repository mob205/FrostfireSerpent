using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Destructible[] destructibles { get; private set; }
    public float enemySpawnMod { get; private set; }
    public int numDestructiblesLeft;

    private PlayerHealth player;
    [SerializeField] private Canvas gameoverUI;

    private int maxEnemySpawnMod = 3;

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
        player = FindObjectOfType<PlayerHealth>();
        player.OnDeath.AddListener(OnPlayerDeath);

        destructibles = FindObjectsByType<Destructible>(FindObjectsSortMode.None);
        numDestructiblesLeft = destructibles.Length;
        enemySpawnMod = 1;

        Debug.Log("Num destructibles: " +  destructibles.Length + " Enemy mod: " + maxEnemySpawnMod);

        foreach (Destructible destructible in destructibles)
        {
            destructible.destroyedDel += OnHouseDestroyed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnHouseDestroyed()
    {
        numDestructiblesLeft--;
        enemySpawnMod = Mathf.Lerp(1, 3, Mathf.InverseLerp(destructibles.Length, 0, numDestructiblesLeft));
        Debug.Log(enemySpawnMod);
    }

    private void OnPlayerDeath()
    {
        gameoverUI.gameObject.SetActive(true);
    }
}
