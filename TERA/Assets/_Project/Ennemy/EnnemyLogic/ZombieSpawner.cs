using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Références")]
    public GameObject zombiePrefab;
    public Transform playerTransform;
    public TextMeshProUGUI waveText;
    public GameObject waveCompletedUI;
    public GameObject waveCompletedPanel;

    [Header("Paramètres de Manche")]
    [Tooltip("Manche actuelle")]
    public int currentWave = 0;
    [Tooltip("Nombre de zombies à faire apparaître pour la manche 1")]
    public int baseZombiesPerWave = 5;
    [Tooltip("Combien de zombies supplémentaires par manche")]
    public int zombiesMultiplier = 2;
    [Tooltip("Temps de repos entre deux manches")]
    public float timeBetweenWaves = 5f;

    [Header("Progression des Stats")]
    public float baseHealth = 100f;
    public float healthIncreasePerWave = 20f;
    public float baseDamage = 10f;
    public float damageIncreasePerWave = 5f;

    [Header("Paramètres de Spawn")]
    public float spawnInterval = 3.0f;
    public float minDistance = 5.0f;
    public float maxDistance = 10.0f;
    [Tooltip("Nombre max de zombies simultanés sur la carte")]
    public int maxConcurrentZombies = 5;

    private int zombiesToSpawnRemaining; // Combien il reste à faire apparaître
    private int zombiesAlive;            // Combien sont actuellement en vie
    private bool isWaveActive = false;

    void Start()
    {
        if (playerTransform == null || zombiePrefab == null)
        {
            Debug.LogError("Références manquantes dans le ZombieSpawner !");
            return;
        }

        waveCompletedUI.GetComponent<TextMeshProUGUI>().text = "";
        waveCompletedPanel.SetActive(false);
        StartCoroutine(WaveSystemRoutine());
    }

    // Gère l'enchaînement des manches
    IEnumerator WaveSystemRoutine()
    {
        while (true)
        {
            currentWave++;
            UpdateUI();
            PrepareWave();

            yield return new WaitUntil(() => zombiesToSpawnRemaining <= 0 && zombiesAlive <= 0);

            yield return StartCoroutine(ShowWaveCompletedMessage());

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
    IEnumerator ShowWaveCompletedMessage()
    {
        if (waveCompletedUI != null)
        {
            waveCompletedUI.SetActive(true);
            waveCompletedPanel.SetActive(true);
            waveCompletedUI.GetComponent<TextMeshProUGUI>().text = "MANCHE RÉUSSIE";

            yield return new WaitForSeconds(3f); // Temps d'affichage du message

            waveCompletedUI.SetActive(false); // Cache le message
            waveCompletedPanel.SetActive(false);
        }
    }

    void UpdateUI()
    {
        if (waveText != null)
        {
            string romanWave = ToRoman(currentWave);
            waveText.text = romanWave;
        }
    }

    private string ToRoman(int number)
    {
        if (number < 1) return string.Empty;

        // Tableaux de correspondance
        int[] values = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
        string[] romanNumerals = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

        string result = "";
        for (int i = 0; i < values.Length; i++)
        {
            while (number >= values[i])
            {
                number -= values[i];
                result += romanNumerals[i];
            }
        }
        return result;
    }

    void PrepareWave()
    {
        // Calcul de la difficulté : ex: Manche 1 = 5, Manche 2 = 7, etc.
        zombiesToSpawnRemaining = baseZombiesPerWave + (currentWave * zombiesMultiplier);
        zombiesAlive = 0;

        // Manches spéciales (pour le debug)
        if(currentWave <= 0){
            zombiesToSpawnRemaining = 1;
        }
        
        // On lance le spawn des zombies pour cette manche
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (zombiesToSpawnRemaining > 0)
        {
            // Si on n'a pas atteint la limite de zombies simultanés
            if (zombiesAlive < maxConcurrentZombies)
            {
                SpawnZombie();
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                // On attend un peu avant de vérifier à nouveau si une place s'est libérée
                yield return new WaitForSeconds(1f);
            }
        }
    }

void SpawnZombie()
    {
        // Calcul de la position
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = playerTransform.position + new Vector3(randomDir.x, 0, randomDir.y) * Random.Range(minDistance, maxDistance);
        spawnPos.y = 0; 

        // Apparition
        GameObject zombieGO = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);

        // Calcul et application des stats pour cette manche
        ZombieAI zombieAI = zombieGO.GetComponent<ZombieAI>();
        if (zombieAI != null)
        {
            zombieAI.spawner = this;
            
            // Calcul mathématique des stats : Base + (Manche * Augmentation)
            zombieAI.health = baseHealth + (currentWave * healthIncreasePerWave);
            zombieAI.damage = baseDamage + (currentWave * damageIncreasePerWave);
        }

        zombiesAlive++;
        zombiesToSpawnRemaining--;
    }

    // Appelé par le script de mort du zombie
    public void OnZombieKilled()
    {
        zombiesAlive--;
    }
}