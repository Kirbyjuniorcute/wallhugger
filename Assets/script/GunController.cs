using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;


public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    public int maxAmmo = 10;
    public float shootRange = 100f;
    public float shootForce = 100f;
    public float reloadDuration = 2f;

    [Header("References")]
    public Camera fpsCamera;

    [Header("UI")]
    public UnityEngine.UI.Text ammoText;


    [Header("Reload Animation")]
    public GameObject[] reloadSteps;

    [Header("Visual Effects")]
    public GameObject muzzleFlash;
    public float flashDuration = 0.05f;

    [Header("Hit Effect")]
    public GameObject hitEffectPrefab; // Drag your splash/hit VFX here
    public float hitEffectDuration = 1f; // How long it stays before being destroyed
    [Tooltip("Multiplier for hit effect scale. 1 = normal size")]
    public float hitEffectScale = 0.5f;

    public AimDownSight adsScript;              // Reference to AimDownSight
    public GameObject normalMuzzleFlash;
    public GameObject adsMuzzleFlash;


    private int currentAmmo;
    private bool isReloading = false;
    private PlayerSlideDuck2 slideScript;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        SetReloadSpritesActive(false);
        slideScript = GetComponent<PlayerSlideDuck2>();
    }

    void Update()
    {
        if (isReloading)
            return;

        // Block shooting if sliding or ducking
        if (slideScript != null && (slideScript.IsSliding() || slideScript.IsDucking()))
            return;

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadRoutine());
        }
    }



    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo! Reload.");
            return;
        }

        currentAmmo--;
        UpdateAmmoUI();
        StartCoroutine(ShowMuzzleFlash());

        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, shootRange))
        {
            Debug.Log("Hit: " + hit.collider.name);

            // Apply force if there's a rigidbody
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(-hit.normal * shootForce);
            }

            // Damage enemy
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }

            // Spawn hit effect scaled to target object
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));

                // Inside Shoot(), after instantiating the effect
                Renderer renderer = hit.collider.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Vector3 boundsSize = renderer.bounds.size;
                    float averageSize = (boundsSize.x + boundsSize.y + boundsSize.z) / 3f;

                    effect.transform.localScale = Vector3.one * averageSize * hitEffectScale;
                }


                Destroy(effect, hitEffectDuration);
            }

        }
    }

    public IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        int stepCount = reloadSteps.Length;
        float waitTime = stepCount > 0 ? reloadDuration / stepCount : reloadDuration;

        for (int i = 0; i < stepCount; i++)
        {
            SetReloadSpritesActive(false);
            reloadSteps[i].SetActive(true);
            yield return new WaitForSeconds(waitTime);
        }

        SetReloadSpritesActive(false);

        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        isReloading = false;
        Debug.Log("Reload complete!");
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = "Ammo: " + currentAmmo;
    }

    void SetReloadSpritesActive(bool active)
    {
        foreach (GameObject step in reloadSteps)
        {
            if (step != null)
                step.SetActive(active);
        }
    }

    IEnumerator ShowMuzzleFlash()
    {
        GameObject flashToUse = null;

        if (adsScript != null && adsScript.IsAiming)
        {
            flashToUse = adsMuzzleFlash;
        }
        else
        {
            flashToUse = normalMuzzleFlash;
        }

        if (flashToUse != null)
        {
            flashToUse.SetActive(true);
            yield return new WaitForSeconds(flashDuration);
            flashToUse.SetActive(false);
        }
    }

}
