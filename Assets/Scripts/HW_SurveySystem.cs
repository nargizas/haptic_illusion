using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HW_SurveySystem : MonoBehaviour
{
    public string userID;
    public float totalTime;
    public float timeRemaining;
    public static bool timeIsRunning;
    private bool inputEnabled;

    public static int number = 0;
    public bool sampleHasEnded = false;
    public static bool hasEnded = false;


    // Start is called before the first frame update
    void Start()
    {
        totalTime = 0;
        timeRemaining = 15.5f;
        timeIsRunning = true;
        inputEnabled = false;
    }

    private void FixedUpdate()
    {
        //count total time
        totalTime += Time.fixedDeltaTime;
        if (!hasEnded)
        {
            if (timeIsRunning)
            {
                //if there is still time remainiing, continue subtracting
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.fixedDeltaTime;
                }
                //when time is up, play the beep sound, set remaining time to 0, enable input
                else
                {
                    timeIsRunning = false;
                    timeRemaining = 0;
                    inputEnabled = true;
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

        //if input is enabled, then press space bar to call first question's box and disable the input
        if (inputEnabled)
        {
            if (Input.GetKeyDown("space"))
            {
                Increment(Randomize.samples);
                inputEnabled = false;
            }
        }
    }

    public void Increment(float[] array)
    {
        if (number < array.Length)
        {
            number++;
            sampleHasEnded = true;
            timeRemaining = 15;
            timeIsRunning = true;
        }
    }

}
