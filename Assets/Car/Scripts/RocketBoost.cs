using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBoost : MonoBehaviour
{
    private bool active = false;
    [SerializeField] private float boost;
    private Rigidbody rigidbody;
    [SerializeField] private float maxBoost;
    [SerializeField] private float forceStrength;

    [SerializeField] private ParticleSystem particleSystem;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        boost = maxBoost;
    }

    private void FixedUpdate()
    {
        if (!active || boost <= 0f)
        {
            particleSystem.Stop();
            return;
        }

        particleSystem.Play();

        float forwardVelocity = Vector3.Dot(transform.forward, rigidbody.velocity);
        float speedRatio = (1 - (forwardVelocity / forceStrength));
        rigidbody.AddForce(transform.forward * forceStrength * speedRatio);
        boost -= Time.fixedDeltaTime;
    }

    #region Interface
    public void ToggleBoost(bool newValue) => active = newValue;
    public void MaxBoost() => boost = maxBoost;
    #endregion
}
