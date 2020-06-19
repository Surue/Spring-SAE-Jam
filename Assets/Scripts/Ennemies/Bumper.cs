using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CarMovement))]
public class Bumper : MonoBehaviour {

    [Header("Idle state")] 
    [SerializeField] private float idleTimer = 1.0f;

    [Header("Move to position state")] 
    [SerializeField] private float distanceToStop = 0.5f;
    [SerializeField] private float basicSpeed = 5.0f;
    private Vector3 targetPosition;
    
    [Header("Preparing dash state")] 
    [SerializeField] private float preparingDashTimer = 5.0f;
    [SerializeField] private float distanceToTarget = 5.0f;

    [Header("Dash")] 
    [SerializeField] private float dashSpeed = 5.0f;
    [SerializeField] private float dashDistance = 7.5f;

    private Transform player;
    private Material material;

    private float currentTimer = 0;

    private CarMovement carMovement;
    
    //Movement
    private float movementAngle = 0;
    private float movementSpeed = 0;
    
    enum State {
        IDLE, //Basic states to wait a few seconds before starting to roll again
        MOVE_TO_POSITION, //The bumper move to a "good" position to run on the player
        PREPARING_DASH, //The bumper stay still and load its dash. It's still able to slowly rotate
        DASH //Dash over a given distance
    }

    private State state_ = State.MOVE_TO_POSITION;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;

        material = GetComponent<MeshRenderer>().material;

        carMovement = GetComponent<CarMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer += Time.deltaTime;
        
        switch (state_)
        {
            case State.IDLE:
                
                //Transition to Move to position
                if (currentTimer > idleTimer)
                {
                    //TODO Select position
                    targetPosition = player.transform.position + player.forward * 5;

                    currentTimer = 0;
                    state_ = State.MOVE_TO_POSITION;
                    material.color = Color.blue;
                }
                break;
            case State.MOVE_TO_POSITION:
            {
                //TODO Move towards position
                Vector3 dir = targetPosition - transform.position;
                movementAngle = Vector2.Angle(new Vector2(dir.x, dir.z),
                    new Vector2(transform.forward.x, transform.forward.z));

                float distance = Vector2.Distance(
                    new Vector2(transform.position.x, transform.position.z),
                    new Vector2(targetPosition.x, targetPosition.z));

                movementSpeed =  Mathf.Clamp01(distance / 5.0f) * basicSpeed;

                //Transition to Preparing dash
                if (distance < distanceToStop)
                {
                    currentTimer = 0;
                    state_ = State.PREPARING_DASH;
                    material.color = Color.yellow;
                }
            }
                break;
            case State.PREPARING_DASH:
            {
                //TODO Slowly rotate towards player

                Vector3 dir = player.position - transform.position;
                movementAngle = Vector2.Angle(new Vector2(dir.x, dir.z),
                    new Vector2(transform.forward.x, transform.forward.z));

                movementSpeed = 0;

                //Transition to dash
                if (currentTimer > preparingDashTimer)
                {
                    currentTimer = 0;
                    material.color = Color.red;
                    state_ = State.DASH;
                }
            }
                break;
            case State.DASH:
                //TODO Dash a certain distance
                movementAngle = 0;
                movementSpeed = dashSpeed;
                //Transition to idle
                if (Vector3.Distance(transform.position, targetPosition) > dashDistance)
                {
                    currentTimer = 0;
                    state_ = State.IDLE;
                    material.color = Color.white;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FixedUpdate()
    {
        carMovement.Movement(movementAngle, movementSpeed);
    }

    /// <summary>
    /// Find the position from witch it will dash to the player
    /// </summary>
    /// <returns></returns>
    Vector3 FindDashPosition()
    {
        return new Vector3();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetPosition, 0.5f);
    }
}
