using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    [Range(1, 100)]
    public float radius;


    void Update()
    {
        transform.localScale = new Vector3(radius, radius, radius);
    }
}
