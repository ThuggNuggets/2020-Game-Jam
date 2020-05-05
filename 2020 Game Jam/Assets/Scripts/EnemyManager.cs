using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    public Transform spawnPointParent;
    public Transform enemyParent;
    public GameObject enemyPrefab;

    [Header("Spawning Properties")]
    public float timeBetweenEnemySpawns = 5.0f;
    public float numberOfEnemies = 1.0f;
    public float addNumberOfEnemies = 0.2f;
    public bool enemiesSpawned = false;

    [Header("Enemy Properties")]
    public List<Transform> obstacles = new List<Transform>();
    public Transform playerTransform;

    // Privates
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

    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        _enemySpawnTimer -= Time.deltaTime;
        if (_enemySpawnTimer <= 0.0f)
        {
            enemiesSpawned = false;
        }

        if (!enemiesSpawned)
        {
            _enemySpawnTimer -= Time.deltaTime;
            for (int i = 0; i < (int) numberOfEnemies; i++)
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
        GameObject enemy = Instantiate(enemyPrefab, spawn.position, spawn.rotation, enemyParent);
        EnemyController controller = enemy.GetComponentInChildren<EnemyController>();
        controller.SetupReferences(playerTransform, obstacles);
    }
}
