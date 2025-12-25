using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System.Collections.Generic;

public class CarAgent : Agent
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

    private int checkPointNum = 0;

    void Start()
    {
        checkPointNum = 32;
        rb.transform.parent = null;
        wPressed = false;
        sPressed = false;
        aPressed = false;
        dPressed = false;
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        transform.position = rb.transform.position;

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        if (Input.GetKey("w"))
        {
            discreteActions[0] = 0;
        }
        else if (Input.GetKey("s"))
        {
            discreteActions[0] = 1;
        }
        else
        {
            discreteActions[0] = 2;
        }

        if (Input.GetKey("a"))
        {
            discreteActions[1] = 0;
        }
        else if (Input.GetKey("d"))
        {
            discreteActions[1] = 1;
        }
        else
        {
            discreteActions[1] = 2;
        }
    }

    public override void OnEpisodeBegin()
    {
        checkPointNum = 32;
        rb.transform.parent = null;
        wPressed = false;
        sPressed = false;
        aPressed = false;
        dPressed = false;

        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;

        rb.transform.position = Vector3.right * (2.7f + (Random.value * (8.0f - 2.7f))) + Vector3.up * 0.55f + Vector3.forward * (20.0f + (Random.value * (37.0f - 20.0f)));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rb.transform.position.x);
        sensor.AddObservation(rb.transform.position.y);
        sensor.AddObservation(rb.transform.position.z);

        sensor.AddObservation(transform.rotation.x);
        sensor.AddObservation(transform.rotation.y);
        sensor.AddObservation(transform.rotation.z);

        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);
        sensor.AddObservation(rb.velocity.z);

        sensor.AddObservation(rb.angularVelocity.x);
        sensor.AddObservation(rb.angularVelocity.y);
        sensor.AddObservation(rb.angularVelocity.z);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var discreteActions = actionBuffers.DiscreteActions;

        if (discreteActions[0] == 0)
        {
            wPressed = true;
            sPressed = false;
        }
        else if (discreteActions[0] == 1)
        {
            sPressed = true;
            wPressed = false;
        }
        else
        {
            wPressed = false;
            sPressed = false;
        }

        if (discreteActions[1] == 0)
        {
            aPressed = true;
            dPressed = false;

            leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (-1f * maxWheelTurn) - 180f, leftFrontWheel.localRotation.eulerAngles.z);
            rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, -1f * maxWheelTurn, rightFrontWheel.localRotation.eulerAngles.z);
        }
        else if (discreteActions[1] == 1)
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
    }

    void Update()
    {
        transform.position = rb.transform.position;
    }

    void FixedUpdate()
    {
        if (rb.transform.position.z > 150f)
        {
            EndEpisode();
        }
        else if (rb.transform.position.z < -66f)
        {
            EndEpisode();
        }
        else if (rb.transform.position.x > 13f)
        {
            EndEpisode();
        }
        else if (rb.transform.position.x < -82f)
        {
            EndEpisode();
        }

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

        transform.position = rb.transform.position;
    }

    public void CheckPointTriggered(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            if (checkPointNum == 32 && other.gameObject.name == "Checkpoint (1)")
            {
                AddReward(1.0f);
                checkPointNum = 1;
            }
            else if (other.gameObject.name == ("Checkpoint (" + (checkPointNum + 1) + ")"))
            {
                AddReward(1.0f);
                checkPointNum++;
            }
            else
            {
                EndEpisode();
            }
        }
    }
}
