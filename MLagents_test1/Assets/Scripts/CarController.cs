using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CarController : MonoBehaviour
{
    public void SetInputs(float speed, float steer)
    {
        if(speed > 0) { speedInput = speed * forwardAccel * 100f; }
        else { speedInput = speed * reverseAccel * 100f; }
        turnInput = steer;
    }
    public void SetPosition(Vector3 pos)
    {
        rb.transform.position = pos;
        rb.velocity = Vector3.zero;
    }

    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float forwardAccel, reverseAccel, turnStrenght, gravityForce, groundDrag, airDrag;

    private float speedInput, turnInput;
    private bool isGrounded;

    [SerializeField]
    private LayerMask WhatIsGround;
    [SerializeField]
    private float groundRayLength = .5f;
    [SerializeField]
    private Transform groundRayPoint;

    [SerializeField]
    private Transform leftFront, rightFront;
    [SerializeField]
    private float maxWheelTurn = 25f;

    private bool tyreTrailsFlag, tyresSmokeFlag;
    [SerializeField]
    private TrailRenderer[] tyreTrails;
    [SerializeField]
    private VisualEffect[] tyresSmoke;

    public float ForwardSpeed => Vector3.Dot(transform.forward, rb.velocity.normalized);
    public Vector3 LocalVelocity => transform.InverseTransformVector(rb.velocity);

    private void Start()
    {
        rb.transform.parent = null;
        tyreTrailsFlag = true;
        tyresSmokeFlag = true;
    }
    private void FixedUpdate()
    {
        transform.position = rb.transform.position;
        if (isGrounded)
        {
            rb.drag = groundDrag;
            if (Mathf.Abs(speedInput) > 0)
            {
                rb.AddForce(transform.forward * speedInput);
            }
        }
        else
        {
            rb.drag = airDrag;
            rb.AddForce(-Vector3.up * gravityForce  * 100f);
        }
    }
    private void Update()
    {
        CheckStatus();
        //GetInput();
        CheckTyreTrailsFlag();

        if (isGrounded && LocalVelocity.magnitude > 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + 
                new Vector3(0f, turnInput * turnStrenght * Mathf.Clamp((1.5f-LocalVelocity.magnitude/300f), 1, 1.2f)  * Time.deltaTime, 0f));
        }
        leftFront.localRotation = Quaternion.Euler(leftFront.localRotation.eulerAngles.x,
            (turnInput * maxWheelTurn) + 180, leftFront.localRotation.eulerAngles.z);
        rightFront.localRotation = Quaternion.Euler(rightFront.localRotation.eulerAngles.x, 
            (turnInput * maxWheelTurn) + 180, rightFront.localRotation.eulerAngles.z);

    }

    private void CheckStatus()
    {
        isGrounded = Physics.Raycast(groundRayPoint.position, -Vector3.up, out RaycastHit hit, groundRayLength, WhatIsGround);
        if (isGrounded)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
    }
    private void GetInput()
    {
        speedInput = 0f;
        if(Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel * 100f;
        }else if(Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel * 100f;
        }

        turnInput = Input.GetAxis("Horizontal");
    }
    private void CheckTyreTrailsFlag()
    {
        if (isGrounded)
        {
            if(speedInput > 0)
            {
                if(Vector3.Angle(rb.velocity, transform.forward) > 25f)
                {
                    StartEmittingTrails();
                    StartEmittingSmoke();
                }
                else
                {
                    StopEmittingTrails();
                    StopEmittingSmoke();
                }
            }
            else if(speedInput < 0)
            {
                if (Vector3.Angle(rb.velocity, -transform.forward) > 25f)
                {
                    StartEmittingTrails();
                    StartEmittingSmoke();
                }
                else
                {
                    StopEmittingTrails();
                    StopEmittingSmoke();
                }
            }
            else
            {
                StopEmittingTrails();
                StopEmittingSmoke();
            }
        }
        else
        {
            StopEmittingTrails();
            StopEmittingSmoke();
        }
    }
    private void StartEmittingTrails()
    {
        if (tyreTrailsFlag) { return; }
        foreach(TrailRenderer t in tyreTrails)
        {
            t.emitting = true;
        }
        tyreTrailsFlag = true;
    }
    private void StopEmittingTrails()
    {
        if (!tyreTrailsFlag) { return; }
        foreach (TrailRenderer t in tyreTrails)
        {
            t.emitting = false;
        }
        tyreTrailsFlag = false;
    }
    private void StartEmittingSmoke()
    {
        if (tyresSmokeFlag) { return; }
        foreach(VisualEffect v in tyresSmoke)
        {
            v.Play();
        }
        tyresSmokeFlag = true;
    }
    private void StopEmittingSmoke()
    {
        if (!tyresSmokeFlag) { return; }
        foreach(VisualEffect v in tyresSmoke)
        {
            v.Stop();
        }
        tyresSmokeFlag = false;
    }
}
