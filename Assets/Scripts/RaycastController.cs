using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastController : MonoBehaviour
{
    //public GameObject gameObj;

    //public GameObject[,] raycasts;
    //public Raycast[,] raycastScripts;
    public float max = 0.0f;
    public float min = 0.0f;
    public float[,] hitDistances;
    private void Awake()
    {
        hitDistances = new float[20, 20];

    }
    private void FixedUpdate()
    {
        int layerMask = 1 << 4;
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                RaycastHit hit;

                if (Physics.Raycast(new Vector3(i * 3, 100, j * 3), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
                {
                    hitDistances[i, j] = 92.5f - hit.distance;
                    if (hitDistances[i,j] < 0)
                    {
                        hitDistances[i, j] = 0;
                    }
                    //Debug.DrawRay(new Vector3(i * 3, 100, j * 3), transform.TransformDirection(Vector3.down) * 1000, Color.yellow);
                    

                }
                else
                {
                    hitDistances[i, j] = 0;
                    //Debug.DrawRay(new Vector3(i * 3, 100, j * 3), transform.TransformDirection(Vector3.down) * 1000, Color.white);
                }



                if (hitDistances[i, j] > max)
                {
                    max = hitDistances[i, j];
                }

                //Debug.Log(hitDistances[i, j]);
            }
        }
    
    }
}
