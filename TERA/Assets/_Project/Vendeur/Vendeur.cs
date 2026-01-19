using UnityEngine;

public class Vendeur : MonoBehaviour
{
    [Header("Paramètres")]
    public GameObject weaponPrefab;
    public int prixArme = 50;
    public Transform spawnPoint;
    public float distanceAchatMax = 3.5f;


    private Transform joueurTransform;
    private Joueur joueurScript;
    private bool joueurEstDansLaZone = false;

    void Start()
    {
        if (Camera.main != null)
        {
            joueurTransform = Camera.main.transform;
            joueurScript = FindObjectOfType<Joueur>();
        }
        else {
            Debug.Log("Pas de cam sur la scène WTF MAN");
        }
    }

    void Update()
    {
        if (joueurScript == null) return;

        // Utilisation de ton script ToolsSceneRange
        bool estAPortee = ToolsSceneRange.IsWithinRange(joueurTransform, transform, distanceAchatMax);

        // On détecte le moment précis où le joueur "entre" dans la zone
        if (estAPortee && !joueurEstDansLaZone)
        {
            TenterAchat(joueurScript);
            joueurEstDansLaZone = true;
        }
        // Reset quand le joueur sort de la zone
        else if (!estAPortee && joueurEstDansLaZone)
        {
            joueurEstDansLaZone = false;
        }
    }

    public void TenterAchat(Joueur joueur)
    {
        if (joueur.Argent >= prixArme)
        {
            joueur.Argent -= prixArme;
            ApparitionArme();
        }
        else
        {
            Debug.Log("Pas assez d'argent !");
        }
    }

    private void ApparitionArme()
    {
        Instantiate(weaponPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    // Visualisation de la zone dans l'éditeur
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanceAchatMax);
    }
}