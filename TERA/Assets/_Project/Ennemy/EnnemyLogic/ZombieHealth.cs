using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Effets")]
    public GameObject bloodEffectPrefab;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, RaycastHit hitInfo)
    {
        currentHealth -= damage;
        Debug.Log("Zombie touché ! PV restants : " + currentHealth);

        if (bloodEffectPrefab != null)
        {
            SpawnBlood(hitInfo);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void SpawnBlood(RaycastHit hit)
    {
        GameObject blood = Instantiate(bloodEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        blood.transform.SetParent(hit.transform);
        blood.transform.localPosition += Vector3.forward * 0.01f;
        float randomScale = Random.Range(0.03f, 0.05f);
        blood.transform.localScale = new Vector3(randomScale, randomScale, 1);
        Destroy(blood, 10f);
    }

    void Die()
    {
        Debug.Log("Zombie Mort !");
        Destroy(gameObject);
    }
}