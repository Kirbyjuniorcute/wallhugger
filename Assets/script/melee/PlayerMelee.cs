using UnityEngine;
using System.Collections;

public class PlayerMelee : MonoBehaviour
{
    [Header("Melee Settings")]
    public KeyCode meleeKey = KeyCode.F;
    public float meleeDuration = 0.3f;
    public int meleeDamage = 1;
    public string enemyTag = "Enemy"; // Tag used to identify enemies

    [Header("References")]
    public GameObject meleeHitBox; // The cube that checks for hits
    public GameObject[] meleeAnimationSteps; // Multiple sprite frames for the melee animation
    public GameObject[] objectsToDisableDuringMelee; // Objects to disable temporarily during melee

    private bool isMeleeActive = false;

    void Start()
    {
        if (meleeHitBox != null) meleeHitBox.SetActive(false);
        DisableAllMeleeSprites();
    }

    void Update()
    {
        if (Input.GetKeyDown(meleeKey) && !isMeleeActive)
        {
            StartCoroutine(PerformMelee());
        }
    }

    private IEnumerator PerformMelee()
    {
        isMeleeActive = true;

        // Disable extra objects (e.g., UI, weapons, crosshair)
        foreach (GameObject obj in objectsToDisableDuringMelee)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (meleeHitBox != null)
            meleeHitBox.SetActive(true);

        float stepTime = meleeDuration / Mathf.Max(1, meleeAnimationSteps.Length);

        for (int i = 0; i < meleeAnimationSteps.Length; i++)
        {
            DisableAllMeleeSprites();
            if (meleeAnimationSteps[i] != null)
                meleeAnimationSteps[i].SetActive(true);

            if (i == 0 && meleeHitBox != null)
            {
                Collider[] hitColliders = Physics.OverlapBox(
                    meleeHitBox.transform.position,
                    meleeHitBox.transform.localScale / 2f,
                    meleeHitBox.transform.rotation);

                foreach (Collider col in hitColliders)
                {
                    if (col.CompareTag(enemyTag))
                    {
                        EnemyAI enemyAI = col.GetComponent<EnemyAI>();
                        if (enemyAI != null)
                        {
                            enemyAI.TakeDamage(meleeDamage);
                        }
                    }
                }
            }

            yield return new WaitForSeconds(stepTime);
        }

        if (meleeHitBox != null)
            meleeHitBox.SetActive(false);

        DisableAllMeleeSprites();

        // Re-enable disabled objects
        foreach (GameObject obj in objectsToDisableDuringMelee)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        isMeleeActive = false;
    }

    private void DisableAllMeleeSprites()
    {
        foreach (GameObject obj in meleeAnimationSteps)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (meleeHitBox != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(meleeHitBox.transform.position, meleeHitBox.transform.localScale);
        }
    }
}
