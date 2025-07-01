using UnityEngine;
using System.Collections;

public class GunSFXPlayer : MonoBehaviour
{
    [Header("Gun Audio Settings")]
    public AudioSource fireSource;     // AudioSource for firing
    public AudioSource reloadSource;   // AudioSource for reloading
    public bool loopFire = false;

    private bool isFiringLoop = false;

    public void PlayFire()
    {
        if (IsGamePaused() || fireSource == null || fireSource.clip == null)
            return;

        if (loopFire)
        {
            if (!isFiringLoop)
            {
                fireSource.loop = true;
                fireSource.Play();
                isFiringLoop = true;
            }
        }
        else
        {
            fireSource.PlayOneShot(fireSource.clip);
        }
    }

    public void StopLoop()
    {
        if (loopFire && isFiringLoop && fireSource != null)
        {
            fireSource.Stop();
            fireSource.loop = false;
            isFiringLoop = false;
        }
    }

    public void PlayReload()
    {
        if (!IsGamePaused() && reloadSource != null && reloadSource.clip != null)
        {
            reloadSource.PlayOneShot(reloadSource.clip);
        }
    }

    private bool IsGamePaused()
    {
        return Time.timeScale == 0;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) PlayFire();
        if (Input.GetKeyDown(KeyCode.Alpha2)) PlayReload();
    }

}
