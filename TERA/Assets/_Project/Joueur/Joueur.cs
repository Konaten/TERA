using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Joueur : MonoBehaviour
{
<<<<<<< HEAD
    [Header("PV")]
    public float maxHealth = 100;
    private float currentHealth = 100;
=======
    private int pv = 100;
    private int argent = 100;

>>>>>>> dddc35463954cf8fd5343c7d81c9ff6dd13a125f

    private AudioSource audioSource;
    private AudioClip[] dmgClips;

    [Header("UI")]
    public Slider healthSlider;
    public float lerpSpeed = 5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Charge tous les sons de dégâts depuis Resources/Joueur/DmgSound
        dmgClips = Resources.LoadAll<AudioClip>("Joueur/DmgSound");

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    
    public int Argent { get => argent; set => argent = value; }

    void Update()
    {
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * lerpSpeed);
        }
    }

    public void PvRemoved(float dmgTaken)
    {
        currentHealth -= dmgTaken;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        if (dmgClips.Length > 0)
        {
            AudioClip clip = dmgClips[Random.Range(0, dmgClips.Length)];

            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
        }
    }

<<<<<<< HEAD
    void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
=======
    public void AjouterArgent(int montant)
    {
    argent += montant;
    Debug.Log("Argent gagné : " + montant + " | Total : " + argent);
>>>>>>> dddc35463954cf8fd5343c7d81c9ff6dd13a125f
    }
}
