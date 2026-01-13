using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ZombieSounds : MonoBehaviour
{
    public float minDelay = 3f;
    public float maxDelay = 7f;

    private AudioSource audioSource;
    private AudioClip[] zombieClips;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        zombieClips = Resources.LoadAll<AudioClip>("Zombie/Sounds");

        StartCoroutine(PlayZombieSounds());
    }

    IEnumerator PlayZombieSounds()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            if (zombieClips.Length > 0 && !audioSource.isPlaying)
            {
                AudioClip clip = zombieClips[Random.Range(0, zombieClips.Length)];
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
