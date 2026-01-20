using UnityEngine;
using TMPro;

public class VendeurChargeur : MonoBehaviour
{
    [Header("Paramètres")]
    public GameObject chargeurPrefab;
    public Transform spawnPoint;
    public float distanceAchatMax = 3.5f;
    public int prixParBalle = 1;
    public int pasIncrement = 15;

    [Header("UI")]
    public TextMeshProUGUI texteQuantite;

    private int quantiteSelectionnee = 0;
    private Joueur joueurRef;
    private Transform joueurTransform;

    void Start()
    {
        joueurRef = Object.FindAnyObjectByType<Joueur>();
        joueurTransform = Camera.main.transform;
        if(joueurRef == null || joueurTransform == null)
        {
            Debug.LogError("VendeurChargeur: Impossible de trouver le joueur dans la scène.");
        }
        UpdateUI();
    }

    // Appelé par le bouton [+]
    public void AugmenterQuantite()
    {
        Debug.Log("Augmenter");
        quantiteSelectionnee += pasIncrement;
        UpdateUI();
    }

    // Appelé par le bouton [-]
    public void DiminuerQuantite()
    {
        Debug.Log("Diminuer");
        quantiteSelectionnee = Mathf.Max(0, quantiteSelectionnee - pasIncrement);
        UpdateUI();
    }

    // Appelé par le bouton [ACHETER]
    public void ConfirmerAchat()
    {
        Debug.Log("Acheter");
        if (joueurRef == null || quantiteSelectionnee <= 0) return;

        if (!ToolsSceneRange.IsWithinRange(joueurTransform, transform, distanceAchatMax))
        {
            Debug.Log("Trop loin !");
            return;
        }

        int coutTotal = quantiteSelectionnee * prixParBalle;

        if (joueurRef.Argent >= coutTotal)
        {
            joueurRef.Argent -= coutTotal;
            SpawnChargeurSurMesure(quantiteSelectionnee);
            
            // On reset après l'achat
            quantiteSelectionnee = 0;
            UpdateUI();
        }
    }

    private void SpawnChargeurSurMesure(int quantite)
    {
        GameObject go = Instantiate(chargeurPrefab, spawnPoint.position, spawnPoint.rotation);
        Magazine scriptChargeur = go.GetComponent<Magazine>();

        if (scriptChargeur != null)
        {
            scriptChargeur.currentAmmo = quantite;

            if (quantite > scriptChargeur.maxAmmo) 
                scriptChargeur.maxAmmo = quantite;
        }
    }

    private void UpdateUI()
    {
        if (texteQuantite != null)
            texteQuantite.text = "Nombre de balle: " + quantiteSelectionnee.ToString() + "\nPrix total: " + (quantiteSelectionnee * prixParBalle).ToString() + " $";
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanceAchatMax);
    }
}   