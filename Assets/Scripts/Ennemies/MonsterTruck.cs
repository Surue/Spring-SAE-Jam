using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonsterTruck : MonoBehaviour
{
    [Header("Idle state")] 
    [SerializeField] private float idleTimer = 1.0f;

    [Header("Anger")] 
    [SerializeField] private List<ParticleSystem> flameParticles;
    [SerializeField] private float angerMaxSpeed = 50.0f;
    [SerializeField] private float angerSpeed = 10.0f;
    [SerializeField] private float angerTime = 5.0f;
    [SerializeField] private float angerTimeFactor = 2.0f;
    
    [Header("Death")] 
    [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private float dyingTime = 3.0f;
    [SerializeField] private List<GameObject> smallParts;
    
    private Transform player;

    private float currentTimer = 0;
    
    //Life
    [Header("Life")]
    [SerializeField] private int lifePoint = 3;
    [SerializeField] private List<GameObject> lifeBag;
    
    //Movement
    private Vector2 movementVector;
    
    private CarMovement carMovement;

    [Header("RayCast")]
    [SerializeField] private float distanceRayCast = 10f;
    [SerializeField] private LayerMask layerMaskRayCast;

    [Header("Audio")] 
    [SerializeField] private AudioSource audioSource;

    [Header("Colliders")] 
    [SerializeField] private Collider carCollider;
    [SerializeField] private Collider bumperCollider;
    
    enum State {
        PAUSE,
        IDLE, //Basic states to wait a few seconds before starting to roll again
        FOLLOW_PLAYER,
        DAMAGE_TAKEN,
        ANGRY,
        DYING //State to wait final destroy
    }

    private State state_ = State.IDLE;
    private State previousState_ = State.IDLE;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;

        carMovement = GetComponent<CarMovement>();

        if (GameManager.Instance.CurrentState == GameManager.GameState.START)
        {
            previousState_ = state_;
            state_ = State.PAUSE;
        }

        foreach (var flameParticle in flameParticles)
        {
            flameParticle.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer += Time.deltaTime;

        movementVector = Vector2.zero;

        switch (state_) {
            case State.PAUSE:
                if (GameManager.Instance.CurrentState != GameManager.GameState.START)
                {
                    state_ = previousState_;
                }
                break;
            case State.IDLE:
                //Transition to Move to position
                if (currentTimer > idleTimer)
                {
                    currentTimer = 0.0f;
                    state_ = State.FOLLOW_PLAYER;
                }
                break;
            case State.FOLLOW_PLAYER:
            {
                carMovement.SetMaxSpeed(20);
                Vector3 dir = (player.position - transform.position).normalized;

                Vector2 force = new Vector2(dir.x, dir.z) * 5.0f;

                movementVector += force;

                EvaluateObstacleInFront();
            }
                break;
            case State.DAMAGE_TAKEN:
                if (currentTimer > 1.0f)
                {
                    currentTimer = 0;
                    state_ = State.ANGRY;
                    
                    foreach (var flameParticle in flameParticles)
                    {
                        flameParticle.Play();
                    }
                }
                break;
            case State.ANGRY:
            {
                carMovement.SetMaxSpeed(angerMaxSpeed);
                
                Vector3 dir = (player.position - transform.position).normalized;

                Vector2 force = new Vector2(dir.x, dir.z) * angerSpeed;

                movementVector += force;
                
                if (currentTimer > angerTime)
                {
                    currentTimer = 0;
                    state_ = State.FOLLOW_PLAYER;
                    
                    foreach (var flameParticle in flameParticles)
                    {
                        flameParticle.Stop();
                    }

                    angerTime *= angerTimeFactor;
                }
            }
                break;
            case State.DYING:
                if (currentTimer > dyingTime)
                {
                    Destroy(gameObject);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void FixedUpdate()
    {
        if (state_ == State.DYING) return;

        float movementSpeed = movementVector.magnitude;
        float movementAngle = Vector2.SignedAngle(new Vector2(movementVector.x, movementVector.y),
                new Vector2(transform.forward.x, transform.forward.z));
            
        carMovement.Movement(movementAngle, movementSpeed);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        //If hit player with bumper 
        if (other.GetContact(0).thisCollider == bumperCollider && other.gameObject.CompareTag("Player") && state_ != State.DYING)
        {
            state_ = State.IDLE;
        }
        
        //If hit player with the car collider => lose life point
        if (other.GetContact(0).thisCollider == carCollider && other.gameObject.CompareTag("Player") && state_ != State.DYING)
        {
            //If angry of has juste taken damage, then don't take damage
            if (state_ == State.DAMAGE_TAKEN || state_ == State.ANGRY) return;
            lifePoint -= 1;

            Destroy(lifeBag[0]);
            lifeBag.RemoveAt(0);

            currentTimer = 0;
            state_ = State.DAMAGE_TAKEN;

            if (lifePoint <= 0)
            {
                other.gameObject.GetComponent<PlayerController>().ScreenShake();

                //Kill bumper
                explosionParticleSystem.Play();

                state_ = State.DYING;

                currentTimer = 0.0f;

                Destroy(carMovement);
                Rigidbody body = GetComponent<Rigidbody>();
                body.velocity = (transform.position - other.GetContact(0).point).normalized *
                                other.rigidbody.velocity.magnitude;
                body.constraints = RigidbodyConstraints.None;

                foreach (var smallPart in smallParts)
                {
                    smallPart.GetComponent<Collider>().enabled = true;
                    smallPart.AddComponent<Rigidbody>();
                }

                Destroy(carCollider);

                audioSource.Play();
            }
        }
    }
    
    void EvaluateObstacleInFront()
    {
        Vector3 position = transform.position;
        Vector3 forward = transform.forward;

        BoxCastDrawer.DrawBoxCastBox(position + forward * 2, new Vector3(2, 1, 1), transform.rotation, forward, distanceRayCast, Color.blue);

        if (Physics.BoxCast(position + forward * 2, new Vector3(2, 1, 1), forward, out RaycastHit hitInfo, transform.rotation, distanceRayCast, layerMaskRayCast))
        {
            Vector3 targetDir = hitInfo.transform.position - position;
            Vector2 force;
            if (Vector3.Cross(targetDir, forward).z < 0)
            {
                force = new Vector2(-forward.z, forward.x).normalized * 5.0f;
            }
            else
            {
                force = new Vector2(forward.z, -forward.x).normalized * 5.0f;
            }
            
            movementVector += force;
            
            Debug.DrawLine(position + Vector3.up, position + Vector3.up + new Vector3(force.x, 0, force.y));
        }
    }
}
