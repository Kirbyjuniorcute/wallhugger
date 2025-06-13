using UnityEngine;

public class EnemyMovementVisuals : MonoBehaviour
{
    public GameObject[] movementSprites; // Assign different movement GameObjects
    public float cycleSpeed = 0.2f; // Time between visual updates
    private int currentIndex = 0;
    private float cycleTimer = 0f;

    void Start()
    {
        UpdateMovementVisual();
    }

    void Update()
    {
        cycleTimer += Time.deltaTime;

        if (cycleTimer >= cycleSpeed)
        {
            CycleMovementVisual();
            cycleTimer = 0f;
        }
    }

    void CycleMovementVisual()
    {
        // Disable current sprite
        movementSprites[currentIndex].SetActive(false);

        // Move to the next sprite
        currentIndex = (currentIndex + 1) % movementSprites.Length;

        // Enable new sprite
        movementSprites[currentIndex].SetActive(true);
    }

    void UpdateMovementVisual()
    {
        foreach (GameObject sprite in movementSprites)
        {
            sprite.SetActive(false);
        }
        movementSprites[currentIndex].SetActive(true);
    }
}