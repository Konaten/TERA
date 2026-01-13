using UnityEngine;
using System.Collections;

public class ChargerSpawner : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject chargerPrefab;
    public float respawnDelay = 2f;

    private GameObject currentCharger;

    void Start()
    {
        SpawnCharger();
    }

    void SpawnCharger()
    {
        if (currentCharger != null) return;

        currentCharger = Instantiate(chargerPrefab, transform.position, transform.rotation);

        ChargerPickup pickup = currentCharger.GetComponent<ChargerPickup>();
        if (pickup != null)
        {
            pickup.spawner = this;
        }
    }

    public void OnChargerPicked()
    {
        currentCharger = null;
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnCharger();
    }
}
