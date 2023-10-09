using UnityEngine;
using System.Collections.Generic;

public class Car : MonoBehaviour
{
    private Rigidbody rigidbody;
    private float driveAxis, brakeAxis, turnAxis;
    [SerializeField] List<Transform> wheels;
    [SerializeField] float wheelRadius = 0.4f;
    [SerializeField] float springStrength = 100f;
    [SerializeField] float springDamping = 3f;

    [SerializeField] bool grounded = false;
    [SerializeField] float maxSpeed;
    [SerializeField] float longitudinalFriction;
    [SerializeField] float lateralFriction;
    [SerializeField] float steeringAngle = 15f;
    [SerializeField] float turnDamping = 5f;

    public void Drive(float driveAxis)
    {
        this.driveAxis = Mathf.Clamp(driveAxis, -1f, 1f);
    }

    public void Brake(float brakeAxis)
    {
        this.brakeAxis = Mathf.Clamp(brakeAxis, 0f, 1f);
    }

    public void Turn(float turnAxis)
    {
        this.turnAxis = Mathf.Clamp(turnAxis, -1f, 1f);
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ApplySuspension();
        if (!grounded) return;
        ApplyLongitudinalForce();
        ApplyLateralForce();
        ApplyTurningForce();
    }

    private void ApplySuspension()
    {
        bool tempGrounded = false;

        foreach (Transform wheel in wheels)
        {
            Vector3 origin = wheel.position;
            Vector3 direction = -wheel.up;
            RaycastHit hit;
            float offset = 0f;

            if (Physics.Raycast(origin, direction, out hit, wheelRadius))
            {
                // At least one of the wheel raycasts hit the ground
                tempGrounded = true;

                Vector3 end = origin + (direction * wheelRadius);
                offset = (end - hit.point).magnitude;

                float pointVelocity = Vector3.Dot(wheel.up, rigidbody.GetPointVelocity(wheel.position));
                float suspensionForce = (springStrength * offset) + (-pointVelocity * springDamping);
                rigidbody.AddForceAtPosition(wheel.up * suspensionForce, wheel.position);
            }
        }

        grounded = tempGrounded;
    }

    private void ApplyLongitudinalForce()
    {
        Vector3 force = Vector3.zero;
        float forwardVelocity = Vector3.Dot(transform.forward, rigidbody.velocity);
        float maxSpeedRatio = (1 - (Mathf.Abs(forwardVelocity) / maxSpeed));

        if (Mathf.Abs(driveAxis) > 0)
        {
            force = transform.forward * driveAxis * maxSpeed * maxSpeedRatio;
        }
        else
        {
            force = transform.forward * -forwardVelocity * longitudinalFriction;
        }

        rigidbody.AddForce(force);
    }

    private void ApplyLateralForce()
    {
        float rightVelocity = Vector3.Dot(transform.right, rigidbody.velocity);
        rigidbody.AddForce(transform.right * -rightVelocity * lateralFriction);
    }

    private void ApplyTurningForce()
    {
        float forwardVelocity = Vector3.Dot(transform.forward, rigidbody.velocity);
        float rotationalVelocity = Vector3.Dot(transform.up, rigidbody.angularVelocity);

        Vector3 rotationAxis = transform.up;
        float torque = forwardVelocity * turnAxis * (Mathf.Deg2Rad * steeringAngle);
        torque += -rotationalVelocity * turnDamping;

        rigidbody.AddTorque(rotationAxis * torque);
    }
}