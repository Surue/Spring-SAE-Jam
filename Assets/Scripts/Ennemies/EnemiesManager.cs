using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{

    private float timeSinceStart = 0.0f;

    [Header("Gunner")]
    [SerializeField] private float timeBeforeGunners;
    [SerializeField] private float gunnerStartSpawnRate;
    [SerializeField] private float increaseGunnerSpawnRate;
    private GunnerSpawner gunnerSpawner;
    private float gunnerTimer = 0.0f;

    [Header("Bumper")]
    [SerializeField] private float timeBeforeBumpers;
    [SerializeField] private float bumperStartSpawnRate;
    [SerializeField] private float increaseBumperSpawnRate;
    private BumperSpawner bumperSpawner;
    private float bumperTimer = 0.0f;
    private bool bumperDoorsOpens = false;

    [Header("MonsterTruck")]
    [SerializeField] private float timeBeforeMonsterTrucks;
    [SerializeField] private float monsterTruckStartSpawnRate;
    [SerializeField] private float increaseMonsterTruckSpawnRate;
    private MonsterTruckSpawner monsterTruckSpawner;
    private float monsterTruckTimer = 0.0f;
    private bool monsterTrucksDoorsOpen = false;

    [Header("Cow")]
    [SerializeField] private float timeBeforeCows;
    [SerializeField] private float cowStartSpawnRate;
    [SerializeField] private float increaseCowSpawnRate;
    private CowSpawner cowSpawner;
    private float cowTimer = 0.0f;
    private bool cowsSpawned = false;

    void Start()
    {
        bumperSpawner = FindObjectOfType<BumperSpawner>();
        gunnerSpawner = FindObjectOfType<GunnerSpawner>();
        cowSpawner = FindObjectOfType<CowSpawner>();
        monsterTruckSpawner = FindObjectOfType<MonsterTruckSpawner>();
    }

    void Update()
    {
        timeSinceStart += Time.deltaTime;
        if (timeSinceStart > timeBeforeGunners)
        {
            gunnerTimer += Time.deltaTime;
            gunnerStartSpawnRate += increaseGunnerSpawnRate * Time.deltaTime;
            if (gunnerTimer > 1 / gunnerStartSpawnRate)
            {
                gunnerSpawner.SpawnGunner();
                gunnerTimer = 0.0f;
            }
        }
        if (timeSinceStart > timeBeforeBumpers)
        {
            if (bumperDoorsOpens)
            {
                bumperTimer += Time.deltaTime;
                bumperStartSpawnRate += increaseBumperSpawnRate * Time.deltaTime;
                if (bumperTimer > 1 / bumperStartSpawnRate)
                {
                    bumperSpawner.SpawnBumper();
                    bumperTimer = 0.0f;
                }
            }
            else
            {
                bumperSpawner.OpenDoors();
                bumperDoorsOpens = true;
            }
        }
        if (timeSinceStart > timeBeforeMonsterTrucks)
        {
            if (monsterTrucksDoorsOpen)
            {
                monsterTruckSpawner.OpenDoors();
                monsterTruckTimer += Time.deltaTime;
                monsterTruckStartSpawnRate += increaseMonsterTruckSpawnRate * Time.deltaTime;
                if (monsterTruckTimer > 1 / monsterTruckStartSpawnRate)
                {
                    monsterTruckSpawner.SpawnMonsterTruck();
                    monsterTruckTimer = 0.0f;
                }
            }
            else
            {
                monsterTruckSpawner.OpenDoors();
                monsterTrucksDoorsOpen = true;
            }
        }
        if (timeSinceStart > timeBeforeCows)
        {
            if (cowsSpawned)
            {
                cowTimer += Time.deltaTime;
                cowStartSpawnRate += increaseCowSpawnRate * Time.deltaTime;
                if (cowTimer > 1 / cowStartSpawnRate)
                {
                    cowSpawner.SpawnCows();
                    cowTimer = 0.0f;
                }
            }
            else
            {
                cowSpawner.Intro();
                cowsSpawned = true;
            }
        }
    }
}
