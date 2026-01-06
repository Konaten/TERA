using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Prefab du zombie à faire apparaître.")]
    public GameObject zombiePrefab;

    [Tooltip("Transform de la caméra attachée au joueur.")]
    public Transform playerTransform; // Référence au transform du joueur
    [Tooltip("Intervalle de temps entre chaque apparition de zombie.")]
    public float spawnInterval = 3.0f;
    [Tooltip("Distance minimale entre le joueur et le zombie")]
    public float minDistance = 5.0f; // Distance minimum du joueur
    [Tooltip("Distance maximale entre le joueur et le zombie")]
    public float maxDistance = 10.0f; // Distance maximum du joueur

    void Start()
    {
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

        Vector3 directionToPlayer = playerTransform.position - spawnPos;
        directionToPlayer.y = 0; // Empêche le zombie de pencher vers le haut/bas

        Quaternion spawnRotation = Quaternion.LookRotation(directionToPlayer);
        
        Instantiate(zombiePrefab, spawnPos, spawnRotation);
    }
}