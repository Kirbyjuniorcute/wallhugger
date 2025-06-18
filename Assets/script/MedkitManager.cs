using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedkitManager : MonoBehaviour
{
    [Header("Medkit Settings")]
    public GameObject[] medkitObjects; // Assign spawnable medkits
    public float spawnDelay = 5f; // Delay before respawn

    private GameObject currentMedkit;

    void Start()
    {
        DeactivateAllMedkits();
        SpawnRandomMedkit();
    }

    public void OnMedkitUsed()
    {
        if (currentMedkit != null)
        {
            currentMedkit.SetActive(false);
            currentMedkit = null;
        }
        StartCoroutine(RespawnMedkitAfterDelay());
    }

    IEnumerator RespawnMedkitAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnRandomMedkit();
    }

    void SpawnRandomMedkit()
    {
        if (medkitObjects.Length == 0) return;

        int index = UnityEngine.Random.Range(0, medkitObjects.Length);

        currentMedkit = medkitObjects[index];
        currentMedkit.SetActive(true);
    }

    void DeactivateAllMedkits()
    {
        foreach (GameObject medkit in medkitObjects)
        {
            if (medkit != null) medkit.SetActive(false);
        }
    }
}
