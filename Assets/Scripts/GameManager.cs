using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Destructible[] destructibles { get; private set; }
    public float enemySpawnMod { get; private set; }
    public int numDestructiblesLeft;

    private PlayerHealth player;
    [SerializeField] private Canvas gameoverUI;
    [SerializeField] private Canvas gameWinUI;

    [SerializeField] private AudioEvent _gameoverSFX;
    [SerializeField] private AudioEvent _gamewinSFX;

    private int maxEnemySpawnMod = 3;

    public delegate void HouseDestroyedDelegate();
    public HouseDestroyedDelegate houseDestroyedDel;

    private AudioSource _audio;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;

        player = FindObjectOfType<PlayerHealth>();
        player.OnDeath.AddListener(OnPlayerDeath);

        _audio = GetComponent<AudioSource>();

        destructibles = FindObjectsByType<Destructible>(FindObjectsSortMode.None);

        Destructible[] tempDestructibleHolder = new Destructible[destructibles.Length];
        int tempIndex = 0;

        foreach (Destructible destructible in destructibles)
        {

            if (destructible._necessaryForGameWin)
            {
                tempDestructibleHolder[tempIndex] = destructible;
                tempIndex++;
                Debug.Log(tempIndex);
            }
        }

        destructibles = tempDestructibleHolder;

        numDestructiblesLeft = tempIndex;
        enemySpawnMod = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
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
        enemySpawnMod = Mathf.Lerp(1, maxEnemySpawnMod, Mathf.InverseLerp(destructibles.Length, 0, numDestructiblesLeft));
        houseDestroyedDel?.Invoke();

        if (numDestructiblesLeft == 0)
        {
            OnGameEnd();
        }

    }

    private void OnPlayerDeath()
    {
        gameoverUI.gameObject.SetActive(true);
        if(_gameoverSFX)
        {
            _gameoverSFX.Play(_audio);
        }
    }

    private void OnGameEnd()
    {
        gameWinUI.gameObject.SetActive(true);
        if(_gamewinSFX)
        {
            _gamewinSFX.Play(_audio);
        }
    }
}
