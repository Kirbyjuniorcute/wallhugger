
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyProximitySFX : MonoBehaviour
{
    [Header("SFX Settings")]
    public float triggerRange = 5f;
    public bool playOnce = true; // If true, will only play once per approach

    [Header("References")]
    public Transform player;
    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource.clip == null)
        {
            Debug.LogWarning("No AudioClip assigned to AudioSource on " + gameObject.name);
        }

        if (player == null)
        {
            Debug.LogError("Player reference not assigned to EnemyProximitySFX on " + gameObject.name);
        }
    }

    void Update()
    {
        if (player == null || audioSource == null || audioSource.clip == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= triggerRange)
        {
            if (playOnce)
            {
                if (!hasPlayed)
                {
                    audioSource.Play();
                    hasPlayed = true;
                }
            }
            else
            {
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
        }
        else
        {
            if (!playOnce && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
