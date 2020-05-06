using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    public Transform spawnPointParent;
    public Transform enemyParent;
    public GameObject[] enemyPrefabs;

    [Header("Spawning Properties")]
    public float timeBetweenEnemySpawns = 5.0f;
    public float numberOfEnemies = 1.0f;
    public float addNumberOfEnemies = 0.2f;
    public float firstWaveDelayTime = 5.0f;
    public bool enemiesSpawned = false;

    [Header("Enemy Properties")]
    public List<Transform> obstacles = new List<Transform>();
    public Transform playerTransform;

    // Privates
    private bool firstWaveSpawned = false;
    private SpawnPoint[] _spawnPoints;
    private float _enemySpawnTimer = 0.0f;

    /// <summary>
    /// Called on initialise
    /// </summary>
    private void Awake()
    {
        Random.InitState((int)System.DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        // Initialise spawn point array
        _spawnPoints = new SpawnPoint[spawnPointParent.childCount];
        for (int i = 0; i < spawnPointParent.childCount; ++i)
        {
            _spawnPoints[i] = spawnPointParent.GetChild(i).GetComponent<SpawnPoint>();
        }

        firstWaveSpawned = false;
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        // Delay before the first wave of enemies spawn
        if (!firstWaveSpawned)
            firstWaveDelayTime -= Time.deltaTime;
        if (firstWaveDelayTime <= 0.0f)
        {
            firstWaveSpawned = true;
        }

        if (firstWaveSpawned)
        {
            _enemySpawnTimer -= Time.deltaTime;
            if (_enemySpawnTimer <= 0.0f)
            {
                enemiesSpawned = false;
            }

            if (!enemiesSpawned)
            {
                _enemySpawnTimer -= Time.deltaTime;
                for (int i = 0; i < (int)numberOfEnemies; i++)
                {
                    Transform rand = GetRandomSpawn();
                    if (rand != null)
                        SpawnEnemy(rand);
                    else
                        Debug.LogWarning("All spawn points occupied! Skipping enemy spawn.");
                }

                _enemySpawnTimer = timeBetweenEnemySpawns;
                numberOfEnemies += addNumberOfEnemies;
                enemiesSpawned = true;
            }
        }
    }

    /// <summary>
    /// Returns a random spawn point.
    /// </summary>
    private Transform GetRandomSpawn()
    {
        int rand = Random.Range(0, _spawnPoints.Length);
        if (!_spawnPoints[rand].Occupied)
            return _spawnPoints[rand].transform;

        return null;
    }

    /// <summary>
    /// Instantiates a zombie at the spawn paramenter.
    /// </summary>
    /// <param name="spawn"> Location of spawn. </param>
    private void SpawnEnemy(Transform spawn)
    {
        int index = enemyPrefabs.Length;
        // Error check to make sure enemy prefabs exists to override index out of bounds
        if (index == 0)
        {
            Debug.LogError("Index out of range, missing prefabs on the Enemy Manager.");
        }
        else
        {
            int rand = Random.Range(0, enemyPrefabs.Length);
            Debug.Log("Spawned enemy prefab: " + rand);
            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                GameObject enemy = Instantiate(enemyPrefabs[rand], spawn.position, spawn.rotation, enemyParent);
                EnemyController controller = enemy.GetComponentInChildren<EnemyController>();
                controller.SetupReferences(playerTransform, obstacles);
            }
        }
    }
}

