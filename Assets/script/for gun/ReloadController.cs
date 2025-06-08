using UnityEngine;
using System.Collections;

public class ReloadController : MonoBehaviour
{
    public GunController gunController;    // Assign your GunController here in inspector
    public GameObject objectToDisable;     // The GameObject to disable during reload (e.g. gun model)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !gunController.IsReloading())
        {
            StartCoroutine(ReloadAndToggleObject());
        }
    }

    IEnumerator ReloadAndToggleObject()
    {
        if (objectToDisable != null)
            objectToDisable.SetActive(false);

        // Start the reload coroutine from GunController
        yield return StartCoroutine(gunController.ReloadRoutine());

        if (objectToDisable != null)
            objectToDisable.SetActive(true);
    }
}
