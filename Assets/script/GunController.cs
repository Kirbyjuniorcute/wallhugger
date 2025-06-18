using UnityEngine;
using UnityEngine.UI;
using System.Collections;







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
    public Text ammoText;

    [Header("Reload Animation")]
    public GameObject[] reloadSteps;

    [Header("Visual Effects")]
    public GameObject muzzleFlash;
    public float flashDuration = 0.05f;

    [Header("Hit Effect")]
    public GameObject hitEffectPrefab;
    public float hitEffectDuration = 1f;
    public float hitEffectScale = 0.5f;

    public AimDownSight adsScript;
    public GameObject normalMuzzleFlash;
    public GameObject adsMuzzleFlash;

    private int currentAmmo;
    private bool isReloading = false;
    private PlayerSlideDuck2 slideScript;
    private Coroutine reloadCoroutine;

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

        // Prevent reloading if sliding
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (slideScript != null && slideScript.IsSliding())
            {
                Debug.Log("Cannot reload while sliding!");
                return; // Don't reload
            }

            if (reloadCoroutine == null) // Prevent double reload
            {
                reloadCoroutine = StartCoroutine(ReloadRoutine());
            }
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

        // Use RaycastAll to detect everything hit
        RaycastHit[] hits = Physics.RaycastAll(ray, shootRange);
        float closestEnemyDistance = float.MaxValue;
        RaycastHit? bestHit = null;
        EnemyAI bestEnemy = null;

        foreach (var hit in hits)
        {
            if (hit.collider.isTrigger)
                continue;

            // Try to find EnemyAI in the object or its parent
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>() ?? hit.collider.GetComponentInParent<EnemyAI>();

            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, hit.point);
                if (distance < closestEnemyDistance)
                {
                    closestEnemyDistance = distance;
                    bestHit = hit;
                    bestEnemy = enemy;
                }
            }
        }

        if (bestEnemy != null && bestHit.HasValue)
        {
            Debug.Log("Enemy hit! Calling TakeDamage on: " + bestEnemy.gameObject.name);
            bestEnemy.TakeDamage(1);

            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, bestHit.Value.point, Quaternion.LookRotation(bestHit.Value.normal));

                Renderer renderer = bestHit.Value.collider.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Vector3 boundsSize = renderer.bounds.size;
                    float averageSize = (boundsSize.x + boundsSize.y + boundsSize.z) / 3f;
                    effect.transform.localScale = Vector3.one * averageSize * hitEffectScale;
                }

                Destroy(effect, hitEffectDuration);
            }

            Rigidbody rb = bestHit.Value.collider.attachedRigidbody;
            if (rb != null)
            {
                rb.AddForce(-bestHit.Value.normal * shootForce);
            }
        }
        else
        {
            Debug.Log("No enemy hit.");
        }
    }


    public IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // Disable slide visuals if sliding
        PlayerSlideDuck2 slideScript = GetComponent<PlayerSlideDuck2>();
        if (slideScript != null && slideScript.IsSliding())
        {
            slideScript.ForceStopSlideVisuals();
        }

        AimDownSight ads = GetComponent<AimDownSight>();
        if (ads != null && ads.IsAiming)
        {
            ads.CancelADS(); // stop aiming first
            yield return new WaitForSeconds(0.2f); // delay to allow ADS animation/transition
        }

        int stepCount = reloadSteps.Length;
        float waitTime = stepCount > 0 ? reloadDuration / stepCount : reloadDuration;

        for (int i = 0; i < stepCount; i++)
        {
            if (!isReloading) yield break;

            SetReloadSpritesActive(false);
            if (i < reloadSteps.Length && reloadSteps[i] != null)
                reloadSteps[i].SetActive(true);

            yield return new WaitForSeconds(waitTime);
        }

        SetReloadSpritesActive(false);

        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        isReloading = false;
        reloadCoroutine = null;
        Debug.Log("Reload complete!");
    }

    public void CancelReload()
    {
        if (isReloading && reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
            isReloading = false;
            SetReloadSpritesActive(false);
            Debug.Log("Reload canceled due to sliding!");
        }
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
        GameObject flashToUse = adsScript != null && adsScript.IsAiming ? adsMuzzleFlash : normalMuzzleFlash;

        if (flashToUse != null)
        {
            flashToUse.SetActive(true);
            yield return new WaitForSeconds(flashDuration);
            flashToUse.SetActive(false);
        }
    }
}
