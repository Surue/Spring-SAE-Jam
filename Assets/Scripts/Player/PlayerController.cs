using System.Collections;
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
    [SerializeField] private float amplitudeGain = 10f;
    [SerializeField] private float frequencyGain = 10f;
    [SerializeField] private float screenshakeDuration = 1f;
    private int nbScreenShake = 0;

    // Start is called before the first frame update
    void Start()
    {
        carMovement = GetComponent<CarMovement>();
        rigidbody = GetComponent<Rigidbody>();
        uiManager = FindObjectOfType<UIManager>();
        vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        material = GetComponentInChildren<MeshRenderer>().material;


        currentLife = maxLife;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float rotationInput = Input.GetAxis("Horizontal");
        float speedInput = Input.GetAxis("Vertical");
        carMovement.Movement(rotationInput, speedInput);
        uiManager.DisplaySpeed(carMovement.CurrentSpeed/carMovement.MaxSpeed);
    }

    void Update()
    {
        coolDownTimer += Time.deltaTime;
        if (coolDownTimer > coolDown)
        {
            coolDownTimer = 0.0f;
            float currentVelocity = rigidbody.velocity.magnitude;
            if (currentVelocity < velocityBeforeDamage)
            {
                Hit((velocityBeforeDamage - currentVelocity)*damageRatio);
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
        Debug.Log("Game Over");
    }

    public void ScreenShake()
    {
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
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

}
