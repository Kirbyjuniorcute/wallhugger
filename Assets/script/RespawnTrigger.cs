
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform respawnPoint; // Set this to where objects should be sent back to

    private void OnTriggerEnter(Collider other)
    {
        if (respawnPoint != null)
        {
            // Move object to respawn point
            other.transform.position = respawnPoint.position;

            // Reset velocity if it has a Rigidbody
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            Debug.LogWarning("Respawn point not assigned on " + gameObject.name);
        }
    }
}
