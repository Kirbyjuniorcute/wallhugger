using UnityEngine;

[CreateAssetMenu(menuName = "Guns/New Gun")]
public class GunData : ScriptableObject
{
    public GunType gunType;
    public string displayName;

    public int maxAmmo = 10;
    public float shootRange = 100f;
    public float shootForce = 100f;
    public float reloadDuration = 2f;

    public float fireRate = 0.5f; // for automatic guns
    public int damagePerShot = 1;

    public GameObject muzzleFlash;
    public GameObject hitEffect;
}
