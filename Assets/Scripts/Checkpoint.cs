using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    public Animator animator;

    public UnityEvent<GameObject, Checkpoint> OnCheckpointPassed;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void CheckpointPassed(){
        animator.Play("PassCheckpoint");
    }

    private void OnTriggerEnter(Collider other)
    {
        OnCheckpointPassed.Invoke(other.attachedRigidbody.gameObject, this);
    }
}
