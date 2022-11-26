using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!target) return;
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        directionToTarget.y = 0f;
        transform.rotation = Quaternion.LookRotation(directionToTarget);
    }
}
