using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] barriers;
    [SerializeField] private GameObject bumperPrefab;

    [SerializeField] private Transform[] spawnPoint;
    [SerializeField] private Transform[] enterPoint;
    [SerializeField] private float bumperSpawnRate;

    private List<GameObject> spawnedBumper = new List<GameObject>();
    

    public void OpenDoors()
    {
        for (int i = 0; i < barriers.Length; i++)
        {
            float rotation = 0.0f;
            switch (i)
            {
                case 0:
                case 2:
                    rotation = 60.0f;
                    break;
                case 1:
                case 3:
                    rotation = -60.0f;
                    break;
            }

            barriers[i].transform.eulerAngles = new Vector3(barriers[i].transform.eulerAngles.x, rotation, barriers[i].transform.eulerAngles.z);
        }
    }

    public void SpawnBumper()
    {
        int enterPointIndex = Random.Range(0, enterPoint.Length);
        GameObject bumper = Instantiate(bumperPrefab, spawnPoint[enterPointIndex].position, Quaternion.identity, transform);
        bumper.transform.LookAt(new Vector3(enterPoint[enterPointIndex].position.x, bumper.transform.position.y, enterPoint[enterPointIndex].position.z));
        spawnedBumper.Add(bumper);
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject parent = other.transform.parent.gameObject;
        if (parent.CompareTag("Enemy") && spawnedBumper.Contains(parent))
        {
            spawnedBumper.Remove(parent);
        }
    }
}
