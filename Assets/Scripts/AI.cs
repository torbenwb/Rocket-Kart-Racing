using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Car car;

    public float stopDistance = 1f;
    public Transform target;
    public float turnCoefficient = 1f;

    private void Drive(Vector3 targetPosition){
        Vector3 directionToTarget = (targetPosition - car.transform.position).normalized;
        float distanceToTargt = (targetPosition - car.transform.position).magnitude;
        float turnAngle = Vector3.SignedAngle(car.transform.forward, directionToTarget, car.transform.up);
        car.Turn(turnAngle * Mathf.Deg2Rad * turnCoefficient);
        if (distanceToTargt > stopDistance) car.Drive(1 - (Mathf.Abs(turnAngle) / 180f));
        else car.Drive(0f);
    }
}
