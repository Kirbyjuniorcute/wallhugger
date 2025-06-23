
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [Header("Settings")]
    public GameObject player;              // Assign the Player GameObject here
    public string nextSceneName;           // Name of the scene in Build Settings
    public float activationDistance = 3f;  // Distance the player must be within to activate

    private bool playerInRange = false;

    void Update()
    {
        if (player == null || string.IsNullOrEmpty(nextSceneName))
            return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        playerInRange = distance <= activationDistance;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player pressed E to change scene.");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a wire sphere in the editor to show trigger radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
