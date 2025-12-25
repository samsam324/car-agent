using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody rb;

    public float forwardAccel = 8f, reverseAccel = 4f, maxSpeed = 50f, turnStrength = 140f, gravityForce = 10f, dragOnGround = 3f;

    private bool wPressed;
    private bool sPressed;
    private bool aPressed;
    private bool dPressed;

    private bool grounded;

    public LayerMask whatIsGround;
    public float groundRayLength = 0.5f;
    public Transform groundRayPoint;

    public Transform leftFrontWheel, rightFrontWheel;
    public float maxWheelTurn = 25f;

    void Start()
    {
        rb.transform.parent = null;
        wPressed = false;
        sPressed = false;
        aPressed = false;
        dPressed = false;
    }

    
    void Update()
    {
        //Movement forward and backwards
        if (Input.GetKey("w"))
        {
            wPressed = true;
            sPressed = false;

        }
        else if (Input.GetKey("s"))
        {
            sPressed = true;
            wPressed = false;
        }
        else
        {
            wPressed = false;
            sPressed = false;
        }
        
        if (Input.GetKey("a"))
        {
            aPressed = true;
            dPressed = false;

            leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (-1f * maxWheelTurn) - 180f, leftFrontWheel.localRotation.eulerAngles.z);
            rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, -1f * maxWheelTurn, rightFrontWheel.localRotation.eulerAngles.z);
        }
        else if (Input.GetKey("d"))
        {
            dPressed = true;
            aPressed = false;

            leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (1f * maxWheelTurn) - 180f, leftFrontWheel.localRotation.eulerAngles.z);
            rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, 1f * maxWheelTurn, rightFrontWheel.localRotation.eulerAngles.z);
        }
        else
        {
            dPressed = false;
            aPressed = false;

            leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (0f * maxWheelTurn) - 180f, leftFrontWheel.localRotation.eulerAngles.z);
            rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, 0f * maxWheelTurn, rightFrontWheel.localRotation.eulerAngles.z);
        }


        transform.position = rb.transform.position;
    }

    void FixedUpdate()
    {
        grounded = false;
        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        
        if (grounded)
        {
            rb.drag = dragOnGround;
            if (wPressed)
            {
                if (aPressed)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, -1f * turnStrength * Time.fixedDeltaTime, 0f));
                }
                else if (dPressed)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, 1f * turnStrength * Time.fixedDeltaTime, 0f));
                }

                rb.AddForce(transform.forward * forwardAccel * 1000f);
                wPressed = false;
            }
            else if (sPressed)
            {
                if (aPressed)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, 1f * turnStrength * Time.fixedDeltaTime, 0f));
                }
                else if (dPressed)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, -1f * turnStrength * Time.fixedDeltaTime, 0f));
                }

                rb.AddForce(transform.forward * reverseAccel * -1000f);
                sPressed = false;
            }
        }
        else
        {
            rb.drag = 0.1f;
            rb.AddForce(Vector3.up * -gravityForce * 100f);
        }
        
    }
}
