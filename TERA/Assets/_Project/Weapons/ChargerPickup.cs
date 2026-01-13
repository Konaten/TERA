using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChargerPickup : MonoBehaviour
{
    [HideInInspector]
    public ChargerSpawner spawner;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrabbed);
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (spawner != null)
        {
            spawner.OnChargerPicked();
        }

        // Optionnel : détruire après un petit délai
        // Destroy(gameObject, 0.05f);
    }
}
