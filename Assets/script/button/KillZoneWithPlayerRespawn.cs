
using UnityEngine;

public class KillZoneWithPlayerRespawn : MonoBehaviour
{
    [Header("Enemy Settings")]
    public string enemyTag = "Enemy"; // Ensure enemies are tagged properly

    [Header("Player Settings")]
    public PlayerHealth playerHealth;      // Assign your Player GameObject here
    public Transform respawnPoint;         // Assign a Transform for player respawn
    public float respawnHealthPercent = 1f; // Restore % of health (1 = full, 0.5 = half)

    private void OnTriggerEnter(Collider other)
    {
        // Handle enemy
        if (other.CompareTag(enemyTag))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>() ?? other.GetComponentInParent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(enemy.maxHealth); // Instant kill
                Debug.Log("Enemy killed by kill zone.");
            }
        }

        // Handle player
        if (playerHealth != null && other.gameObject == playerHealth.gameObject)
        {
            Debug.Log("Player touched kill zone. Respawning...");

            // Set health back to X percent
            int newHealth = Mathf.RoundToInt(playerHealth.maxHealth * respawnHealthPercent);
            playerHealth.SetHealth(newHealth); // Custom method we’ll add below

            // Move to respawn point
            if (respawnPoint != null)
            {
                playerHealth.transform.position = respawnPoint.position;

                // Reset velocity if rigidbody exists
                Rigidbody rb = playerHealth.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
            else
            {
                Debug.LogWarning("Respawn point not assigned.");
            }
        }
    }
}
