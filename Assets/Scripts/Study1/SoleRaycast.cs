using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SoleRaycast : MonoBehaviour
{
    [SerializeField]
    private int[] soleHit;
    //layer mask for Sole Parts
    private int[] layerMasks = new int[6];
    private string[] layers = { "Sole1", "Sole2", "Sole3", "Sole4", "Sole5", "Sole6" };
    int layerMask;
    float interval;

    //input these to differentiate between datasets
    public string studyPart;
    public string userID;

    //Distinguish when user is experiencing the virtual object
    [SerializeField]
    private bool isTesting;
    //What visuo-haptic illusion is beaing tested
    [SerializeField]
    private float sample;
    private float trialNumber;
    //count time
    [SerializeField]
    private float time = 0;

    private int count = 0;

    //for saving data
    private SoleDatabase soleDatabase;
    private SoleXMLManager xmlManager;

    [SerializeField]
    private bool save = false;

    private string filename = "";

    private TextWriter tw;

    private void Awake()
    {
        //set fps
        Application.targetFrameRate = 90;
        
        //set layermask
        layerMask = 0;
        for (int i = 0; i < 6; i++)
        {
            layerMasks[i] = 6 + i;
        }
        layerMask = LayerMask.GetMask(layers);
        

        xmlManager = GetComponent<SoleXMLManager>();
        soleDatabase = new SoleDatabase();
    }

    private void Start()
    {
        filename = "C:/Users/MAKinteract_Leopard/Documents/sole_data_" + userID + ".csv";
        tw = new StreamWriter(filename, false);
        tw.WriteLine("Time, Study Part, Sample, Trial Number, In progress, Sole A, Sole B, Sole C, Sole D, Sole E, Sole F, No Touch");
        tw.Close();

        tw = new StreamWriter(filename, true);
    }
    private void FixedUpdate()
    {
        //count time
        time += Time.fixedDeltaTime;

        //check if it is waiting time or testing time
        isTesting = !SurveySystem.isWaiting;

        //check if samples have been randomized
        if (Randomize.samples == null)
        {
            Debug.Log("Null");
            sample = 0.0f;
        }
        else
        {
            if (!SurveySystem.hasEnded)
            {
                sample = (Randomize.samples[SurveySystem.number] >= 1 && Randomize.samples[SurveySystem.number] <= 2) ? (2 / Randomize.samples[SurveySystem.number]) : Randomize.samples[SurveySystem.number];
                trialNumber = SurveySystem.number + 1;
            }
            else
            {
                sample = 0.0f;
                trialNumber = 0;
            }
            
        }

        //set all values to zero
        soleHit = new int[7];

        interval = Retarget.coef * 2.0f;
        for (int i = 0; i < 30; i++)
        {
            RaycastHit hit;
            //raycast
            if (Physics.Raycast(Retarget.edgePoint + new Vector3(i*interval, -0.04f, 0), Vector3.up, out hit, Retarget.coef * 6.0f, layerMask))
            {
               
                Debug.DrawRay(Retarget.edgePoint + new Vector3(i*interval, -0.04f, 0), Vector3.up, Color.magenta, hit.distance);
                //check which layer was hit
                for (int j = 0; j < 6; j++)
                {
                    if (hit.collider.gameObject.layer == layerMasks[j])
                    {
                        soleHit[j] = 1;
                    }
                }
                soleHit[6] = 0;
            }
            else
            {
                //if not hit then NoTouch =
                Debug.DrawRay(Retarget.edgePoint + new Vector3(i*interval, -0.035f, 0), Vector3.up, Color.white);
            }
        }

        int sum = 0;
        for(int i =0; i<6; i++)
        {
            sum += soleHit[i];
        }
        if(sum == 0)
        {
            soleHit[6] = 1;
        } else
        {
            soleHit[6] = 0;
        }
        //save data
        SoleDataEntry dataEntry = new SoleDataEntry();
        dataEntry.studyPart = studyPart;
        dataEntry.sample = sample;
        dataEntry.isTesting = isTesting;
        dataEntry.time = time;
        dataEntry.soleA = soleHit[0];
        dataEntry.soleB = soleHit[1];
        dataEntry.soleC = soleHit[2];
        dataEntry.soleD = soleHit[3];
        dataEntry.soleE = soleHit[4];
        dataEntry.soleF = soleHit[5];
        dataEntry.noTouch = soleHit[6];

        if (!SurveySystem.hasEnded)
        {
            tw.WriteLine(time + "," + studyPart + "," + sample + "," + trialNumber + ","+ isTesting + "," + soleHit[0] + "," + soleHit[1] + "," + soleHit[2] + "," + soleHit[3] + "," + soleHit[4] + "," + soleHit[5] + "," + soleHit[6]);
        }
        

        soleDatabase.dataList.Add(dataEntry);
        if (Input.GetKeyDown(KeyCode.S))
        {
            tw.Close();
        }

        if (Randomize.samples == null)
        {
            Debug.Log("Null");
        }
        else
        {
            if (SurveySystem.number == Randomize.samples.Length)
            {
                tw.Close();
            }
        }

            /*count += 1;

            if (count == 10000)
            {
                SaveData();
                count = 0;
            }

            if(save == true)
            {
                SaveData();
                save = false;
            }
    */

        }

    public void SaveData()
    {
        xmlManager.SaveItems(soleDatabase, userID);
    }
}
