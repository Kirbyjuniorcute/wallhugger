using UnityEngine;

public class AimDownSight : MonoBehaviour
{
    [Header("ADS Settings")]
    public GameObject normalView;      // UI like crosshair
    public GameObject adsView;         // Scope overlay or zoom cam

    [Header("Muzzle Flash")]
    public GameObject normalMuzzleFlash; // Muzzle flash for normal
    public GameObject adsMuzzleFlash;    // Muzzle flash for ADS

    [Header("Zoom Settings")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float adsFOV = 40f;
    public float zoomSpeed = 10f;

    private bool isAiming = false;
    private PlayerSlideDuck2 slideScript;

    void Start()
    {
        slideScript = GetComponent<PlayerSlideDuck2>();
    }

    void Update()
    {
        isAiming = Input.GetMouseButton(1);
        bool isSlidingOrDucking = slideScript != null && (slideScript.IsSliding() || slideScript.IsDucking());
        bool blockNormalView = isAiming || isSlidingOrDucking;

        // Toggle views
        if (normalView != null) normalView.SetActive(!blockNormalView);
        if (adsView != null) adsView.SetActive(isAiming);

        // Muzzle flash toggle (based on aiming only)
        if (normalMuzzleFlash != null) normalMuzzleFlash.SetActive(isAiming == false);
        if (adsMuzzleFlash != null) adsMuzzleFlash.SetActive(isAiming);

        // Smooth zoom
        if (playerCamera != null)
        {
            float targetFOV = isAiming ? adsFOV : normalFOV;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        }
    }

    public bool IsAiming => isAiming;
}
