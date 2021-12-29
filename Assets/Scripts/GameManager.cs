using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class Wave
{
    public string waveName;
    public int numberOfEnemies;
    public float spawnInterval;
}
public class GameManager : MonoBehaviour
{
    public Wave[] waves;
    public Transform[] spawnPoint;
    public GameObject enemyPrefab;
    
    // Wave involving parameter;
    private Wave currentWave;
    private int currentWaveNumber;
    private int waveIdx = 0;
    private int maxWaveIdx;

    // Game State
    public int lifePoint = 20;
    public bool isGameOver = false;

    // UI
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI gameOverText;

    private AudioSource backgroundMusic;
    private void Start()
    {
        currentWave = waves[waveIdx];
        maxWaveIdx = waves.Length;
        backgroundMusic = GetComponent<AudioSource>();
        StartCoroutine(SpawnWave(currentWave));
    }

    // Update is called once per frame
    void Update()
    {
        lifeText.SetText("Villager Life: " + lifePoint);
        if (isGameOver)
        {
            gameOverText.gameObject.SetActive(true);
            Debug.Log("Game Over");
            return;
        }
        int enemyCount = GameObject.FindGameObjectsWithTag("Zombie").Length;

        // If enemy is all destroyed spawn a new wave;
        if (enemyCount == 0 && !isGameOver)
        {
            waveIdx++;
            if (waveIdx > maxWaveIdx) // Check for the final wave;
            {
                isGameOver = true;
                return;
            }
            currentWave = waves[waveIdx];
            StartCoroutine(SpawnWave(currentWave)); // Create Coroutine for Spawn a Enemy;
        }
    }

    IEnumerator SpawnWave(Wave currentWave)
    {
        Debug.Log(currentWave.waveName);
        int n = currentWave.numberOfEnemies;
        float delay = currentWave.spawnInterval;
        Transform randomPoint;
        for (int i=0; i < n; i++)
        {
            if (isGameOver) break;
            randomPoint = spawnPoint[Random.Range(0, spawnPoint.Length)];
            Instantiate(enemyPrefab, randomPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(delay);
        }
    }
}
