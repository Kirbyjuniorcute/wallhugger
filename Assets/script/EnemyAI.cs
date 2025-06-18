
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public int maxHealth = 3;
    private int currentHealth;

    public float rotationSpeed = 5f;
    public float obstacleDetectRange = 1f;
    public LayerMask obstacleMask;
    public EnemySpawner spawner;

    private Rigidbody rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;         // Let gravity do its job
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent tipping over
    }

    void FixedUpdate() // physics-based movement
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;

        // Obstacle detection
        if (IsObstacleInFront())
        {
            direction = Quaternion.Euler(0, 45, 0) * direction;
        }

        // Rotate smoothly
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Move using Rigidbody (keeps grounded)
        Vector3 moveVector = transform.forward * moveSpeed;
        moveVector.y = rb.velocity.y; // preserve gravity effect
        rb.velocity = moveVector;
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
        Debug.Log($"{gameObject.name} took {amount} damage. Health left: {currentHealth}");

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
