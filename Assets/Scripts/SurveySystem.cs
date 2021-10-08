using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using TMPro;

public class SurveySystem : MonoBehaviour
{
    public string userID;
    [SerializeField]
    private bool withIllusion;
    public float totalTime;
    private float testTime;
    private float answerTime;
    public float timeRemaining;
    public static float oneTrialTime;
    public static bool timeIsRunning;
    private bool inputEnabled;

    public GameObject firstBox;
    public GameObject secondBox;
    public GameObject trialBox;
    public TextMeshProUGUI trialNumberText;

    public GameObject waitingBox;
    public TextMeshProUGUI waitingBoxText;

    public GameObject instructionBox;
    public TextMeshProUGUI instructionBoxText;
    AudioSource audioSource;

    public static int number = 0;
    private bool sampleStarted = false;
    public static bool hasEnded = false;

    private XMLManager xmlManager;
    private UserDatabase userDatabase;

    [HideInInspector]
    public bool isTraining;
    private string isIllusion = "Y";
    private string hadIllusion;


    public Slider slider;
    public SteamVR_Input_Sources inputSource;
    public SteamVR_Action_Boolean clickAction;

    public GameObject illusionBox;
    public TextMeshProUGUI illusionText;



    // Start is called before the first frame update
    void Start()
    {
        xmlManager = GetComponent<XMLManager>();
        audioSource = GetComponent<AudioSource>();
        userDatabase = new UserDatabase();
        totalTime = 0;
        timeIsRunning = true;
        inputEnabled = false;
        oneTrialTime = timeRemaining;
        
    }

    private void FixedUpdate()
    {
        //count total time
        if (sampleStarted)
        {
            totalTime += Time.fixedDeltaTime;
        }
        

        if (!hasEnded)
        {
            if (timeIsRunning)
            {

                trialBox.SetActive(true);
                instructionBox.SetActive(true);
                //when timer is active, the dialogue boxes are inactive
                firstBox.SetActive(false);
                secondBox.SetActive(false);
                //if there is still time remainiing, continue subtracting

                if (timeRemaining > oneTrialTime - 5.0f)
                {
                    waitingBoxText.text = "WAIT";
                    waitingBoxText.color = Color.red;
                    instructionBoxText.text = "Please, get into the starting position";

                } else
                {
                    instructionBoxText.text = "Please, explore the virtual object from right to left";
                    waitingBoxText.text = "START";
                    waitingBoxText.color = Color.black;
                    sampleStarted = true;
                }

                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.fixedDeltaTime;
                }
                //when time is up, play the beep sound, set remaining time to 0, enable input
                else
                {
                    audioSource.Play();
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
        //Show the trial number
        int trialNumber = number + 1;
        if (isTraining == true)
        {
            if (trialNumber <= Randomize.samples.Length)
            {
                int sampleNumber = trialNumber % 2 == 0 ? (trialNumber / 2) : (trialNumber / 2 + 1);

                int total = Randomize.samples.Length / 2;
                trialNumberText.text = (sampleNumber + "/" + total);
            }
            else
            {
                trialNumberText.text = "-";
            }
        } else
        {
            if (trialNumber <= Randomize.samples.Length)
            {
                trialNumberText.text = (trialNumber + "/" + Randomize.samples.Length);
            }
            else
            {
                trialNumberText.text = "-";
            }
        }

        //first sample of a trial with illusion, second sample - without
        if(number % 2 == 0)
        {
            withIllusion = true;
            isIllusion = "Y";
            illusionText.text = "With Illusion";
            illusionText.color = Color.blue;
        } else
        {
            withIllusion = false;
            isIllusion = "N";
            illusionText.text = "No Illusion";
            illusionText.color = Color.red;
        }

        //if input is enabled, then press space bar or Trigger to call first question's box and disable the input
        if (inputEnabled)
        {
            if (!isTraining)
            {
                if (clickAction.GetStateDown(inputSource) || Input.GetKeyDown("space"))
                {
                    testTime = totalTime;
                    Debug.Log(testTime);
                    instructionBox.SetActive(false);
                    trialBox.SetActive(false);
                    waitingBox.SetActive(false);
                    firstBox.SetActive(true);
                    totalTime = 0;
                    inputEnabled = false;
                }
            } else
            {
                if (clickAction.GetStateDown(inputSource) || Input.GetKeyDown("space"))
                {
                    Increment(Randomize.trainingSamples);
                    inputEnabled = false;
                }
            }
            
        }
    }

    public void Increment()
    {
        if (number < Randomize.samples.Length)
        {

            UserDataEntry dataEntry = GetAnswers();
            Debug.Log(dataEntry.number + " " + dataEntry.hasPerceivedIllusion + " " + dataEntry.rating);
            userDatabase.dataList.Add(dataEntry);
            number++;
            sampleStarted = false;
            timeRemaining = oneTrialTime;
            totalTime = 0;
            timeIsRunning = true;
            slider.value = 3.0f;
            xmlManager.SaveItems(userDatabase, userID);
        }

        if(number == Randomize.samples.Length)
        {
            instructionBox.SetActive(false);
            
            trialBox.SetActive(false);
        }

    }

    public void Increment(float[] array)
    {
        if (number < array.Length)
        {
            sampleStarted = true;
            number++;
            timeRemaining = oneTrialTime;
            timeIsRunning = true;
        }

        if (number == array.Length)
        {
            instructionBox.SetActive(false);
            instructionBoxText.text = "Thank you. This is the end of the experiment.";
            trialBox.SetActive(false);
            illusionText.text = "The end";
            illusionText.color = Color.black;
        }
    }

    public UserDataEntry GetAnswers()
    {
        UserDataEntry dataEntry = new UserDataEntry();

        dataEntry.number = number + 1;
        dataEntry.testTime = testTime;
        dataEntry.answerTime = totalTime;
        dataEntry.sample = (Randomize.samples[number] >= 1 && Randomize.samples[number] <= 2) ? (2 / Randomize.samples[number]) : Randomize.samples[number];
        dataEntry.isIllusion = "Y";
        dataEntry.hasPerceivedIllusion = hadIllusion;
        dataEntry.rating = (int)slider.value;

        return dataEntry;
    }
    
    public void hasPerceivedIllusion(bool answer)
    {
        if (answer)
        {
            hadIllusion = "Y";
        } else
        {
            hadIllusion = "N";
        }
    }

}
