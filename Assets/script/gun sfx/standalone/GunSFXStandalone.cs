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

        if (loopFire)
        {
            if (Input.GetButtonDown("Fire1") && gunController.CurrentAmmo > 0 && !isFiringLoop)
            {
                PlayLoopFire();
            }
            if (Input.GetButtonUp("Fire1") && isFiringLoop)
            {
                StopLoopFire();
            }

            // Stop firing if ammo hits 0 while firing
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
            PlayReload();
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
