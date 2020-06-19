using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CarMovement carMovement;
    // Start is called before the first frame update
    void Start()
    {
        carMovement = GetComponent<CarMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        float rotationInput = Input.GetAxis("Horizontal");
        float speedInput = Input.GetAxis("Vertical");
        carMovement.Movement(rotationInput, speedInput);
    }
}
