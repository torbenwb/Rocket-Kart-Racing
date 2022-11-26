using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrackBoundsVolume : MonoBehaviour
{
    public UnityEvent<GameObject> OnGameObjectOutOfBounds;

    private void OnTriggerEnter(Collider other)
    {
        OnGameObjectOutOfBounds.Invoke(other.attachedRigidbody.gameObject);
    }
}
