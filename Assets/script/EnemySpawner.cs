using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;



public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
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
    public Text waveText;

    [Header("Enemy Prefabs (Multiple)")]
    public GameObject[] enemyPrefabs; // Now an array of prefabs

    [Header("Spawn Offset")]
    public float spawnOffsetDistance = 2f;

    private int enemiesAlive = 0;
    private bool isSpawningWave = false;

    void Start()
    {
        foreach (var enemy in enemiesPool)
        {
            if (enemy != null)
            {
                enemy.SetActive(false);
                EnemyAI ai = enemy.GetComponent<EnemyAI>();
                if (ai != null)
                {
                    ai.spawner = this;
                }
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

        if (currentWave > 1 && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddWaveXP();
        }

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

        foreach (GameObject enemy in enemiesPool)
        {
            if (enemy != null && !enemy.activeInHierarchy)
            {
                enemyToSpawn = enemy;
                break;
            }
        }

        if (enemyToSpawn == null)
        {
            if (enemyPrefabs.Length > 0)
            {
                GameObject prefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
                enemyToSpawn = Instantiate(prefab);
                Array.Resize(ref enemiesPool, enemiesPool.Length + 1);
                enemiesPool[enemiesPool.Length - 1] = enemyToSpawn;
            }
            else
            {
                Debug.LogWarning("No enemy prefabs assigned.");
                return;
            }
        }

        Vector3 offset = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized * spawnOffsetDistance;
        Vector3 spawnPosition = spawnPoint.position + offset;

        enemyToSpawn.transform.position = spawnPosition;
        enemyToSpawn.transform.rotation = spawnPoint.rotation;
        enemyToSpawn.SetActive(true);

        EnemyAI enemyAI = enemyToSpawn.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.ResetEnemy();
            enemyAI.spawner = this;
        }

        enemiesAlive++;
    }

    public void OnEnemyDied()
    {
        enemiesAlive--;
    }
}
