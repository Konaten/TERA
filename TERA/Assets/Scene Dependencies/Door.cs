using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform player;
    public Joueur playerScript;
    public GameObject pivot;
    public float interactDistance = 2f;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpen = false;
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

        Debug.Log("pivot " + pivot.name);
        if (pivot == null)
        {
            Debug.LogError("La porte doit avoir un parent qui sert de pivot !");
            enabled = false;
            return;
        }

        closedRotation = pivot.transform.rotation;
        targetRotation = closedRotation;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, pivot.transform.position);
        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.X))
        {
            ToggleDoor();
        }

        pivot.transform.rotation = Quaternion.Lerp(pivot.transform.rotation, targetRotation, Time.deltaTime * openSpeed);
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        targetRotation = isOpen ? closedRotation * Quaternion.Euler(0, openAngle, 0) : closedRotation;
    }
}