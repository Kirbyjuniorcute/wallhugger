using UnityEngine;

public class EnableOnWave : MonoBehaviour
{
    [Header("Reference to Wave Manager")]
    public EnemySpawner spawner;

    [System.Serializable]
    public class WaveControlledObject
    {
        public GameObject target;

        [Header("Activation Settings")]
        public int enableOnWave = -1;
        public int disableOnWave = -1;

        [Header("Movement Settings")]
        public bool enableMovement = false;
        public int startMoveWave = -1;         // Wave when odd/even movement starts
        public Vector3 moveOffset = Vector3.zero;

        [HideInInspector] public Vector3 originalPosition;
        [HideInInspector] public bool hasEnabled = false;
        [HideInInspector] public bool hasDisabled = false;
    }

    [Header("Wave-Controlled Objects")]
    public WaveControlledObject[] objects;

    void Start()
    {
        foreach (var obj in objects)
        {
            if (obj.target != null)
                obj.originalPosition = obj.target.transform.position;
        }
    }

    void Update()
    {
        if (spawner == null) return;

        int wave = spawner.currentWave;

        foreach (var obj in objects)
        {
            if (obj.target == null) continue;

            // Enable on specific wave
            if (!obj.hasEnabled && obj.enableOnWave > 0 && wave >= obj.enableOnWave)
            {
                obj.target.SetActive(true);
                obj.hasEnabled = true;
            }

            // Disable on specific wave
            if (!obj.hasDisabled && obj.disableOnWave > 0 && wave >= obj.disableOnWave)
            {
                obj.target.SetActive(false);
                obj.hasDisabled = true;
            }

            // Movement based on wave parity
            if (obj.enableMovement && obj.startMoveWave > 0 && wave >= obj.startMoveWave)
            {
                bool isOddWave = (wave % 2 != 0);
                if (isOddWave)
                {
                    obj.target.transform.position = obj.originalPosition + obj.moveOffset;
                }
                else
                {
                    obj.target.transform.position = obj.originalPosition;
                }
            }
        }
    }
}
