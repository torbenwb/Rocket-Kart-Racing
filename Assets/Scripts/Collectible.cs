using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public bool active = true;
    public float respawnTime = 5f;
    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        GameObject otherGameObject = other.attachedRigidbody.gameObject;
        Car car = otherGameObject.GetComponent<Car>();

        if (car){
            car.boost = car.maxBoost;
            StartCoroutine(Inactive());
            
        }
    }

    private void Activate(){
        active = true;
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer m in meshRenderers){
            m.enabled = true;
        }
    }

    private void Deactivate(){
        active = false;
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer m in meshRenderers){
            m.enabled = false;
        }
    }

    IEnumerator Inactive(){
        Deactivate();
        yield return new WaitForSeconds(respawnTime);
        Activate();
    }
}
