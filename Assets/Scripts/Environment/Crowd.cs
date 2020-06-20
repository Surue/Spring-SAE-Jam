using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowd : MonoBehaviour
{
    const int nbBleach = 10;
    const int maxPeopleColums = nbBleach * 50;
    [SerializeField] private GameObject[] crowdPrefab;
    [SerializeField] private Transform[] blechersPos;
    [SerializeField] private Transform[] relativeStepPos;
    [SerializeField] private int peoplePerStep = 10;
    private List<GameObject>[] prefab = new List<GameObject>[maxPeopleColums];
    private List<Vector3>[] prefabPosition = new List<Vector3>[maxPeopleColums];
    [SerializeField] private float waveSpeed = 5.0f;
    [SerializeField] private float jumpSpeed = 5.0f;
    [SerializeField] private float jumpHeight = 5.0f;
    [SerializeField] private int nbWaves = 2;
    private float currentWavePos = 0;
    [SerializeField] private bool waveLaunch = false;
    // Start is called before the first frame update
    void Start()
    {
        int rowsPos = 0;
        for (int b = 0; b < blechersPos.Length; b++) {
            for (float i = 0; i < peoplePerStep; i++)
            {
                if (prefab[Mathf.FloorToInt(rowsPos * peoplePerStep + i)] == null)
                {
                    prefab[Mathf.FloorToInt(rowsPos * peoplePerStep + i)] = new List<GameObject>();
                    prefabPosition[Mathf.FloorToInt(rowsPos * peoplePerStep + i)] = new List<Vector3>();
                }
                for (int j = 0; j < relativeStepPos.Length; j++)
                {
                    float stepShifting = (j % 2.0f)/2.0f + 0.25f;
                    Vector3 vector = new Vector3(Mathf.Lerp(-relativeStepPos[j].localPosition.x, relativeStepPos[j].localPosition.x, (i +stepShifting)/ peoplePerStep), relativeStepPos[j].localPosition.y, relativeStepPos[j].localPosition.z);
                    vector = blechersPos[b].transform.rotation * vector;
                    vector += blechersPos[b].position;
                    GameObject people = Instantiate(crowdPrefab[Random.Range(0, crowdPrefab.Length)], vector, blechersPos[b].transform.rotation * Quaternion.Euler(0, 180, 0), blechersPos[b].transform);
                    if (b % 8 == 0 || b % 8 == 7 || b % 2 == 0)
                    {
                        prefab[Mathf.FloorToInt(rowsPos * peoplePerStep + i)].Add(people);
                        prefabPosition[Mathf.FloorToInt(rowsPos * peoplePerStep + i)].Add(vector);
                    } else
                    {
                        prefab[Mathf.FloorToInt(rowsPos * peoplePerStep + i)].Add(people);
                        prefabPosition[Mathf.FloorToInt(rowsPos * peoplePerStep + i)].Add(vector);
                    }
                }
            }
            if (b % 8 == 0 || b % 8 == 7 || b % 2 == 0)
            {
                rowsPos++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (waveLaunch)
        {
            for (int i = 0; i < nbBleach * peoplePerStep; i++)
            {
                for (int j = 0; j < prefab[i].Count; j++)
                {
                    if (i > currentWavePos - Mathf.PI * jumpSpeed && i < currentWavePos + Mathf.PI * jumpSpeed * nbWaves * 2 - Mathf.PI * jumpSpeed)
                    {
                        prefab[i][j].transform.position = prefabPosition[i][j] + Vector3.up * (Mathf.Cos((currentWavePos - i) / jumpSpeed) + 1) * jumpHeight;
                    }
                    else
                    {
                        prefab[i][j].transform.position = prefabPosition[i][j];
                    }
                }
            }
            currentWavePos += Time.deltaTime * waveSpeed;
            if (currentWavePos > peoplePerStep * 10)
            {
                currentWavePos = -Mathf.PI * jumpSpeed * nbWaves * 2 + Mathf.PI * jumpSpeed;
                //waveLaunch = false;
            }
        } else
        {
            for (int i = 0; i < nbBleach * peoplePerStep; i++)
            {
                for (int j = 0; j < prefab[i].Count; j++)
                {
                    prefab[i][j].transform.position = prefabPosition[i][j] + Vector3.up * (Random.value > 0.5 ? 0 : jumpHeight);
                }
            }
        }
    }

    public void LaunchWave()
    {
        waveLaunch = true;
    }
}
