using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using TMPro;

public class HW_SurveySystem : MonoBehaviour
{
    public string userID;

    //time count
    public float totalTime;


    //minimum time for exploring
    public float timeRemaining;
    public static float oneTrialTime;

    //check if time is still running
    public static bool timeIsRunning;
    //check if time is up an input is enabled
    private bool inputEnabled;

    //check if it is waiting time
    public static bool isWaiting = true;

    //test time for illusion
    private float timeWithIllusion;
    //test time for real
    private float timeWithoutIllusion;
    private float answerTime;

    public static int number = 0;
    [SerializeField]
    private int trialNumber;
    public bool sampleHasEnded = false;
    private bool sampleStarted = false;
    public static bool hasEnded = false;


    public GameObject trialBox;
    public TextMeshProUGUI trialNumberText;

    public GameObject waitingBox;
    public TextMeshProUGUI waitingBoxText;

    public GameObject instructionBox;
    public TextMeshProUGUI instructionBoxText;

    //questionnaire and answers
    public GameObject firstBox;
    public GameObject secondBox;
    public GameObject thirdBox;

    [HideInInspector]
    public int firstAnswer;
    [HideInInspector]
    public int secondAnswer;
    [HideInInspector]
    public int thirdAnswer;

    //public SteamVR_Input_Sources inputSource;
    //public SteamVR_Action_Boolean clickAction;

    //database
    private HW_XML hw_xmlManager;
    private HW_UserDatabase hw_userDatabase;

    //AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        hw_xmlManager = GetComponent<HW_XML>();
        hw_userDatabase = new HW_UserDatabase();
        //audioSource = GetComponent<AudioSource>();

