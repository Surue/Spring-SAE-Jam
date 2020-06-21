using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
public class PlayerController : MonoBehaviour
{
    private CarMovement carMovement;
    private Rigidbody rigidbody;
    private GameUIManager gameUiManager;
    private Material material;
    private CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] private float initScreenAmplitudeGain = 10f;
    [SerializeField] private float initScreenFrequencyGain = 10f;
    [SerializeField] private float initScreenshakeDuration = 1f;
    [SerializeField] private float initCarAmplitudeGain = 5f;
    [SerializeField] private float initCarFrequencyGain = 50f;
    [SerializeField] private float initCarshakeDuration = 1f;
    private float currentCarAmplitudeGain = 5f;
    private float currentCarFrequencyGain = 50f;

    [SerializeField] private float maxLife = 100.0f;
    private float currentLife;

    [SerializeField] private float velocityBeforeDamage = 10f;
    [SerializeField] private float damageRatio = 0.1f;
    [SerializeField] private float coolDown = 0.1f;
    private float coolDownTimer = 0.0f;
    private int nbScreenShake = 0;
    private int nbCarShake = 0;
    private float carShakeTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        carMovement = GetComponent<CarMovement>();
        rigidbody = GetComponent<Rigidbody>();
        gameUiManager = FindObjectOfType<UIManager>().GameUiManager;
        vcam = FindObjectOfType<CinemachineVirtualCamera>();
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        material = GetComponentInChildren<MeshRenderer>().material;
        currentLife = maxLife;
        gameUiManager.DisplayLife(maxLife, currentLife);
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
                carShakeTimer += Time.deltaTime * currentCarFrequencyGain;
                transform.Rotate(Vector3.forward, Mathf.Sin(carShakeTimer) * currentCarAmplitudeGain);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                carMovement.Movement(rotationInput, speedInput);
            }
            gameUiManager.DisplaySpeed(carMovement.CurrentSpeed / carMovement.MaxSpeed);
            gameUiManager.DisplayLife(maxLife, Mathf.Abs(currentLife));
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

    public void ScreenShake(float screenAmplitudeGain = 0.0f, float screenFrequencyGain = 0.0f, float screenshakeDuration = 0.0f)
    {
        nbScreenShake++;
        if (screenAmplitudeGain == 0.0f)
        {
            noise.m_AmplitudeGain = initScreenAmplitudeGain;
        }
        else
        {
            noise.m_AmplitudeGain = screenAmplitudeGain;
        }

        if (screenFrequencyGain == 0.0f)
        {
            noise.m_FrequencyGain = initScreenFrequencyGain;
        }
        else
        {
            noise.m_FrequencyGain = screenFrequencyGain;
        }

        if (screenshakeDuration == 0.0f)
        {
            StartCoroutine(StopScreenShake(initScreenshakeDuration));
        }
        else
        {
            StartCoroutine(StopScreenShake(screenshakeDuration));
        }
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

    public void CarShake(float carAmplitudeGain, float carFrequencyGain, float carShakeDuration)
    {
        nbCarShake++; 
        if (carAmplitudeGain == 0.0f)
        {
            currentCarAmplitudeGain = initCarAmplitudeGain;
        } else
        {
            currentCarAmplitudeGain = carAmplitudeGain;
        }

        if (carFrequencyGain == 0.0f)
        {
            currentCarFrequencyGain = initCarFrequencyGain;
        } else
        {
            currentCarFrequencyGain = carFrequencyGain;
        }

        if (carShakeDuration == 0.0f)
        {
            StartCoroutine(StopCarShake(initScreenshakeDuration));
        }
        else
        {
            StartCoroutine(StopCarShake(carShakeDuration));
        }
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
