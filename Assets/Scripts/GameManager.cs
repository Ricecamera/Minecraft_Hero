using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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
    public static GameManager instance = null;    // static instance of GameManager which allows it to be accessed by any other script.

    // Wave involving parameter;
    private Wave currentWave;
    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;

    public Transform[] spawnPoint;
    public Zombie zombiePrefab;

    // Game State
    public int playerLife = 3;
    public int level = 0;
    public bool isGameOver = false;

    // UI
    //public TextMeshProUGUI lifeText;
    //public TextMeshProUGUI gameOverText;


    private AudioSource backgroundMusic;

    private void Awake() {
        // Singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        backgroundMusic = GetComponent<AudioSource>();
        NextWave();
    }

    void Update()
    {
        if (isGameOver)
        {
            //gameOverText.gameObject.SetActive(true);
            Debug.Log("Game Over");
            return;
        }
    }

    private void NextWave() {
        level++;
        currentWave = new Wave(level);
        enemiesRemainingToSpawn = currentWave.numberOfEnemies;
        enemiesRemainingAlive = enemiesRemainingToSpawn;

        // Create Timer for spawning enemies
        StartCoroutine(SpawnWave(currentWave)); 
    }

    private void OnEnemyDeath() {
        enemiesRemainingAlive--;

        if (enemiesRemainingAlive == 0) {
            NextWave();
        }
    }

    private void OnPlayerDeath() {
        isGameOver = true;
    }

    IEnumerator SpawnWave(Wave currentWave)
    {
        print("Wave: " + currentWave.waveLevel);
        int n = currentWave.numberOfEnemies;
        float delay = currentWave.spawnInterval;
        Transform randomPoint;
        for (int i=0; i < n; i++)
        {
            // If game is over, Don't spawn zombie anymore
            if (isGameOver) break;

            enemiesRemainingToSpawn--;

            // Set random spawn position
            randomPoint = spawnPoint[Random.Range(0, spawnPoint.Length)];
            Zombie spawnedEnemy = Instantiate(zombiePrefab, randomPoint.position, Quaternion.identity);
            spawnedEnemy.OnDeath += OnEnemyDeath;

            if (level > 10) {
                spawnedEnemy.setHealth(5);
            }
            yield return new WaitForSeconds(delay);
        }
    }
}
