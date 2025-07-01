using UnityEngine;
using UnityEngine.UI;
using System.Collections;




public class GunController : MonoBehaviour
{
    [Header("Gun Data")]
    public GunData currentGun;

    [Header("References")]
    public Camera fpsCamera;
    public Text ammoText;
    public AimDownSight adsScript;
    public GameObject normalMuzzleFlash;
    public GameObject adsMuzzleFlash;

    [Header("Reload Animation")]
    public GameObject[] reloadSteps;

    [Header("Hit Effect")]
    public float hitEffectDuration = 1f;
    public float hitEffectScale = 0.5f;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => currentGun != null ? currentGun.maxAmmo : 0;
    public GunData CurrentGun => currentGun;
    public float GetReloadDuration() => currentGun != null ? currentGun.reloadDuration : 0f;
    public GunSFXPlayer sfxPlayer;

    private int currentAmmo;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;
    private PlayerSlideDuck2 slideScript;
    private Coroutine reloadCoroutine;

    void Start()
    {
        slideScript = GetComponent<PlayerSlideDuck2>();
        LoadGun(currentGun);
        SetReloadSpritesActive(false);
    }

    public void StartReload()
    {
        if (!isReloading && reloadCoroutine == null)
        {
            if (sfxPlayer != null)
            {
                sfxPlayer.PlayReload();
            }

            reloadCoroutine = StartCoroutine(ReloadRoutine());
        }
    }

    public void LoadGun(GunData newGun)
    {
        currentGun = newGun;
        currentAmmo = currentGun.maxAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (slideScript != null && (slideScript.IsSliding() || slideScript.IsDucking()))
            return;
        // Block input during reload, no gun, or while sliding/ducking
        if (isReloading || currentGun == null) return;
       

        if ((currentGun.gunType == GunType.Automatic && Input.GetButton("Fire1")) ||
            (currentGun.gunType != GunType.Automatic && Input.GetButtonDown("Fire1")))
        {
            //  Block shooting when sliding or ducking
            if (slideScript != null && (slideScript.IsSliding() || slideScript.IsDucking()))
                return;

            if (Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + currentGun.fireRate;
                Shoot();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (slideScript != null && slideScript.IsSliding())
            {
                Debug.Log("Cannot reload while sliding!");
                return;
            }

            StartReload();
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
        if (sfxPlayer != null)
        {
            sfxPlayer.PlayFire();
        }
        UpdateAmmoUI();
        StartCoroutine(ShowMuzzleFlash());

        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit[] hits = Physics.RaycastAll(ray, currentGun.shootRange);
        float closestEnemyDistance = float.MaxValue;
        RaycastHit? bestHit = null;
        EnemyAI bestEnemy = null;

        foreach (var hit in hits)
        {
            if (hit.collider.isTrigger)
                continue;

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
            bestEnemy.TakeDamage(currentGun.damagePerShot);
            Debug.Log("Enemy hit! " + bestEnemy.name);

            if (currentGun.hitEffect != null)
            {
                GameObject effect = Instantiate(currentGun.hitEffect, bestHit.Value.point, Quaternion.LookRotation(bestHit.Value.normal));
                Renderer renderer = bestHit.Value.collider.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Vector3 size = renderer.bounds.size;
                    float scale = (size.x + size.y + size.z) / 3f;
                    effect.transform.localScale = Vector3.one * scale * hitEffectScale;
                }
                Destroy(effect, hitEffectDuration);
            }

            Rigidbody rb = bestHit.Value.collider.attachedRigidbody;
            if (rb != null)
            {
                rb.AddForce(-bestHit.Value.normal * currentGun.shootForce);
            }
        
            if (sfxPlayer != null && currentGun.gunType != GunType.Automatic)
            {
                sfxPlayer.StopLoop();
            }

        }
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        if (adsScript != null && adsScript.IsAiming)
        {
            adsScript.CancelADS();
            yield return new WaitForSeconds(0.2f);
        }

        int stepCount = reloadSteps.Length;
        float waitTime = stepCount > 0 ? currentGun.reloadDuration / stepCount : currentGun.reloadDuration;

        for (int i = 0; i < stepCount; i++)
        {
            if (!isReloading) yield break;

            SetReloadSpritesActive(false);
            if (reloadSteps[i] != null)
                reloadSteps[i].SetActive(true);

            yield return new WaitForSeconds(waitTime);
        }

        SetReloadSpritesActive(false);
        currentAmmo = currentGun.maxAmmo;
        UpdateAmmoUI();
        isReloading = false;
        reloadCoroutine = null;
        Debug.Log("Reload complete!");
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"Ammo: {currentAmmo}";
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
            yield return new WaitForSeconds(currentGun.flashDuration);
            flashToUse.SetActive(false);
        }
    }

    public bool IsReloading() => isReloading;

    public void CancelReload()
    {
        if (isReloading && reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
            isReloading = false;
            SetReloadSpritesActive(false);
        }
    }
}
