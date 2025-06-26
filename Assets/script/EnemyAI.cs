
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
    public float attackRange = 2f;
    public float attackCooldown = 1f; //  Editable cooldown in seconds
    public LayerMask obstacleMask;
    public EnemySpawner spawner;

    private float lastAttackTime = -Mathf.Infinity;
    private Rigidbody rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 moveDir = directionToPlayer;

        // Basic directions
        bool forwardBlocked = IsObstacleInDirection(transform.forward);
        bool backBlocked = IsObstacleInDirection(-transform.forward);
        bool leftBlocked = IsObstacleInDirection(-transform.right);
        bool rightBlocked = IsObstacleInDirection(transform.right);

        // Diagonal directions
        Vector3 fwdRight = (transform.forward + transform.right).normalized;
        Vector3 fwdLeft = (transform.forward - transform.right).normalized;
        Vector3 backRight = (-transform.forward + transform.right).normalized;
        Vector3 backLeft = (-transform.forward - transform.right).normalized;

        bool fwdRightBlocked = IsObstacleInDirection(fwdRight);
        bool fwdLeftBlocked = IsObstacleInDirection(fwdLeft);
        bool backRightBlocked = IsObstacleInDirection(backRight);
        bool backLeftBlocked = IsObstacleInDirection(backLeft);

        // Decision logic
        if (forwardBlocked)
        {
            if (!rightBlocked) moveDir = transform.right;
            else if (!leftBlocked) moveDir = -transform.right;
            else if (!fwdRightBlocked) moveDir = fwdRight;
            else if (!fwdLeftBlocked) moveDir = fwdLeft;
            else if (!backRightBlocked) moveDir = backRight;
            else if (!backLeftBlocked) moveDir = backLeft;
            else if (!backBlocked) moveDir = -transform.forward;
            else moveDir = Vector3.zero;
        }
        else if ((leftBlocked && rightBlocked) && !backBlocked)
        {
            moveDir = -transform.forward;
        }

        // Smooth rotate
        if (moveDir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Move
        Vector3 moveVector = transform.forward * moveSpeed;
        moveVector.y = rb.velocity.y;
        rb.velocity = moveVector;

        // Attack with delay
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(20);
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    bool IsObstacleInDirection(Vector3 dir)
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, obstacleDetectRange, obstacleMask);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Health left: {currentHealth}");

        if (currentHealth <= 0)
        {
            if (spawner != null)
                spawner.OnEnemyDied();

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddEnemyKillXP();
            }

            gameObject.SetActive(false);
        }
    }

    public void ResetEnemy()
    {
        currentHealth = maxHealth;
    }
}
