using UnityEngine;
using System.Collections;

public class ReloadController : MonoBehaviour
{
    public GunController gunController;        // Reference to the GunController
    public GameObject objectToDisable;         // Optional: gun model or overlay to hide during reload
    public AimDownSight adsScript;            // Reference to AimDownSight script
    public PlayerSlideDuck2 slideScript;       // Reference to PlayerSlideDuck2 script
    public float adsCancelDelay = 0.2f;        // Delay before canceling ADS (helps avoid animation overlap)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !gunController.IsReloading())
        {
            StartCoroutine(ReloadAndToggleObject());
        }
    }

    IEnumerator ReloadAndToggleObject()
    {
        // Cancel ADS if aiming
        if (adsScript != null && adsScript.IsAiming)
        {
            yield return new WaitForSeconds(adsCancelDelay);
        }

        // Hide gun/scope/etc. during reload
        if (objectToDisable != null)
            objectToDisable.SetActive(false);

        // Stop slide visuals if sliding or ducking
        if (slideScript != null)
        {
            if (slideScript.IsSliding() || slideScript.IsDucking())
                slideScript.ForceStopSlideVisuals();
        }

        // Start the reload coroutine
        yield return StartCoroutine(gunController.ReloadRoutine());

        // Restore the gun model or visuals
        if (objectToDisable != null)
            objectToDisable.SetActive(true);
    }
}
