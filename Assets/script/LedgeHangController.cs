using UnityEngine;
using System.Collections;

public class LedgeHangController : MonoBehaviour
{
    public Transform ledgeGrabCheck; // Empty GameObject placed at player’s chest level
    public float ledgeDetectRange = 0.5f;
    public LayerMask ledgeLayer;

    public Transform hangPositionOffset; // Empty GameObject indicating where to snap for hanging
    public float climbUpDelay = 0.5f;

    private Rigidbody rb;
    private PlayerMovement playerMovement; // Your existing movement script
    private bool isHanging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>(); // Modify this if your movement script has a different name
    }

    void Update()
    {
        if (!isHanging && rb.velocity.y < 0)
        {
            if (CheckForLedge())
            {
                EnterHangState();
            }
        }

        if (isHanging)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(ClimbLedge());
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                ExitHangState(); // Drop
            }
        }
    }

    bool CheckForLedge()
    {
        RaycastHit hit;
        if (Physics.Raycast(ledgeGrabCheck.position, transform.forward, out hit, ledgeDetectRange, ledgeLayer))
        {
            return true;
        }
        return false;
    }

    void EnterHangState()
    {
        isHanging = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        playerMovement.enabled = false;

        // Snap to hanging position
        transform.position = hangPositionOffset.position;
    }

    IEnumerator ClimbLedge()
    {
        yield return new WaitForSeconds(climbUpDelay);

        isHanging = false;
        rb.useGravity = true;
        playerMovement.enabled = true;

        // Move player slightly up and forward to simulate climb
        transform.position += new Vector3(0, 1.5f, 0.5f);
    }

    void ExitHangState()
    {
        isHanging = false;
        rb.useGravity = true;
        playerMovement.enabled = true;
    }
}
