using UnityEngine;

public class EnableOnWave : MonoBehaviour
{
    [Header("Wave Activation Settings")]
    public EnemySpawner spawner;               // Reference to your EnemySpawner
    public int activateOnWave = 3;             // Wave to trigger activation
    public GameObject[] objectsToEnable;       // Multiple objects to enable

    private bool hasActivated = false;

    void Update()
    {
        if (!hasActivated && spawner != null && spawner.currentWave >= activateOnWave)
        {
            foreach (GameObject obj in objectsToEnable)
            {
                if (obj != null)
                    obj.SetActive(true);
            }

            hasActivated = true;
        }
    }
}
