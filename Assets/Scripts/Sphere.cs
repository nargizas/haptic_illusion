using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    [Range(0, 0.5f)]
    public float radius;

    private HW_PinController pinController;

    private void Update()
    {
        transform.localScale = new Vector3(radius, radius, radius) * pinController.scale;
    }
}
