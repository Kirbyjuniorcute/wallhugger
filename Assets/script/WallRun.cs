using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Wall Detection")]
    public float wallCheckDistance = 0.6f;
    public string wallTag = "Wall";
    public float minJumpHeight = 1.2f;

    [Header("Wall Running")]
    public float wallRunGravity = 1f;
    public float wallRunForce = 6f;
    public float wallJumpForce = 7f;

    [Header("Camera Tilt")]
    public Transform cameraTransform;
    public float tiltSpeed = 5f;
    public float leftWallTilt = 15f;
    public float rightWallTilt = -15f;
    public float frontWallTilt = 0f;
    public float backWallTilt = 0f;
    public float noWallTilt = 0f;

    private Rigidbody rb;
    private bool isWallRunning = false;
    private bool isWallLeft = false;
    private bool isWallRight = false;
    private bool isWallFront = false;
    private bool isWallBack = false;
    private float currentTilt = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckWalls();

        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        if (CanWallRun() && (isWallLeft || isWallRight) && isMoving)
        {
            StartWallRun();

            if (Input.GetKeyDown(KeyCode.Space))
                WallJump();
        }
        else
        {
            StopWallRun();
        }

        UpdateCameraTilt();
    }

    void CheckWalls()
    {
        isWallLeft = false;
        isWallRight = false;

        if (Physics.Raycast(transform.position, -transform.right, out RaycastHit leftHit, wallCheckDistance) &&
            leftHit.collider.CompareTag(wallTag))
        {
            // Make sure wall is actually to the left by checking if the normal points right-ish
            if (Vector3.Dot(leftHit.normal, transform.right) > 0.5f)
                isWallLeft = true;
        }

        if (Physics.Raycast(transform.position, transform.right, out RaycastHit rightHit, wallCheckDistance) &&
            rightHit.collider.CompareTag(wallTag))
        {
            // Make sure wall is actually to the right by checking if the normal points left-ish
            if (Vector3.Dot(rightHit.normal, -transform.right) > 0.5f)
                isWallRight = true;
        }

        // Front/Back can stay the same if needed
        isWallFront = Physics.Raycast(transform.position, transform.forward, out RaycastHit frontHit, wallCheckDistance)
            && frontHit.collider.CompareTag(wallTag);

        isWallBack = Physics.Raycast(transform.position, -transform.forward, out RaycastHit backHit, wallCheckDistance)
            && backHit.collider.CompareTag(wallTag);
    }


    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight);
    }

    void StartWallRun()
    {
        isWallRunning = true;
        rb.useGravity = false;

        Vector3 wallDir = transform.forward;
        rb.AddForce(wallDir * wallRunForce, ForceMode.Force);

        if (isWallLeft)
            rb.AddForce(-transform.right * 5f, ForceMode.Force);
        else if (isWallRight)
            rb.AddForce(transform.right * 5f, ForceMode.Force);
    }

    void StopWallRun()
    {
        if (isWallRunning)
        {
            isWallRunning = false;
            rb.useGravity = true;
        }
    }

    void WallJump()
    {
        Vector3 jumpDir = Vector3.up;
        if (isWallLeft) jumpDir += transform.right;
        else if (isWallRight) jumpDir -= transform.right;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(jumpDir.normalized * wallJumpForce, ForceMode.Impulse);

        StopWallRun();
    }

    void UpdateCameraTilt()
    {
        float targetTilt = noWallTilt;

        if (isWallLeft && !isWallRight)
            targetTilt = leftWallTilt;
        else if (isWallRight && !isWallLeft)
            targetTilt = rightWallTilt;
        else
            targetTilt = noWallTilt;

        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);
        cameraTransform.localRotation = Quaternion.AngleAxis(currentTilt, cameraTransform.forward);
    }

}
