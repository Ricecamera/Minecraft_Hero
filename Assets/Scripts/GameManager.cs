using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[System.Serializable]
public class Wave
{
    private static int STARTING_ENEMY_NUM = 5;
    private static float STARTING_SPAWN_INTERVAL = 3f;

    public int waveLevel;
    public int numberOfEnemies;
    public float spawnInterval;

    public Wave(int level) {
        waveLevel = level;

        if (1 <= waveLevel && waveLevel <= 6) {
            numberOfEnemies = STARTING_ENEMY_NUM + (waveLevel - 1)*2;
            spawnInterval = STARTING_SPAWN_INTERVAL - (waveLevel - 1) * .2f;
        } 
        else if (6 < waveLevel && waveLevel <= 15) {
            numberOfEnemies = STARTING_ENEMY_NUM + (waveLevel - 6) * 4 + 10;
            spawnInterval = STARTING_SPAWN_INTERVAL - (waveLevel - 6) * .15f;
        }
        else if (15 < waveLevel) {
            numberOfEnemies = STARTING_ENEMY_NUM + (waveLevel - 15) * 8 + 46;
            spawnInterval = 0.5f;
        }
        else {
            numberOfEnemies = STARTING_ENEMY_NUM;
            spawnInterval = STARTING_SPAWN_INTERVAL;
        }
    }
}

public class GameManager : MonoBehaviour
{
    private static int MAX_PLAYER_LIFE = 10;
    private static int MAX_AIRDROPS = 20;
    private static float AIRDROP_STARTING_Y = 50f;

    public static GameManager instance = null;    // static instance of GameManager which allows it to be accessed by any other script.

    // Wave involving parameter;
    private Wave currentWave;
    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;

    // Gameplay
    private int playerLife = 3;
    private int currentAirDrop = 0;
    public float playerSpawnDelay = 3f;
    public float airDropSpawnTime = 5f;
    public int level = 0;
    public long score = 0;
    public bool isGameOver = false;

    // Object References
    private UIManager UImanager;
    public Board boardManager;
    public Transform[] enemySpawnPoint;
    public Zombie zombiePrefab;
    public Player playerPrefab;
    public GameObject boxPrefab;

    public UnityEvent OnPlayerSpawn;

    private void Awake() {
        // Singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        UImanager = GameObject.Find("UI").GetComponent<UIManager>();
        Reset();
        SpawnPlayer(new Vector3(0, 0, 0));
        //NextWave();
        StartCoroutine(SpawnAirDrop());
    }

    void Update()
    {
    }


    private void NextWave() {
        level++;
        currentWave = new Wave(level);
        enemiesRemainingToSpawn = currentWave.numberOfEnemies;
        enemiesRemainingAlive = enemiesRemainingToSpawn;
        
        // Clear wave score
        if (level > 1) {
            score += 1500 + 250*level;
            UImanager.UpdateScore(score);
        }

        // Create Timer for spawning enemies
        StartCoroutine(SpawnWave(currentWave)); 
    }

    private void OnEnemyDeath() {
        enemiesRemainingAlive--;
        score += 150;
        UImanager.UpdateScore(score);

        if (enemiesRemainingAlive == 0) {
            NextWave();
        }
    }

    private void OnPlayerDeath() {
        if (playerLife == 0) {
            print("Game over!!");
            isGameOver = true;
            return;
        }

        playerLife--;
        UImanager.UpdateLife(playerLife);
        StartCoroutine(SpawnPlayer());
    }

    private void OnAirDropDestroy() {
        if (currentAirDrop > 0) {
            currentAirDrop--;
            score += 300;
            UImanager.UpdateScore(score);
        }
    }

    private IEnumerator SpawnWave(Wave currentWave)
    {
        print("Wave: " + currentWave.waveLevel);
        int n = currentWave.numberOfEnemies;
        float delay = currentWave.spawnInterval;
        Transform randomPoint;
        for (int i=0; i < n; i++)
        {
            // If game is over, Don't spawn zombie anymore
            if (isGameOver) {
                enemiesRemainingToSpawn = 0;
                yield return null;
                break;
            }

            enemiesRemainingToSpawn--;

            // Set random spawn position
            randomPoint = enemySpawnPoint[Random.Range(0, enemySpawnPoint.Length)];
            Zombie spawnedEnemy = Instantiate(zombiePrefab, randomPoint.position, Quaternion.identity);
            spawnedEnemy.OnDeath.AddListener(OnEnemyDeath);

            if (level > 10) {
                spawnedEnemy.setHealth(5);
            }
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator SpawnPlayer() {
        yield return new WaitForSeconds(playerSpawnDelay);
        Player playerEntity = Instantiate(playerPrefab, GetRandomPosition(), Quaternion.identity);
        playerEntity.OnDeath.AddListener(OnPlayerDeath);
        OnPlayerSpawn?.Invoke();
    }

    private void SpawnPlayer(Vector3 spawnPostion) {
        if (OnPlayerSpawn == null)
            OnPlayerSpawn = new UnityEvent();
        Player playerEntity = Instantiate(playerPrefab, spawnPostion, Quaternion.identity);
        playerEntity.OnDeath.AddListener(OnPlayerDeath);
    }

    private IEnumerator SpawnAirDrop() {
        // Prevent from spawning an airdrop on game starting
        yield return new WaitForSeconds(airDropSpawnTime * 2);

        while (!isGameOver) {
            int randomIndex = Random.Range(0, boardManager.gridPositions.Count);
            Board.Grid currentGrid = boardManager.gridPositions[randomIndex];

            if (currentGrid.isVacant && currentAirDrop < MAX_AIRDROPS) {
                currentGrid.isVacant = false;
                Vector3 spawnPos = new Vector3(currentGrid.Position.x, AIRDROP_STARTING_Y, currentGrid.Position.y);
                GameObject box = Instantiate(boxPrefab, spawnPos, Quaternion.identity);
                Airdrop airdrop = box.GetComponent<Airdrop>();
                airdrop.OnDestroy.AddListener(OnAirDropDestroy);
                currentAirDrop++;
            }
            yield return new WaitForSeconds(airDropSpawnTime);
        }
    }

    public void IncresePlayerLife(int addLife) {
        if (playerLife + addLife <= MAX_PLAYER_LIFE) {
            playerLife += addLife;
            UImanager.UpdateLife(playerLife);
        }
        else
            playerLife = MAX_PLAYER_LIFE;
        score += 500;
        UImanager.UpdateScore(score);
    }

    public Vector3 GetRandomPosition() {
        float xBound = CameraManager.instance.Bound.x;
        float zBound = CameraManager.instance.Bound.z;

        float xPos = Random.Range(-xBound + 5, xBound - 5);
        float zPos = Random.Range(-zBound + 5, zBound - 5);
        return new Vector3(xPos, 0, zPos);
    }

    public void Reset() {
        playerLife = 3;
        currentAirDrop = 0;
        level = 0;
        score = 0;
        isGameOver = false;
        UImanager.UpdateScore(score);
        UImanager.UpdateLife(playerLife);
    }
}
