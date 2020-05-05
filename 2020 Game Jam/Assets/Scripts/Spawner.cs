using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float numberOfEnemies = 1.0f;
    public float addNumberOfEnemies = 0.2f;
    public float timeBetweenEachEnemySpawn = 5f;
    public bool enemiesSpawned = false;
    public Transform[] spawnerPositions;
    private float tempSpawnTimer = 0.0f;

    // Use this for initialization
    void Start()
    {
        tempSpawnTimer = timeBetweenEachEnemySpawn;
    }

    // Update is called once per frame
    void Update()
    {
        tempSpawnTimer -= Time.deltaTime;

        if (tempSpawnTimer <= 0.0f)
        {
            enemiesSpawned = false;
        }

        if (!enemiesSpawned)
        {
            tempSpawnTimer -= Time.deltaTime;
            for (int i = 0; i < (int)numberOfEnemies; i++)
            {
                int spawnNumber = Random.Range(0, spawnerPositions.Count());

                Instantiate(objectToSpawn, spawnerPositions[spawnNumber].transform.position, Quaternion.identity);
            }

            tempSpawnTimer = timeBetweenEachEnemySpawn;
            numberOfEnemies += addNumberOfEnemies;
            enemiesSpawned = true;
        }
    }
}