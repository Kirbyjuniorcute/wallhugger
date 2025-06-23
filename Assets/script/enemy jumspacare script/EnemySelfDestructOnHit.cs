using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class EnemySelfDestructOnHit : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject player; // Drag your player here
    public float detectionRange = 1.5f;

    [Header("Effect Settings")]
    public GameObject effectToEnable; // This object will activate temporarily
    public float effectDuration = 2f;

    [Header("Enemy Settings")]
    public EnemySpawner spawner; // Optional: assign if you use pooling/spawning

    private bool hasTriggered = false;

    void Update()
    {
        if (!hasTriggered && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= detectionRange)
            {
                StartCoroutine(TriggerEffectAndDie());
                hasTriggered = true;
            }
        }
    }

    IEnumerator TriggerEffectAndDie()
    {
        // Enable the effect
        if (effectToEnable != null)
            effectToEnable.SetActive(true);

        yield return new WaitForSeconds(effectDuration);

        // Disable effect again
        if (effectToEnable != null)
            effectToEnable.SetActive(false);

        // Notify spawner and deactivate
        if (spawner != null)
            spawner.OnEnemyDied();

        gameObject.SetActive(false); // Deactivate instead of destroy
    }
}
