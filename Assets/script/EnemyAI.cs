

using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public int maxHealth = 3;
    private int currentHealth;

    public float rotationSpeed = 5f;
    public float obstacleDetectRange = 1f; // How far ahead to check for walls
    public LayerMask obstacleMask; // Assign this to "Ground", "Walls", "Enemy", etc.
    public EnemySpawner spawner;


    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            // Obstacle detection
            if (IsObstacleInFront())
            {
                // Try slight turning to avoid obstacle
                direction = Quaternion.Euler(0, 45, 0) * direction;
                Debug.DrawRay(transform.position, direction * obstacleDetectRange, Color.red);
            }

            // Rotate toward the direction
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }

            // Move forward
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    bool IsObstacleInFront()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, obstacleDetectRange, obstacleMask);
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(20);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            if (spawner != null)
                spawner.OnEnemyDied();

            gameObject.SetActive(false);
        }
    }


    public void ResetEnemy()
    {
        currentHealth = maxHealth;
    }



}
