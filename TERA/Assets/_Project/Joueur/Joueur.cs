using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Joueur : MonoBehaviour
{
    private int pv = 100;
    private int argent = 100;


    private AudioSource audioSource;
    private AudioClip[] dmgClips;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Charge tous les sons de dégâts depuis Resources/Joueur/DmgSound
        dmgClips = Resources.LoadAll<AudioClip>("Joueur/DmgSound");
    }

    
    public int Argent { get => argent; set => argent = value; }

    void Update()
    {
    }

    public void PvRemoved(int dmgTaken)
    {
        pv -= dmgTaken;

        if (dmgClips.Length > 0)
        {
            AudioClip clip = dmgClips[Random.Range(0, dmgClips.Length)];

            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
        }
    }
}
