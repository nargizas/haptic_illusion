using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomize : MonoBehaviour
{
    public int numberOfSamples;
    public float maximum;
    public float minimum;
    public int numberOfRepeats;

    [HideInInspector]
    public int totalNumber;
    private float[] angleOptions;
    public static float[] samples;
    public static float[] trainingSamples;
    
    private bool isRandomized = false;
    private SurveySystem surveySystem;
    private Retarget retarget;

    private void Awake()
    {
        samples = CreateSamplesArray(numberOfSamples, maximum, minimum, numberOfRepeats);
        //randomize array with all possible samples
        RandomizeArray(samples);
        for (int i = 0; i < samples.Length; i++)
        {
            Debug.Log((i + 1) + " " + samples[i]);
        }
/*
        if (!surveySystem.isTraining)
        {
            for (int i = 0; i < samples.Length; i++)
            {
                Debug.Log((i + 1) + " " + samples[i]);
            }
        }
        else
        {
            for (int i = 0; i < trainingSamples.Length; i++)
            {
                Debug.Log((i + 1) + " " + trainingSamples[i]);
            }
        }*/
    }
    private void Start()
    {
        trainingSamples = CreateSamplesArray(numberOfSamples, maximum, minimum, numberOfRepeats);
        surveySystem = GetComponent<SurveySystem>();

    }
    void Update()
    {
/*
        if (!isRandomized)
        {
            samples = CreateSamplesArray(numberOfSamples, maximum, minimum, numberOfRepeats);
            //randomize array with all possible samples
            RandomizeArray(samples);


            if (!surveySystem.isTraining)
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    Debug.Log((i + 1) + " " + samples[i]);
                }
            }
            else
            {
                for (int i = 0; i < trainingSamples.Length; i++)
                {
                    Debug.Log((i + 1) + " " + trainingSamples[i]);
                }
            }

            isRandomized = true;
        }*/
    }

    private void OnValidate()
    {
        //set min angle to be less than max angle and vice versa
        minimum = Mathf.Min(minimum, maximum);
        maximum = Mathf.Max(minimum, maximum);
        
        //at least 1 sample
        if(numberOfSamples < 2)
        {
            numberOfSamples = 2;
            numberOfRepeats = 1;
        } 
    }

    private void RandomizeArray(float[] array)
    {
        //shuffling array elements
        for (int t = 0; t < array.Length; t++)
        {
            float tmp = array[t];
            int r = Random.Range(t, array.Length);
            array[t] = array[r];
            array[r] = tmp;
        }
    }
    

    public float[] CreateSamplesArray(int numberOfSamples, float maximum, float minimum, int numberOfRepeats)
    {
        //array of unique angle samples (i.e. 5, 10, 15, 20, 25 etc.)
        angleOptions = new float[numberOfSamples];

        //each angle sample is repeated certain number of times
        totalNumber = numberOfSamples * numberOfRepeats;

        //array of all angles
        float[] samples = new float[totalNumber];

        if (numberOfSamples == 1)
        {
            if(minimum != 0)
            {
                samples[0] = minimum;
                samples[1] = maximum;
            } else {
                samples[0] = maximum;
                samples[1] = minimum;
            }
            return samples;
        }

        //difference between two consecutive angle options
        float interval = (maximum - minimum) / (numberOfSamples - 1);


        

        for (int i = 0; i < numberOfSamples; i++)
        {
            angleOptions[i] = minimum + interval * i;

            for (int j = 0; j < numberOfRepeats; j++)
            {
                samples[numberOfRepeats * i + j] = angleOptions[i];
            }
        }

        return samples;
    }
}
