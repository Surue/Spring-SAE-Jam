using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] gunnerPrefab;

    [SerializeField] private Transform[] spawnPoint;
    [SerializeField] private Transform[] enterPoint;
    [SerializeField] private float gunnerHeightJump;
    [SerializeField] private float gunnerWidthJump;
    [SerializeField] private float gunnerSpeed;
    [SerializeField] private float gunnerSpawnRate;

    private List<GameObject> spawnedGunner = new List<GameObject>();

    public void SpawnGunner()
    {
        int enterPointIndex = Random.Range(0, enterPoint.Length);
        GameObject gunner = Instantiate(gunnerPrefab[Random.Range(0, gunnerPrefab.Length)], spawnPoint[enterPointIndex].position, Quaternion.identity, transform);
        gunner.transform.LookAt(new Vector3(enterPoint[enterPointIndex].position.x, gunner.transform.position.y, enterPoint[enterPointIndex].position.z));
        gunner.GetComponent<Gunner>().SetEnterPoint(enterPoint[enterPointIndex]);
        spawnedGunner.Add(gunner);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent == null) return;
        
        GameObject parent = other.transform.parent.gameObject;
        if (parent.CompareTag("Enemy") && spawnedGunner.Contains(parent))
        {
            parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            parent.GetComponent<Rigidbody>().AddForce(parent.transform.forward*gunnerWidthJump+ Vector3.up * gunnerHeightJump);
            spawnedGunner.Remove(parent);
        }
    }
}
