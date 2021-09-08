using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HW_Randomize : MonoBehaviour
{
    public int numberOfSamples;
    public float maximum;
    public float minimum;
    public int numberOfRepeats;

    [HideInInspector]
    public int totalNumber;
    private float[] angleOptions;
    private float[] samplePairs;
    public static float[] samples;
    public static bool[] illusions;
    private bool[] illusionPair = new bool[2];

    private bool isRandomized = false;

    private void Start()
    {
        illusionPair[0] = true;
        illusionPair[1] = false;
    }
    void Update()
    {
        if (!isRandomized)
        {
            samplePairs = CreateSamplesArray(numberOfSamples, maximum, minimum, numberOfRepeats);
            Randomize(illusionPair);
            //randomize array with all possible samples
            Randomize(samplePairs);

            
            samples = new float[samplePairs.Length * 2];
            illusions = new bool[samples.Length];
            Debug.Log(samplePairs.Length);
            Debug.Log(samples.Length);
            
            //double each trial
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = samplePairs[(int)(i / 2)];
                if(i % 2 == 0)
                {
                    Randomize(illusionPair);
                    illusions[i] = illusionPair[0];
                    illusions[i + 1] = illusionPair[1];
                }
            }


            for (int i = 0; i < samples.Length; i++)
            {
                Debug.Log((i + 1) + " " + samples[i] + " " + illusions[i]);
            }

            isRandomized = true;
        }

    }

    private void OnValidate()
    {
        //set min angle to be less than max angle and vice versa
        minimum = Mathf.Min(minimum, maximum);
        maximum = Mathf.Max(minimum, maximum);

        //at least 1 sample
        if (numberOfSamples < 1)
        {
            numberOfSamples = 1;
        }
    }

    private void Randomize(float[] array)
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

    private void Randomize(bool[] array)
    {
        //shuffling array elements
        for (int t = 0; t < array.Length; t++)
        {
            bool tmp = array[t];
            int r = Random.Range(t, array.Length);
            array[t] = array[r];
            array[r] = tmp;
        }
    }

    /*
    private void RandomizeTwoArrays(float[] array, float[] illusions)
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
    */

    public float[] CreateSamplesArray(int numberOfSamples, float maximum, float minimum, int numberOfRepeats)
    {

        //difference between two consecutive angle options
        float interval = (maximum - minimum) / (numberOfSamples - 1);

        //array of unique angle samples (i.e. 5, 10, 15, 20, 25 etc.)
        angleOptions = new float[numberOfSamples];

        //each angle sample is repeated certain number of times
        totalNumber = numberOfSamples * numberOfRepeats;

        //array of all angles
        float[] allSamples = new float[totalNumber];
        
        for (int i = 0; i < numberOfSamples; i++)
        {
            angleOptions[i] = minimum + interval * i;

            for (int j = 0; j < numberOfRepeats; j++)
            {
                allSamples[numberOfRepeats * i + j] = angleOptions[i];
            }
        }

        return allSamples;
    }

}
