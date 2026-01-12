using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("Références XRI")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor magazineSocket;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptyClickSound;

    [Header("Paramètres de Tir")]
    public Transform firePoint;
    public float range = 100f;
    public float damage = 20f;
    public LayerMask hitLayers;

    [Header("Cadence de Tir")]
    public float fireRate = 10f; 
    private float nextTimeToFire = 0f;
    private bool isTriggerHeld = false;

    [Header("Effets Visuels")]
    public ParticleSystem muzzleFlash;

    [Header("Animation Culasse (Bolt)")]
    public Transform boltObject;
    public float boltRecoilDistance = -0.05f;
    private Vector3 boltStartPosition;
    private Coroutine boltCoroutine;

    private bool isMagazineInserted = false;

    void Start()
    {
        if (boltObject != null)
        {
            boltStartPosition = boltObject.localPosition;
        }
    }

    void OnEnable()
    {
        if (magazineSocket != null)
        {
            magazineSocket.selectEntered.AddListener(OnMagazineInserted);
            magazineSocket.selectExited.AddListener(OnMagazineRemoved);
        }
    }

    void OnDisable()
    {
        if (magazineSocket != null)
        {
            magazineSocket.selectEntered.RemoveListener(OnMagazineInserted);
            magazineSocket.selectExited.RemoveListener(OnMagazineRemoved);
        }
    }

    public void StartShooting(ActivateEventArgs args) { isTriggerHeld = true; }
    public void StopShooting(DeactivateEventArgs args) { isTriggerHeld = false; }

    void Update()
    {
        if (isTriggerHeld && isMagazineInserted)
        {
            if (Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Fire();
            }
        }
        else if (isTriggerHeld && !isMagazineInserted && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            if (emptyClickSound) audioSource.PlayOneShot(emptyClickSound);
        }
    }

    private void OnMagazineInserted(SelectEnterEventArgs args)
    {
        isMagazineInserted = true;
        if (reloadSound) audioSource.PlayOneShot(reloadSound);
    }

    private void OnMagazineRemoved(SelectExitEventArgs args)
    {
        isMagazineInserted = false;
    }

    private void Fire()
    {
        if (shootSound) audioSource.PlayOneShot(shootSound);
        if (muzzleFlash != null) { muzzleFlash.Stop(); muzzleFlash.Play(); }

        if (boltObject != null)
        {
            if (boltCoroutine != null) StopCoroutine(boltCoroutine);
            boltCoroutine = StartCoroutine(AnimateBoltCycle());
        }

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, range, hitLayers))
        {
            ZombieAI zombie = hit.transform.GetComponentInParent<ZombieAI>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage, hit);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * 50f);
            }
        }
    }

    IEnumerator AnimateBoltCycle()
    {
        float cycleDuration = 1f / fireRate; 
        Vector3 recoilPosition = boltStartPosition + new Vector3(0, 0, boltRecoilDistance);
        float timer = 0f;

        while (timer < cycleDuration)
        {
            float progress = timer / cycleDuration;

            if (progress < 0.2f)
            {
                boltObject.localPosition = Vector3.Lerp(boltStartPosition, recoilPosition, progress / 0.2f);
            }
            else
            {
                float returnProgress = (progress - 0.2f) / 0.8f; 
                boltObject.localPosition = Vector3.Lerp(recoilPosition, boltStartPosition, returnProgress);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        boltObject.localPosition = boltStartPosition;
    }
}