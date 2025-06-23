using System;
using System.Collections;
using UnityEngine;

public class TimedRandomObjectSpawner : MonoBehaviour
{
    [Header("Spawnable Objects")]
    public GameObject[] spawnObjects; // Assign objects to activate randomly

    [Header("Spawn Points")]
    public Transform[] spawnPoints; // Where to place objects

    [Header("Spawning Settings")]
    public float switchDelay = 5f;        // Time between switches
    public bool autoSwitch = true;        // Toggle for timer mode

    private GameObject currentObject;
    private Coroutine switchCoroutine;

    void Start()
    {
        DeactivateAllObjects();
        SpawnRandomObject();

        if (autoSwitch)
            switchCoroutine = StartCoroutine(AutoSwitchRoutine());
    }

    void OnDisable()
    {
        if (switchCoroutine != null)
            StopCoroutine(switchCoroutine);
    }

    void SpawnRandomObject()
    {
        if (spawnObjects.Length == 0 || spawnPoints.Length == 0) return;

        int objectIndex = UnityEngine.Random.Range(0, spawnObjects.Length);
        int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);


        currentObject = spawnObjects[objectIndex];
        currentObject.transform.position = spawnPoints[spawnIndex].position;
        currentObject.SetActive(true);
    }

    void DeactivateAllObjects()
    {
        foreach (var obj in spawnObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    IEnumerator AutoSwitchRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(switchDelay);

            DeactivateAllObjects();
            SpawnRandomObject();
        }
    }

    // Call this from another script to trigger manual switch
    public void TriggerSwitchNow()
    {
        StopAllCoroutines();
        DeactivateAllObjects();
        SpawnRandomObject();

        if (autoSwitch)
            switchCoroutine = StartCoroutine(AutoSwitchRoutine());
    }
}
