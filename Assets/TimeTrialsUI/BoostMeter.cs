using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BoostMeter : MonoBehaviour
{
    Image image;
    public float fillTarget;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = Mathf.MoveTowards(image.fillAmount, fillTarget, Time.deltaTime);
    }
}
