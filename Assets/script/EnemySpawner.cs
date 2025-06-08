using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

using System;





public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    // Instead of prefab, assign enemies from scene (disabled)
    public GameObject[] enemiesPool;

    public int enemiesPerWave = 3;
    public float timeBetweenWaves = 5f;
    public float spawnDelay = 0.5f;

    [Header("Wave Settings")]
    public int currentWave = 0;
    public int increasePerWave = 2;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("UI")]
    public UnityEngine.UI.Text waveText;

    public GameObject enemyPrefab; // Assign in Inspector



    private int enemiesAlive = 0;
    private bool isSpawningWave = false;

    void Start()
    {
        // Disable all enemies in pool at start
        foreach (var enemy in enemiesPool)
        {
            enemy.SetActive(false);
            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.spawner = this; // Assign spawner reference
            }
        }
    }

    void Update()
    {
        if (!isSpawningWave && enemiesAlive <= 0)
        {
            StartCoroutine(StartNextWave());
        }
    }

    IEnumerator StartNextWave()
    {
        isSpawningWave = true;
        currentWave++;
        int enemiesToSpawn = enemiesPerWave + (currentWave - 1) * increasePerWave;

        if (waveText != null)
        {
            waveText.text = "Wave " + currentWave;
        }

        Debug.Log($"Starting Wave {currentWave} with {enemiesToSpawn} enemies.");

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawningWave = false;
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Missing spawn points.");
            return;
        }

        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        GameObject enemyToSpawn = null;

        // Find a disabled enemy in the pool
        foreach (GameObject enemy in enemiesPool)
        {
            if (!enemy.activeInHierarchy)
            {
                enemyToSpawn = enemy;
                break;
            }
        }

        if (enemyToSpawn == null)
        {
            if (enemyPrefab != null)
            {
                enemyToSpawn = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                Array.Resize(ref enemiesPool, enemiesPool.Length + 1);
                enemiesPool[enemiesPool.Length - 1] = enemyToSpawn;
            }
            else
            {
                Debug.LogWarning("No available enemies in pool, and no prefab assigned.");
                return;
            }
        }


        // Reactivate and reset the enemy
        enemyToSpawn.transform.position = spawnPoint.position;
        enemyToSpawn.transform.rotation = spawnPoint.rotation;
        enemyToSpawn.SetActive(true);

        EnemyAI enemyAI = enemyToSpawn.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.ResetEnemy(); // Optional: reset health
            enemyAI.spawner = this;
        }

        enemiesAlive++;
    }
    public void OnEnemyDied()
    {
        enemiesAlive--;
    }

}
