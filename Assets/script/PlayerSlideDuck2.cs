using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerSlideDuck2 : MonoBehaviour
{
    public float slideForce = 10f;
    public float slideDuration = 1f;
    public float duckHeight = 0.5f;
    public float normalHeight = 1f;
    public float slideSpeedMultiplier = 2f;

    [Header("Objects to Disable When Sliding")]
    public List<GameObject> objectsToDisableOnSlide;

    [Header("Objects to Enable When Sliding")]
    public List<GameObject> objectsToEnableOnSlide;

    private Rigidbody rb;
    private BoxCollider box;
    private PlayerMovement movementScript;

    private bool isSliding = false;
    private bool isDucking = false;
    private float slideTimer = 0f;

    private Vector3 originalCenter;
    private Vector3 originalSize;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        movementScript = GetComponent<PlayerMovement>();

        originalCenter = box.center;
        originalSize = box.size;
    }

    void Update()
    {
        bool isMoving = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude > 0.1f;
        bool ctrlDown = Input.GetKeyDown(KeyCode.LeftControl);
        bool ctrlUp = Input.GetKeyUp(KeyCode.LeftControl);

        if (ctrlDown)
        {
            if (isMoving && IsGrounded())
                StartSlide();
            else
                StartDuck();
        }

        if (ctrlUp)
        {
            if (isSliding)
                StopSlide();
            else if (isDucking)
                StopDuck();
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
                StopSlide();
        }
    }

    void StartSlide()
    {
        if (isSliding) return;

        isSliding = true;
        slideTimer = slideDuration;

        AdjustColliderForCrouch();
        rb.AddForce(transform.forward * slideForce, ForceMode.Impulse);

        ToggleGameObjects(objectsToDisableOnSlide, false);
        ToggleGameObjects(objectsToEnableOnSlide, true);

        // Apply speed boost
        if (movementScript != null)
            movementScript.speedMultiplier = slideSpeedMultiplier;
    }

    void StopSlide()
    {
        isSliding = false;
        ResetCollider();

        ToggleGameObjects(objectsToDisableOnSlide, true);
        ToggleGameObjects(objectsToEnableOnSlide, false);

        // Reset speed boost
        if (movementScript != null)
            movementScript.speedMultiplier = 1f;
    }

    void StartDuck()
    {
        if (isDucking) return;

        isDucking = true;
        AdjustColliderForCrouch();
        // Optional: Add ducking-specific toggles if needed
    }

    void StopDuck()
    {
        isDucking = false;
        ResetCollider();
        // Optional: Add ducking-specific toggles if needed
    }

    void AdjustColliderForCrouch()
    {
        box.size = new Vector3(originalSize.x, duckHeight, originalSize.z);
        box.center = new Vector3(originalCenter.x, duckHeight / 2f, originalCenter.z);
    }

    void ResetCollider()
    {
        box.size = originalSize;
        box.center = originalCenter;
    }

    void ToggleGameObjects(List<GameObject> objects, bool state)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(state);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, movementScript.groundCheckDistance + 0.1f);
    }
}
