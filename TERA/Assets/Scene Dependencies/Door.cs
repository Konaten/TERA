using UnityEngine;


public class Door : MonoBehaviour
{
    private  Joueur playerScript;
    private Transform player;
    public GameObject pivot;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpen = false;
    bool alreadyOpen = false;
    private Quaternion closedRotation;
    private Quaternion targetRotation;

    void Start()
    {
        playerScript = Object.FindAnyObjectByType<Joueur>();
        player = Camera.main.transform;
        if (playerScript == null || player == null)
        {
            Debug.LogError("Door: Impossible de trouver le joueur dans la scène.");
        }

        if (pivot == null)
        {
            enabled = false;
            return;
        }

        closedRotation = pivot.transform.rotation;
        targetRotation = closedRotation;

        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(_ => ToggleDoor());
            Debug.Log("Appuyé");
        }
    }

    void Update()
    {
        pivot.transform.rotation = Quaternion.Lerp(pivot.transform.rotation, targetRotation, Time.deltaTime * openSpeed);
    }

    void ToggleDoor()
    {
        // Si la porte est déjà ouverte
        if (isOpen)
        {
            isOpen = false;
            targetRotation = closedRotation;
            return;
        }

        int prixPorte = 10;

    if (alreadyOpen)
    {
        isOpen = true;
        targetRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }
    else if (playerScript != null && playerScript.RetirerArgent(prixPorte))
    {
        isOpen = true;
        alreadyOpen = true;
        targetRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
        Debug.Log("Porte déverrouillée !");
    }
    // Pas payé et pas assez d'argent
    else
    {
        Debug.Log("Action impossible : " + prixPorte + " $ requis.");
    }
    }
}
