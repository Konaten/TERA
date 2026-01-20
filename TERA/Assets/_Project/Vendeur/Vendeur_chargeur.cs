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

    void Start()
    {
        joueurRef = Object.FindAnyObjectByType<Joueur>();
        UpdateUI();
    }

    // Appelé par le bouton [+]
    public void AugmenterQuantite()
    {
        quantiteSelectionnee += pasIncrement;
        UpdateUI();
    }

    // Appelé par le bouton [-]
    public void DiminuerQuantite()
    {
        quantiteSelectionnee = Mathf.Max(0, quantiteSelectionnee - pasIncrement);
        UpdateUI();
    }

    // Appelé par le bouton [ACHETER]
    public void ConfirmerAchat()
    {
        if (joueurRef == null || quantiteSelectionnee <= 0) return;

        if (!ToolsSceneRange.IsWithinRange(joueurRef.transform, transform, distanceAchatMax))
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
            texteQuantite.text = quantiteSelectionnee.ToString();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanceAchatMax);
    }
}   