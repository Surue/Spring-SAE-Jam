using System;
using System.Collections.Generic;
using UnityEngine;

public class Cow : MonoBehaviour
{

    [Header("Idle state")] 
    [SerializeField] private float idleTimer = 1.0f;

    [SerializeField] private float distanceToPlayer = 30;
    
    [Header("Death")] 
    // [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private float dyingTime = 3.0f;
    
    private Transform player;

    private float currentTimer = 0;

    [Header("Audio")] 
    [SerializeField] private AudioSource audioSource;

    [Header("Colliders")] 
    [SerializeField] private Collider cowCollider;
    
    enum State {
        PAUSE,
        IDLE, //Basic states to wait a few seconds before starting to roll again
        ROTATE_TOWARDS_PLAYER,
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
                    state_ = State.MILK_AT_PLAYER;
                }
                break;
            case State.MILK_AT_PLAYER:

                state_ = State.ROTATE_TOWARDS_PLAYER;
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
}
