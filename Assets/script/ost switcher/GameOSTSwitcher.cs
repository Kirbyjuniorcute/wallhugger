using UnityEngine;
using System.Collections;

public class GameOSTSwitcher : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip defaultOST;   // The main theme that plays normally
    public AudioClip altOST;       // The theme that plays when player shoots near enemy

    [Header("Settings")]
    public AudioSource audioSource;   // Assign the AudioSource component here
    public float enemyDetectionRange = 10f; // Range to check for nearby enemies
    public Transform playerTransform;       // Assign player transform here

    private bool isAltPlaying = false;

    void Start()
    {
        PlayDefaultOST();
    }

    void Update()
    {
        // When player shoots (left click) and near enemy
        if (Input.GetButtonDown("Fire1") && !isAltPlaying && IsEnemyNearby())
        {
            StartCoroutine(SwitchToAltOST());
        }
    }

    bool IsEnemyNearby()
    {
        if (playerTransform == null) return false;

        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, enemyDetectionRange);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
                return true;
        }
        return false;
    }

    void PlayDefaultOST()
    {
        if (audioSource == null || defaultOST == null) return;

        audioSource.clip = defaultOST;
        audioSource.loop = true;
        audioSource.Play();
        isAltPlaying = false;
    }

    IEnumerator SwitchToAltOST()
    {
        if (audioSource == null || altOST == null) yield break;

        isAltPlaying = true;

        // Play the alternate OST (no loop)
        audioSource.Stop();
        audioSource.clip = altOST;
        audioSource.loop = false;
        audioSource.Play();

        // Wait for it to finish
        yield return new WaitForSeconds(altOST.length);

        // Back to default OST
        PlayDefaultOST();
    }

    void OnDrawGizmosSelected()
    {
        // Optional: visualize enemy detection range
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, enemyDetectionRange);
        }
    }
}
