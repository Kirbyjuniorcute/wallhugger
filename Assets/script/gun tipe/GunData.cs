using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Guns/Gun Data")]
public class GunData : ScriptableObject
{
    public string displayName;        // Add this line
    public GunType gunType;
    public int maxAmmo = 10;
    public float fireRate = 0.8f;
    public float shootRange = 100f;
    public float shootForce = 100f;
    public float reloadDuration = 2f;
    public int damagePerShot = 1;

    public GameObject muzzleFlash;
    public GameObject hitEffect;
    public float flashDuration = 0.05f;
}
