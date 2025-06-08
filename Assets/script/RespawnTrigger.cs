
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform respawnPoint; // Drag the respawn point GameObject here

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure your player has the "Player" tag
        {
            if (respawnPoint != null)
            {
                other.transform.position = respawnPoint.position;
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero; // Reset velocity to prevent weird motion after respawn
                }
            }
            else
            {
                Debug.LogWarning("Respawn point not assigned on " + gameObject.name);
            }
        }
    }
}
