using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gunner : MonoBehaviour
{
    [Header("Spawn state")]
    [SerializeField] private float minDistanceFromEnter = 1.0f;

    [Header("Idle state")] 
    [SerializeField] private float idleTimer = 1.0f;

    [Header("Goes near player")] 
    [SerializeField] private float minDistanceFromPlayer = 30;

    [Header("Shooting")] 
    [SerializeField] private GameObject prefabProjectile;
    [SerializeField] private Transform shootingPos;
    [SerializeField] private float firingSpeed = 50;
    [SerializeField] private GameObject haloObject;

    [Header("Aim")] 
    [SerializeField] private float aimingTime = 3;
    [SerializeField] private Transform gun;
    
    [Header("Death")] 
    [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private float dyingTime = 3.0f;
    [SerializeField] private List<GameObject> smallParts;
    [SerializeField] private Collider mainCollider;
    
    private Transform player;
    private Transform enterPoint;

    private float currentTimer = 0;

    //Movement
    private Vector2 movementVector;
    [Header("Force")] 
    [SerializeField] private float horizontalForceFactor = 10;
    [SerializeField] private float verticalForceFactor = 150;

    [Header("RayCast")]
    [SerializeField] private float distanceRayCast = 10f;
    [SerializeField] private LayerMask layerMaskRayCast;

    [Header("Audio")] 
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip shotGunClip;
    [SerializeField] private AudioClip deathSound;

    enum State
    {
        SPAWN_STATE,
        PAUSE,
        IDLE,
        GOES_NEAR_PLAYER,
        AIM,
        SHOOT,
        DYING
    }

    private State state_ = State.IDLE;
    private State previousState_ = State.IDLE;

    private Rigidbody body;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;

        body = GetComponent<Rigidbody>();
        
        if (GameManager.Instance.CurrentState == GameManager.GameState.START)
        {
            previousState_ = state_;
            state_ = State.PAUSE;
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
                    state_ = State.GOES_NEAR_PLAYER;
                }
                break;
            case State.SPAWN_STATE:
            {
                Vector3 dir = (enterPoint.position - transform.position).normalized;

                float distance = Vector2.Distance(
                    new Vector2(transform.position.x, transform.position.z),
                    new Vector2(enterPoint.position.x, enterPoint.position.z)
                    );

                Vector2 force = new Vector2(dir.x, dir.z);
                movementVector += force;
                //EvaluateObstacleInFront();

                //Transition to Preparing dash
                if (distance < minDistanceFromEnter)
                {
                    currentTimer = 0.0f;
                    state_ = State.IDLE;
                }
            }
                break;
            case State.GOES_NEAR_PLAYER:
            {
                Vector3 dir = (player.position - transform.position).normalized;

                float distance = Vector2.Distance(
                    new Vector2(transform.position.x, transform.position.z),
                    new Vector2(player.position.x, player.position.z)
                );

                Vector2 force = new Vector2(dir.x, dir.z);
                movementVector += force;
                EvaluateObstacleInFront();

                //Transition to Preparing dash
                if (distance < minDistanceFromPlayer)
                {
                    currentTimer = 0.0f;
                    state_ = State.AIM;
                }
            }
                break;
            case State.AIM:
                gun.forward = (player.position - gun.position).normalized;
                transform.forward = gun.forward;

                if (currentTimer > aimingTime)
                {
                    currentTimer = 0;
                    state_ = State.SHOOT;
                }
                break;
            case State.SHOOT:
                GameObject instance = Instantiate(prefabProjectile, shootingPos.position, shootingPos.rotation);
                instance.GetComponent<Rigidbody>().velocity = instance.transform.forward * firingSpeed;

                audioSource.clip = shotGunClip;
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.Play();
                
                state_ = State.IDLE;
                currentTimer = 0;

                StartCoroutine(GunBlinking());
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
        
        // SerializedObject halo = new SerializedObject(haloObject.GetComponent("Halo"));
        // halo.FindProperty("m_Size").floatValue = 1;
        // halo.FindProperty("m_Enabled").boolValue = true;
        // halo.FindProperty("m_Color").colorValue = Color.green;
        // halo.ApplyModifiedProperties();
    }

    IEnumerator GunBlinking()
    {
        SerializedObject halo = new SerializedObject(haloObject.GetComponent("Halo"));

        halo.FindProperty("m_Size").floatValue = 2.0f;
        halo.ApplyModifiedProperties();
        yield return new WaitForSeconds(0.1f);
        
        halo.FindProperty("m_Size").floatValue = 0.0f;
        halo.ApplyModifiedProperties();
    }

    void EvaluateObstacleInFront()
    {
        Vector3 position = transform.position;
        Vector3 forward = transform.forward;

        BoxCastDrawer.DrawBoxCastBox(new Vector3(position.x, 0, position.z) + forward * 2, new Vector3(2, 1, 1), transform.rotation, forward, distanceRayCast, Color.blue);

        if (Physics.BoxCast(new Vector3(position.x, 0, position.z) + forward * 2, new Vector3(2, 1, 1), forward, out RaycastHit hitInfo, transform.rotation, distanceRayCast, layerMaskRayCast))
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

    private void FixedUpdate()
    {
        if (state_ == State.DYING) return;

        if (movementVector != Vector2.zero)
        {
            if (Physics.Raycast(transform.position + Vector3.down * 0.99f, Vector3.down, 0.1f))
            {
                body.AddForce(Vector3.up * verticalForceFactor); //Jump force
                body.AddForce(new Vector3(movementVector.x, 0, movementVector.y) * horizontalForceFactor); //Movement vector
                
                transform.forward = new Vector3(movementVector.x, 0, movementVector.y);
                gun.forward = new Vector3(movementVector.x, 0, movementVector.y);
            }

            if (body.velocity.y < 0)
            {
                body.velocity = new Vector3(body.velocity.x, body.velocity.y * 1.05f, body.velocity.z);
            }
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && state_ != State.DYING)
        {
            other.gameObject.GetComponent<PlayerController>().ScreenShake();
            
            //Kill bumper
            explosionParticleSystem.Play();

            state_ = State.DYING;

            currentTimer = 0.0f;
            
            body.velocity = (transform.position - other.GetContact(0).point).normalized * other.rigidbody.velocity.magnitude;
            body.constraints = RigidbodyConstraints.None;

            audioSource.clip = deathSound;
            audioSource.Play();

            foreach (var smallPart in smallParts)
            {
                smallPart.GetComponent<BoxCollider>().enabled = true;
                smallPart.AddComponent<Rigidbody>();
            }
            
            Destroy(mainCollider);
        }
    }

    public void SetEnterPoint(Transform enterPoint)
    {
        this.enterPoint = enterPoint;
        state_ = State.SPAWN_STATE;
    }
}
