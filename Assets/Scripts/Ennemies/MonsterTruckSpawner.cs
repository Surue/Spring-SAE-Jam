using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTruckSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] barriers;
    [SerializeField] private GameObject monsterTruckPrefab;

    [SerializeField] private Transform[] spawnPoint;
    [SerializeField] private Transform[] enterPoint;
    [SerializeField] private float monsterTruckSpawnRate;

    private List<GameObject> spawnedMonsterTruck = new List<GameObject>();

    public void OpenDoors()
    {
        for (int i = 0; i < barriers.Length; i++)
        {
            float rotation = 0.0f;
            switch (i)
            {
                case 0:
                    rotation = -120.0f;
                    break;
                case 1:
                    rotation = 120.0f;
                    break;
            }

            barriers[i].transform.eulerAngles = new Vector3(barriers[i].transform.eulerAngles.x, rotation, barriers[i].transform.eulerAngles.z);
        }
    }

    public void SpawnMonsterTruck()
    {
        int enterPointIndex = Random.Range(0, enterPoint.Length);
        GameObject monsterTruck = Instantiate(monsterTruckPrefab, spawnPoint[enterPointIndex].position, Quaternion.identity, transform);
        monsterTruck.transform.LookAt(new Vector3(enterPoint[enterPointIndex].position.x, monsterTruck.transform.position.y, enterPoint[enterPointIndex].position.z));
        spawnedMonsterTruck.Add(monsterTruck);
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject parent = other.transform.parent.gameObject;
        if (parent.CompareTag("Enemy") && spawnedMonsterTruck.Contains(parent))
        {
            spawnedMonsterTruck.Remove(parent);
        }
    }
}
