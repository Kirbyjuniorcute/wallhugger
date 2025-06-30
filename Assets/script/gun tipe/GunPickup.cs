
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public GunData gunData;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            GunController controller = other.GetComponentInChildren<GunController>();
            if (controller != null)
            {
                controller.LoadGun(gunData);
                Debug.Log($"Picked up {gunData.displayName}!");
                Destroy(gameObject); // Optional: remove the pickup
            }
        }
    }
}
