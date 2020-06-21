using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cowsPrefab;
    [SerializeField] private Transform[] spawnPoints;
    private List<GameObject> spawnedCows = new List<GameObject>();


    public void SpawnCows()
    {
        if (spawnedCows.Count < spawnPoints.Length)
        {
            GameObject cow = Instantiate(cowsPrefab, spawnPoints[spawnedCows.Count].position, Quaternion.identity, spawnPoints[spawnedCows.Count]);
            spawnedCows.Add(cow);
        }
    }

    public void Intro()
    {
        GameObject cow = Instantiate(cowsPrefab, spawnPoints[0].position, Quaternion.identity, spawnPoints[0]);
        spawnedCows.Add(cow);
    }

}
