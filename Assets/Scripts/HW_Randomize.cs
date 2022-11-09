using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HW_Randomize : MonoBehaviour
{
    [HideInInspector]
    private float[] angleSamples = {10, 10, 20, 20, 30, 30, 40, 40};
    private float[] scaleSamples = {1.15f, 1.15f, 1.3f, 1.3f, 1.45f, 1.45f, 1.6f, 1.6f};
    public static float[] samples = new float[16];
    public static bool[] illusions = new bool[16];

    public enum RetargetingType
    {
        ScalingUp,
        Rotation
    };

    public RetargetingType type;

    private bool isRandomized = false;

    void Update()
    {
        if (!isRandomized)
        {
            if (type == RetargetingType.Rotation)
            {
                Randomize(angleSamples);
                for (int i = 0; i < 8; i++)
                {
                    samples[2 * i] = angleSamples[i];
                    samples[2 * i + 1] = angleSamples[i];
                    int r = Random.Range(1, 20);
                    if(r < 11)
                    {
                        illusions[2 * i] = true;
                        illusions[2 * i + 1] = false;
                    } else
                    {
                        illusions[2 * i] = false;
                        illusions[2 * i + 1] = true;
                    }
                }
            }
            else if (type == RetargetingType.ScalingUp)
            {
                RandomizeTrial(scaleSamples);
            }

            for (int i = 0; i < samples.Length; i++)
            {
                Debug.Log((i + 1) + " " + samples[i] + " " + illusions[i]);
            }
            isRandomized = true;
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

    private void RandomizeTrial(float[] array)
    {
        Randomize(array);
        for (int i = 0; i < 8; i++)
        {
            samples[2 * i] = array[i];
            samples[2 * i + 1] = array[i];
            int r = Random.Range(1, 20);
            if (r < 11)
            {
                illusions[2 * i] = true;
                illusions[2 * i + 1] = false;
            }
            else
            {
                illusions[2 * i] = false;
                illusions[2 * i + 1] = true;
            }
        }
    }
}
