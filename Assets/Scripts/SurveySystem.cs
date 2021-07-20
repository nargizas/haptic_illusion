using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveySystem : MonoBehaviour
{
    public float totalTime;
    public float timeRemaining = 30;
    private bool timeIsRunning;
    private bool inputEnabled;

    public GameObject firstBox;
    public GameObject secondBox;

    AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        totalTime = 0;
        timeIsRunning = true;
        inputEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;


        if (timeIsRunning)
        {
            firstBox.SetActive(false);
            secondBox.SetActive(false);
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                audioSource.Play();
                timeIsRunning = false;
                timeRemaining = 0;
                inputEnabled = true;
            }
        }

        if (inputEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                firstBox.SetActive(true);
                inputEnabled = false;
            }
        }
    }
}
