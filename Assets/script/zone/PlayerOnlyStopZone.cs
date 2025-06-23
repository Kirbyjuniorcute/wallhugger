
using UnityEngine;

public class PlayerOnlyStopZone : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject playerObject; // Drag your Player here in Inspector

    private void OnTriggerEnter(Collider other)
    {
        // Ignore enemies
        if (other.CompareTag("Enemy"))
            return;

        if (other.gameObject == playerObject)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            var playerMovement = playerObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            Debug.Log("Player has been stopped.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Ignore enemies
        if (other.CompareTag("Enemy"))
            return;

        if (other.gameObject == playerObject)
        {
            var playerMovement = playerObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }

            Debug.Log("Player has exited the stop zone.");
        }
    }
}
