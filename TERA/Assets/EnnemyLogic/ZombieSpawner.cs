using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform playerTransform; // Référence au transform du joueur
    public float spawnInterval = 3.0f;
    public float minDistance = 5.0f; // Distance minimum du joueur
    public float maxDistance = 10.0f; // Distance maximum du joueur

    void Start()
    {
        // Cherche automatiquement le joueur si non assigné
        if (playerTransform == null)
        {
            throw new System.Exception("Player Transform is not assigned!");
        }
        if (zombiePrefab == null)
        {
            throw new System.Exception("Zombie Prefab is not assigned!");
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnZombie();
        }
    }

    void SpawnZombie()
    {
        // Génère un point entre minDistance et maxDistance
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float randomDist = Random.Range(minDistance, maxDistance);

        Vector3 spawnOffset = new Vector3(randomDir.x, 0, randomDir.y) * randomDist;
        Vector3 spawnPos = playerTransform.position + spawnOffset;

        Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
    }
}