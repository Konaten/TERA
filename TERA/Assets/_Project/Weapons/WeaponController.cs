using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WeaponController : MonoBehaviour
{
    [Header("Références")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor magazineSocket;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    [Header("Settings")]
    public Transform firePoint;

    private bool isMagazineInserted = false;

    void OnEnable()
    {
        magazineSocket.selectEntered.AddListener(OnMagazineInserted);
        magazineSocket.selectExited.AddListener(OnMagazineRemoved);
    }

    void OnDisable()
    {
        magazineSocket.selectEntered.RemoveListener(OnMagazineInserted);
        magazineSocket.selectExited.RemoveListener(OnMagazineRemoved);
    }

    private void OnMagazineInserted(SelectEnterEventArgs args)
    {
        isMagazineInserted = true;
        if(reloadSound) audioSource.PlayOneShot(reloadSound);
    }

    private void OnMagazineRemoved(SelectExitEventArgs args)
    {
        isMagazineInserted = false;
    }

    public void PullTrigger()
    {
        if (isMagazineInserted)
        {
            Fire();
        }
        else
        {
            // Jouer un son de "clic" à vide ici
        }
    }

    private void Fire()
    {
        // Logique de tir à implémenter
        if(shootSound) audioSource.PlayOneShot(shootSound);
    }
}