        totalTime = 0;
        oneTrialTime = timeRemaining;
        timeIsRunning = true;
        inputEnabled = false;


    }

    private void FixedUpdate()
    {
        /*
        //count total time
        totalTime += Time.fixedDeltaTime;
        if (!hasEnded)
        {
            if (timeIsRunning)
            {
                trialBox.SetActive(true);
                instructionBox.SetActive(true);
                //when timer is active, the dialogue boxes are inactive
                firstBox.SetActive(false);
                secondBox.SetActive(false);
                thirdBox.SetActive(false);

                if (timeRemaining > oneTrialTime - 5.0f)
                {
                    waitingBoxText.text = "WAIT";
                    waitingBoxText.color = Color.red;

                }
                else
                {
                    waitingBoxText.text = "START";
                    waitingBoxText.color = Color.black;
                }


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
        */
        if (isWaiting == false)
        {
            totalTime += Time.fixedDeltaTime;
            Debug.Log("1");
        }

        if (!hasEnded)
        {
            if (timeIsRunning)
            {
                Debug.Log("2");
                trialBox.SetActive(true);
                instructionBox.SetActive(true);

                //when timer is active, the dialogue boxes are inactive
                firstBox.SetActive(false);
                secondBox.SetActive(false);
                thirdBox.SetActive(false);


                if (timeRemaining > 10.0f)
                {
                    Debug.Log("3");
                    waitingBoxText.text = "WAIT";
                    waitingBoxText.color = Color.red;
                    instructionBoxText.text = "Please, get into the starting position";
                    isWaiting = true;
                }
                else
                {
                    Debug.Log("4");
                    instructionBoxText.text = "Please, explore the virtual object from right to left";
                    waitingBoxText.text = "GO";
                    waitingBoxText.color = Color.black;
                    isWaiting = false;
                }


                //if there is still time remainiing, continue subtracting
                if (timeRemaining > 0)
                {
                    Debug.Log("5");
                    timeRemaining -= Time.fixedDeltaTime;
                }

                //when time is up, play the beep sound, set remaining time to 0, enable input
                else
                {
                    Debug.Log("6");
                    // audioSource.Play();
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
        trialNumber = number + 1;

        //samples are doubled during one trial
        if (trialNumber <= HW_Randomize.samples.Length)
        {
            int sampleNumber = trialNumber % 2 == 0 ? (trialNumber / 2) : (trialNumber / 2 + 1);

            int total = HW_Randomize.samples.Length / 2;
            trialNumberText.text = (sampleNumber + "/" + total);
        }
        else
        {
            trialNumberText.text = "-";
        }


        //if input is enabled, then press space bar to call first question's box and disable the input
        if (inputEnabled)
        {   
            //if (clickAction.GetStateDown(inputSource) || Input.GetKeyDown("space"))
            if (Input.GetKeyDown("space"))
            {
                Debug.Log(totalTime);
                if (HW_Randomize.illusions[number] == true)
                {
                    timeWithIllusion = totalTime;
                    totalTime = 0;
                }
                else
                {
                    timeWithoutIllusion = totalTime;
                    totalTime = 0;
                }
                if (number % 2 == 1)
                {
                    instructionBox.SetActive(false);
                    trialBox.SetActive(false);
                    waitingBox.SetActive(false);
                    firstBox.SetActive(true);
                }
                else
                {
                    Increment();
                }

                inputEnabled = false;
            }

        }
    }
    //increment to move on to next sample value
    public void Increment()
    {
        if (number < HW_Randomize.samples.Length)
        {
            
            if(number % 2 == 1)
            {
                HW_UserDataEntry dataEntry = GetAnswers();
                Debug.Log(trialNumber + " " + dataEntry.firstAnswer + " " + dataEntry.secondAnswer + " " + dataEntry.thirdAnswer);
                hw_userDatabase.hw_dataList.Add(dataEntry);
                hw_xmlManager.SaveItems(hw_userDatabase, userID);
                Debug.Log("saved");
            }
            number++;
            sampleHasEnded = true;
            timeRemaining = oneTrialTime;
            timeIsRunning = true;
            totalTime = 0;
            
            
        }

        if (number == HW_Randomize.samples.Length)
        {
            instructionBox.SetActive(false);
            trialBox.SetActive(false);
        }

    }
    /*
    public void Increment(float[] array)
    {
        //increment number to move on to next trial
        if (number < array.Length)
        {
            SecondUserDataEntry dataEntry = GetAnswers();
            Debug.Log(dataEntry.number + " " + dataEntry.isIllusion + " " + dataEntry.firstAnswer + " " + dataEntry.secondAnswer + " " + dataEntry.thirdAnswer);
            userDatabase.dataList.Add(dataEntry);
            number++;
            sampleHasEnded = true;
            timeRemaining = oneTrialTime;
            timeIsRunning = true;
            totalTime = 0;
            xmlManager.SaveItems(userDatabase, userID);
        }

        if (number == array.Length)
        {
            hasEnded = true;
            Debug.Log("Thank you. This is the end of the experiment.");
        }
    }
    */

    public void answerFirstQuestion(bool answer)
    {
        if (answer)
        {
            firstAnswer = 1;
        }
        else
        {
            firstAnswer = 2;
        }
    }
    public void answerSecondQuestion(bool answer)
    {
        if (answer)
        {
            secondAnswer = 1;
        }
        else
        {
            secondAnswer = 2;
        }
    }
    public void answerThirdQuestion(bool answer)
    {
        if (answer)
        {
            thirdAnswer = 1;
        }
        else
        {
            thirdAnswer = 2;
        }
    }

    public HW_UserDataEntry GetAnswers()
    {
        HW_UserDataEntry dataEntry = new HW_UserDataEntry();
        dataEntry.number = number + 1;
        dataEntry.sample = (HW_Randomize.samples[number] >= 1 && HW_Randomize.samples[number] <= 2) ? (2 / HW_Randomize.samples[number]) : HW_Randomize.samples[number];
        dataEntry.timeWithIllusion = timeWithIllusion;
        dataEntry.timeWithoutIllusion = timeWithoutIllusion;
        dataEntry.answerTime = totalTime;
        dataEntry.firstAnswer = firstAnswer;
        dataEntry.secondAnswer = secondAnswer;
        dataEntry.thirdAnswer = thirdAnswer;

        return dataEntry;
    }


}
