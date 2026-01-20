using UnityEngine;
using TMPro;

public class VendeurArme : MonoBehaviour
{
    [Header("Paramètres")]
    public GameObject weaponPrefab;
    public Transform spawnPoint;
    public float distanceAchatMax = 3.5f;
    public int prixArme = 25;

    [Header("UI")]
    public TextMeshProUGUI textePrix;

    private Joueur joueurRef;
    private Transform joueurTransform;

    void Start()
    {
        joueurRef = Object.FindAnyObjectByType<Joueur>();
        joueurTransform = Camera.main.transform;
        if (joueurRef == null || joueurTransform == null)
        {
            Debug.LogError("VendeurArme: Impossible de trouver le joueur dans la scène.");
        }
        UpdateUI();
    }

    // Appelé par ton bouton unique [ACHETER]
    public void ConfirmerAchat()
    {
        if (joueurRef == null || joueurTransform == null) return;

        // Vérification de la distance
        if (!ToolsSceneRange.IsWithinRange(joueurTransform, transform, distanceAchatMax))
        {
            Debug.Log("Trop loin pour acheter l'arme !");
            return;
        }

        if (joueurRef.Argent >= prixArme)
        {
            joueurRef.Argent -= prixArme;
            Instantiate(weaponPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Arme achetée !");
        }
        else
        {
            Debug.Log("Pas assez d'argent pour l'arme.");
        }
    }

    private void UpdateUI()
    {
        if (textePrix != null)
        {
            textePrix.text = "Prix de l'arme : " + prixArme + " $";
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceAchatMax);
    }
}