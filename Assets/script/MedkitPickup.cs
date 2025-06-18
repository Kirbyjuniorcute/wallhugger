using UnityEngine;

public class MedkitPickup : MonoBehaviour
{
    public int healAmount = 50;
    public float interactDistance = 2.5f;
    private Transform player;
    private PlayerHealth playerHealth;
    private MedkitManager medkitManager;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        medkitManager = FindObjectOfType<MedkitManager>();
    }

    void Update()
    {
        if (player == null || playerHealth == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E))
        {
            playerHealth.Heal(healAmount);
            medkitManager.OnMedkitUsed();
        }
    }
}
