using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesSpawner : MonoBehaviour
{
    [SerializeField] private GameObject tntPrefab;
    [SerializeField] private int maxTnts;
    private List<GameObject> tntGameObjects = new List<GameObject>();
    [SerializeField] private GameObject tirePrefab;
    [SerializeField] private int maxTires;
    private List<GameObject> tireGameObjects = new List<GameObject>();
    [SerializeField] private GameObject canPrefab;
    [SerializeField] private int maxCans;
    private List<GameObject> canGameObjects = new List<GameObject>();
    [SerializeField] private float spawnDelay;
    private float timer;


    [SerializeField] private Vector2 extend;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.GAME)
        {
            timer += Time.deltaTime;
            if (timer > spawnDelay)
            {
                if (tntGameObjects.Count < maxTnts)
                {
                    GameObject tnt = Instantiate(tntPrefab, transform.position + new Vector3(Random.Range(-extend.x, extend.x), 0, Random.Range(-extend.y, extend.y)), Quaternion.identity, transform);
                    tntGameObjects.Add(tnt);
                }
                if (tireGameObjects.Count < maxTires)
                {
                    GameObject tire = Instantiate(tirePrefab, transform.position + new Vector3(Random.Range(-extend.x, extend.x), 0, Random.Range(-extend.y, extend.y)), Quaternion.identity, transform);
                    tireGameObjects.Add(tire);
                }
                if (canGameObjects.Count < maxCans)
                {
                    GameObject can = Instantiate(canPrefab, transform.position + new Vector3(Random.Range(-extend.x, extend.x), 0, Random.Range(-extend.y, extend.y)), Quaternion.identity, transform);
                    canGameObjects.Add(can);
                }
                timer = 0.0f;
            }
        }
    }

    public void Remove(GameObject gameObject)
    {
        if (tntGameObjects.Contains(gameObject))
        {
            tntGameObjects.Remove(gameObject);
        }
        if (tireGameObjects.Contains(gameObject))
        {
            tireGameObjects.Remove(gameObject);
        }
        if (canGameObjects.Contains(gameObject))
        {
            canGameObjects.Remove(gameObject);
        }
    }
}
