using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class Joueur : MonoBehaviour
{
    [Header("ressource")]
    public float maxHealth = 100;
    private float currentHealth = 100;
    private int argent = 100;


    private AudioSource audioSource;
    private AudioClip[] dmgClips;

    [Header("UI")]
    public Slider healthSlider;
    public TextMeshProUGUI argentText;
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
        UpdateArgentUI();
    }


    public int Argent 
    { 
        get => argent; 
        set 
        { 
            argent = value; 
            UpdateArgentUI(); 
        } 
    }

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
    void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    void UpdateArgentUI()
    {
        if (argentText != null)
        {
            argentText.text = argent.ToString() + " $";
        }
    }

    public void AjouterArgent(int montant)
    {
        argent += montant;
        UpdateArgentUI();
        Debug.Log("Argent gagné : " + montant + " | Total : " + argent);

    }
}
