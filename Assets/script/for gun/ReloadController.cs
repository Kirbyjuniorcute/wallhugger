using UnityEngine;
using System.Collections;

public class ReloadController : MonoBehaviour
{
    public GunController gunController;
    public GameObject objectToDisable;
    public AimDownSight adsScript;
    public PlayerSlideDuck2 slideScript;
    public float adsCancelDelay = 0.2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && gunController != null && !gunController.IsReloading())
        {
            StartCoroutine(ReloadAndToggleObject());
        }
    }

    IEnumerator ReloadAndToggleObject()
    {
        if (gunController == null || gunController.CurrentGun == null)
            yield break;

        if (adsScript != null && adsScript.IsAiming)
        {
            yield return new WaitForSeconds(adsCancelDelay);
        }

        if (objectToDisable != null)
            objectToDisable.SetActive(false);

        if (slideScript != null && (slideScript.IsSliding() || slideScript.IsDucking()))
        {
            slideScript.ForceStopSlideVisuals();
        }

        gunController.StartReload();

        yield return new WaitForSeconds(gunController.GetReloadDuration());

        if (objectToDisable != null)
            objectToDisable.SetActive(true);
    }
}
