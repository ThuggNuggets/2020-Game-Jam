using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Debug")]
    public float timeBetweenEnemySpawns = 1.0f;

    [Header("References")]
    public Transform spawnPointParent;
    public Transform enemyParent;
    public GameObject enemyPrefab;

    [Header("Enemy Properties")]
    public List<Transform> obstacles = new List<Transform>();
    public Transform playerTransform;
    public float spawnCheckRadius = 2.0f;
    public LayerMask spawnCheckLayer;

    // Privates
    private SpawnPoint[] _spawnPoints;
    private float _enemySpawnTimer = 0.0f;
    private float _timeBetweenTimer = 0.0f;

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

    private void Update()
    {
        _timeBetweenTimer += Time.deltaTime;
        if(_timeBetweenTimer > timeBetweenEnemySpawns)
        {
            SpawnEnemy(GetRandomSpawn());
            _timeBetweenTimer = 0.0f;
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
        else
            GetRandomSpawn();

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
