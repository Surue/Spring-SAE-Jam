﻿using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
public class PlayerController : MonoBehaviour
{
    private CarMovement carMovement;
    private Rigidbody rigidbody;
    private UIManager uiManager;
    private Material material;
    private CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin noise;

    [SerializeField] private float maxLife = 100.0f;
    private float currentLife;

    [SerializeField] private float velocityBeforeDamage = 10f;
    [SerializeField] private float damageRatio = 0.1f;
    [SerializeField] private float coolDown = 0.1f;
    private float coolDownTimer = 0.0f;
    [SerializeField] private float screenAmplitudeGain = 10f;
    [SerializeField] private float screenFrequencyGain = 10f;
    [SerializeField] private float carAmplitudeGain = 5f;
    [SerializeField] private float carFrequencyGain = 50f;
    [SerializeField] private float screenshakeDuration = 1f;
    [SerializeField] private float carshakeDuration = 1f;
    private int nbScreenShake = 0;
    private int nbCarShake = 0;
    private float carShakeTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        carMovement = GetComponent<CarMovement>();
        rigidbody = GetComponent<Rigidbody>();
        uiManager = FindObjectOfType<UIManager>();
        vcam = FindObjectOfType<CinemachineVirtualCamera>();
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        material = GetComponentInChildren<MeshRenderer>().material;


        currentLife = maxLife;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.GAME)
        {
            float rotationInput = Input.GetAxis("Horizontal");
            float speedInput = Input.GetAxis("Vertical");
            if (nbCarShake > 0)
            {
                carShakeTimer += Time.deltaTime * carFrequencyGain;
                transform.Rotate(Vector3.forward, Mathf.Sin(carShakeTimer) * carAmplitudeGain);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                carMovement.Movement(rotationInput, speedInput);
            }
            uiManager.DisplaySpeed(carMovement.CurrentSpeed / carMovement.MaxSpeed);
        }
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.GAME)
        {
            coolDownTimer += Time.deltaTime;
            if (coolDownTimer > coolDown)
            {
                coolDownTimer = 0.0f;
                float currentVelocity = rigidbody.velocity.magnitude;
                if (currentVelocity < velocityBeforeDamage)
                {
                    Hit((velocityBeforeDamage - currentVelocity) * damageRatio);
                }
            }
        }
    }

    public void Hit(float damage)
    {
        currentLife -= damage;
        if (currentLife < 0.0f)
        {
            GameOver();
        }
        //Test
        material.SetColor("_Color", Color.Lerp(Color.red, Color.white, currentLife/maxLife));
    }

    void GameOver()
    {
        GameManager.Instance.End(false);
    }

    public void ScreenShake()
    {
        noise.m_AmplitudeGain = screenAmplitudeGain;
        noise.m_FrequencyGain = screenFrequencyGain;
        nbScreenShake++;
        StartCoroutine(StopScreenShake(screenshakeDuration));
    }

    IEnumerator StopScreenShake(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (nbScreenShake == 1)
        {
            noise.m_AmplitudeGain = 0;
            noise.m_FrequencyGain = 0;
        }
        nbScreenShake--;
    }

    public void CarShake()
    {
        nbCarShake++;
        StartCoroutine(StopCarShake(carshakeDuration));
    }

    IEnumerator StopCarShake(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (nbCarShake == 1)
        {
            noise.m_AmplitudeGain = 0;
            noise.m_FrequencyGain = 0;
        }
        nbCarShake--;
    }

}
