using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cow : MonoBehaviour
{

    [Header("Idle state")] 
    [SerializeField] private float idleTimer = 1.0f;

    [Header("Rotate towards player")]
    [SerializeField] private float distanceToPlayer = 30;

    [Header("Milking at player")] 
    [SerializeField] private GameObject prefabMilkDroplet;
    [SerializeField] private float minTimeBetweenShoot = 1;
    [SerializeField] private float maxTimeBetweenShoot = 1;
    private float shootingTimer = 0;
    [SerializeField] private List<Transform> shootingPosition;
    [SerializeField] private float minForce = 5;
    [SerializeField] private float maxForce = 5;
    
    [Header("Death")] 
    // [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private float dyingTime = 3.0f;
    
    private Transform player;

    private float currentTimer = 0;

    [Header("Audio")] 
    [SerializeField] private AudioSource audioSource;

    [Header("Colliders")] 
    [SerializeField] private Collider cowCollider;

    [Header("Animation")] 
    [SerializeField] private Animator animator;
    [SerializeField] private float animationTime;
    
    enum State {
        PAUSE,
        IDLE, //Basic states to wait a few seconds before starting to roll again
        ROTATE_TOWARDS_PLAYER,
        OPEN_ANIMATION,
        MILK_AT_PLAYER, 
        DYING //State to wait final destroy
    }

    private State state_ = State.IDLE;
    private State previousState_ = State.IDLE;
    
    // Start is called before the first frame update
    void Start() {
        player = FindObjectOfType<PlayerController>().transform;

        if (GameManager.Instance.CurrentState == GameManager.GameState.START)
        {
            previousState_ = state_;
            state_ = State.PAUSE;
        }
    }

    // Update is called once per frame
    void Update() {
        currentTimer += Time.deltaTime;

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
                    state_ = State.ROTATE_TOWARDS_PLAYER;
                }
                break;
            case State.ROTATE_TOWARDS_PLAYER:
                Vector3 dir = transform.position - player.position;
                dir.y = 0;
                transform.forward = Vector3.Lerp(transform.forward, dir, 0.1f);
                
                if (Vector3.Distance(transform.position, player.position) < distanceToPlayer)
                {
                    state_ = State.OPEN_ANIMATION;
                    animator.SetBool("open", true);
                }
                break;
            case State.OPEN_ANIMATION:
                if (currentTimer > animationTime)
                {
                    state_ = State.MILK_AT_PLAYER;
                    
                    shootingTimer = Random.Range(minTimeBetweenShoot, maxTimeBetweenShoot);
                }
                break;
            case State.MILK_AT_PLAYER:
                shootingTimer -= Time.deltaTime;
                if (shootingTimer <= 0)
                {
                    int index = Random.Range(0, shootingPosition.Count);
                    
                    GameObject instance = Instantiate(prefabMilkDroplet,
                        shootingPosition[index].position, Quaternion.identity);

                    instance.GetComponent<Rigidbody>().velocity = shootingPosition[index].forward * Random.Range(minForce, maxForce);
                    shootingTimer = 0;
                    
                    shootingTimer = Random.Range(minTimeBetweenShoot, maxTimeBetweenShoot);
                }
                
                if (Vector3.Distance(transform.position, player.position) > distanceToPlayer)
                {
                    animator.SetBool("open", false);
                    state_ = State.ROTATE_TOWARDS_PLAYER;
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

    private void OnCollisionEnter(Collision other)
    {
        //If hit player with the car collider => Die
        if (other.GetContact(0).thisCollider == cowCollider && other.gameObject.CompareTag("Player") && state_ != State.DYING)
        {
            other.gameObject.GetComponent<PlayerController>().ScreenShake();
            
            //Kill bumper
            // explosionParticleSystem.Play();

            state_ = State.DYING;

            currentTimer = 0.0f;
            
            Rigidbody body = GetComponent<Rigidbody>();
            body.velocity = (transform.position - other.GetContact(0).point).normalized * other.rigidbody.velocity.magnitude;
            body.constraints = RigidbodyConstraints.None;

            Destroy(cowCollider);
            
            audioSource.Play();
        }
    }

    private void OnDrawGizmos()
    {
        if (shootingPosition == null) return;
        foreach (var transform1 in shootingPosition)
        {
            Gizmos.DrawWireSphere(transform1.position, 0.25f);
            Gizmos.DrawLine(transform1.position, transform1.position + transform1.forward);
        }
        
        Gizmos.DrawWireSphere(transform.position, distanceToPlayer);
    }
}
