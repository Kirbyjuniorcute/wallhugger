using UnityEngine;

public class TriggerObjectMover : MonoBehaviour
{
    [Header("References")]
    public GameObject player;               // Assign Player GameObject
    public GameObject objectToMove;         // The GameObject to move

    [Header("Movement Settings")]
    public Vector3 targetWorldPosition;     // The world position to move to
    public float moveSpeed = 2f;            // Speed of movement
    public float activationDistance = 3f;   // Player must be within this distance

    private Vector3 originalPosition;
    private Vector3 currentTarget;
    private bool isMoving = false;
    private bool atOriginalPosition = true;

    void Start()
    {
        if (objectToMove != null)
        {
            originalPosition = objectToMove.transform.position;
            currentTarget = targetWorldPosition;
        }
    }

    void Update()
    {
        if (player == null || objectToMove == null) return;

        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (distanceToPlayer <= activationDistance && Input.GetKeyDown(KeyCode.E) && !isMoving)
        {
            // Toggle the direction
            currentTarget = atOriginalPosition ? targetWorldPosition : originalPosition;
            atOriginalPosition = !atOriginalPosition;
            isMoving = true;
        }

        if (isMoving)
        {
            objectToMove.transform.position = Vector3.MoveTowards(
                objectToMove.transform.position,
                currentTarget,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(objectToMove.transform.position, currentTarget) < 0.01f)
            {
                isMoving = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(targetWorldPosition, 0.2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
