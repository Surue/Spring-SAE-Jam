using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
public class PlayerController : MonoBehaviour
{
    private CarMovement carMovement;
    private Rigidbody rigidbody;
    private Material material;

    [SerializeField] private float maxLife = 100.0f;
    private float currentLife;

    [SerializeField] private float velocityBeforeDamage = 10f;
    [SerializeField] private float damageRatio = 0.1f;
    [SerializeField] private float coolDown = 0.1f;
    private float coolDownTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        carMovement = GetComponent<CarMovement>();
        rigidbody = GetComponent<Rigidbody>();
        material = GetComponentInChildren<MeshRenderer>().material;


        currentLife = maxLife;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float rotationInput = Input.GetAxis("Horizontal");
        float speedInput = Input.GetAxis("Vertical");
        carMovement.Movement(rotationInput, speedInput);
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
        material.SetColor("_Color", Color.Lerp(Color.red, Color.white, currentLife/maxLife));
    }

    void GameOver()
    {
        Debug.Log("Game Over");
    }

}
