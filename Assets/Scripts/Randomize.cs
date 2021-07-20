using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomize : MonoBehaviour
{
    public int numberOfSamples;
    public float maxAngle;
    public float minAngle;

    private float interval;
    private int totalNumber;
    private float[] angleOptions;
    private float[] samples;
    // Start is called before the first frame update
    void Start()
    {
        interval = (maxAngle - minAngle) / (numberOfSamples - 1) ;
        angleOptions = new float[numberOfSamples];
        totalNumber = numberOfSamples * 4; 
        samples = new float[totalNumber];

        for(int i = 0; i < numberOfSamples; i++)
        {
            angleOptions[i] = minAngle + interval * i;
            
            for (int j = 0; j < 4; j++)
            {
                samples[4 * i + j] = angleOptions[i];
                
            }
        }

        RandomizeArray(samples);

        /*
        for(int i = 0; i<samples.Length; i++)
        {
            Debug.Log(i + " " + samples[i]);
        }
        */
    }

    private void OnValidate()
    {
        minAngle = Mathf.Min(minAngle, maxAngle);
        maxAngle = Mathf.Max(minAngle, maxAngle);
        
        if(numberOfSamples < 1)
        {
            numberOfSamples = 1;
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RandomizeArray(float[] array)
    {
        for (int t = 0; t < array.Length; t++)
        {
            float tmp = array[t];
            int r = Random.Range(t, array.Length);
            array[t] = array[r];
            array[r] = tmp;
        }
    }

}
