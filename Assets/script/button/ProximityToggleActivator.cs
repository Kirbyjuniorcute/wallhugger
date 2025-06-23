
using UnityEngine;

public class ProximityToggleActivator : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject player;                 // Assign your player GameObject
    public float activationDistance = 3f;     // Distance to trigger the interaction

    [Header("Target Objects")]
    public GameObject[] objectsToToggle;      // Assign GameObjects to toggle

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= activationDistance && Input.GetKeyDown(KeyCode.E))
        {
            ToggleObjects();
            Debug.Log("Toggled target objects!");
        }
    }

    void ToggleObjects()
    {
        foreach (GameObject obj in objectsToToggle)
        {
            if (obj != null)
                obj.SetActive(!obj.activeSelf);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
