using UnityEngine;

public class WaterSlowZone : MonoBehaviour
{
    [Header("Target Player")]
    public GameObject player; // Drag your Player here

    [Header("Speed Reduction Settings")]
    [Range(0.1f, 1f)] public float speedMultiplier = 0.5f;

    private PlayerMovement playerMovement;
    private PlayerSlideDuck2 slideDuck;
    private WallRun wallRun;

    private void Start()
    {
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            slideDuck = player.GetComponent<PlayerSlideDuck2>();
            wallRun = player.GetComponent<WallRun>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            // Apply slow to player
            if (playerMovement != null) playerMovement.speedMultiplier *= speedMultiplier;
            if (slideDuck != null) slideDuck.slideSpeedMultiplier *= speedMultiplier;
            if (wallRun != null) wallRun.wallRunForce *= speedMultiplier;
        }
        else if (other.CompareTag("Enemy"))
        {
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.moveSpeed *= speedMultiplier;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            // Revert slow for player
            if (playerMovement != null) playerMovement.speedMultiplier /= speedMultiplier;
            if (slideDuck != null) slideDuck.slideSpeedMultiplier /= speedMultiplier;
            if (wallRun != null) wallRun.wallRunForce /= speedMultiplier;
        }
        else if (other.CompareTag("Enemy"))
        {
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.moveSpeed /= speedMultiplier;
            }
        }
    }
}
