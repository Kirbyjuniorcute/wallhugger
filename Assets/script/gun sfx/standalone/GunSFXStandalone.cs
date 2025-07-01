using UnityEngine;
using System.Collections;

public class GunSFXStandalone : MonoBehaviour
{
    [Header("SFX GameObjects")]
    public GameObject fireSFXObject;    // With AudioSource (PlayOnAwake = false)
    public GameObject reloadSFXObject;  // With AudioSource (PlayOnAwake = false)

    [Header("Weapon Mode")]
    public bool loopFire = false; // Set true for AR / auto

    [Header("References")]
    public GunController gunController; // Assign in Inspector

    private bool isFiringLoop = false;

    void Update()
    {
        if (Time.timeScale == 0 || gunController == null) return;

        PlayerSlideDuck2 slideScript = gunController.GetComponent<PlayerSlideDuck2>();
        if (slideScript != null && (slideScript.IsSliding() || slideScript.IsDucking()))
            return;

        if (loopFire)
        {
            if (Input.GetButtonDown("Fire1") && !isFiringLoop)
            {
                if (gunController.CurrentAmmo > 0)
                {
                    PlayLoopFire();
                }
            }

            if (Input.GetButtonUp("Fire1") && isFiringLoop)
            {
                StopLoopFire();
            }

            // EXTRA: If player is holding the button but runs out of ammo mid-loop, stop the loop
            if (isFiringLoop && gunController.CurrentAmmo <= 0)
            {
                StopLoopFire();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && gunController.CurrentAmmo > 0)
            {
                PlaySingleFire();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayReload(); // Reload still allowed
        }
    }



    void PlaySingleFire()
    {
        if (fireSFXObject != null)
        {
            fireSFXObject.SetActive(true);
            StartCoroutine(DisableAfterClip(fireSFXObject));
        }
    }

    void PlayLoopFire()
    {
        if (fireSFXObject != null)
        {
            fireSFXObject.SetActive(true);
            AudioSource src = fireSFXObject.GetComponent<AudioSource>();
            if (src != null)
            {
                src.loop = true;
                src.Play();
                isFiringLoop = true;
            }
        }
    }

    void StopLoopFire()
    {
        if (fireSFXObject != null)
        {
            AudioSource src = fireSFXObject.GetComponent<AudioSource>();
            if (src != null)
            {
                src.Stop();
                src.loop = false;
            }
            fireSFXObject.SetActive(false);
            isFiringLoop = false;
        }
    }

    void PlayReload()
    {
        if (reloadSFXObject != null)
        {
            reloadSFXObject.SetActive(true);
            StartCoroutine(DisableAfterClip(reloadSFXObject));
        }
    }

    IEnumerator DisableAfterClip(GameObject sfxObject)
    {
        AudioSource src = sfxObject.GetComponent<AudioSource>();
        if (src != null && src.clip != null)
        {
            yield return new WaitForSeconds(src.clip.length);
            sfxObject.SetActive(false);
        }
    }
}
