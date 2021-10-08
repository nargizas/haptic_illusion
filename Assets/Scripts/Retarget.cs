using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Retarget : MonoBehaviour
{
    //choose the type of retargeting
    public enum RetargetingType
    {
        ScalingUp,
        Rotation,
        TrainingScalingUp,
        TrainingRotation
    };
    public RetargetingType type;

    //scaling factor for Scaling-up
    [Range(0.5f, 2.5f)] public float scale;
    //angle for retargeting
    [Range(0, 90)] public float angle;
    //last angle used for retargeting
    private float lastAngle;


    //virtual image of physical rod used in experiment
    public GameObject stick;
    public GameObject floor;

    public GameObject leftEdge;
    public GameObject rightEdge;

    private Vector3 calibratedLocalScale;
    private Vector3 stickStartPosition;


    //virtual image (red ball) of actual position(white ball)
    public GameObject retargetedPosition;

    // the edge of the stick
    private Vector3 edgePoint;

    private bool isInitialized = false;
    private bool afterScaling = false;


    public enum InputMode
    {
        Manual,
        Automatic,
        Training
    }
    public InputMode inputMode;

    private Randomize randomize;
    private SurveySystem surveySystem;

    private void Start()
    {
        randomize = GetComponent<Randomize>();
        surveySystem = GetComponent<SurveySystem>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        Initialize();
        retargetedPosition.transform.rotation = gameObject.transform.rotation;

        

        switch (inputMode)
        {
            // set angle and scale factor manually from inspector
            case InputMode.Manual:
                surveySystem.isTraining = false;
                surveySystem.illusionBox.SetActive(false);
                surveySystem.waitingBox.SetActive(false);
                SurveySystem.timeIsRunning = false;
                break;
            //set sample angles automatically
            case InputMode.Automatic:
                if (surveySystem.waitingBoxText.text == "WAIT" || surveySystem.firstBox.activeSelf)
                {
                    StopSimulation();
                }
                else if (SurveySystem.timeIsRunning)
                {
                    StartSimulation();
                }

                surveySystem.isTraining = false;
                surveySystem.illusionBox.SetActive(false);
                
                if (SurveySystem.number < Randomize.samples.Length)
                {
                    angle = Randomize.samples[SurveySystem.number];
                    scale = Randomize.samples[SurveySystem.number];
                }
                else
                {
                    Debug.Log("This is the end of the experiment");
                    inputMode = InputMode.Manual;
                    SurveySystem.timeIsRunning = false;
                }
                break;

            case InputMode.Training:
                surveySystem.isTraining = true;
                if (surveySystem.waitingBoxText.text == "WAIT")
                {
                    StopSimulation();
                } else
                {
                    StartSimulation();
                }
                //Going through all 
                surveySystem.illusionBox.SetActive(true);
                
                if (SurveySystem.number < Randomize.trainingSamples.Length)
                {
                    angle = Randomize.trainingSamples[SurveySystem.number];
                    scale = Randomize.trainingSamples[SurveySystem.number];

                }
                else
                {
                    Debug.Log("This is the end of the training session");
                    inputMode = InputMode.Manual;
                    SurveySystem.timeIsRunning = false;
                    surveySystem.illusionBox.SetActive(false);
                }
                break;
                
            default:
                break;
        }

        switch (type)
        {
            case RetargetingType.ScalingUp:
                afterScaling = true;
                
                stick.transform.rotation = Quaternion.AngleAxis(0.0f, Vector3.up);
                Vector3 tempLocalScale = new Vector3(calibratedLocalScale.x / 2 * scale, calibratedLocalScale.y, calibratedLocalScale.z);
                stick.transform.localScale = tempLocalScale;
                stick.transform.position = stickStartPosition + Vector3.left * (1 - scale/2) * calibratedLocalScale.x/2;
                retargetedPosition.transform.position = ScaleUp(gameObject.transform.position, scale);
                break;

            case RetargetingType.Rotation:
                float newLength = calibratedLocalScale.x / Mathf.Cos(Mathf.Deg2Rad * angle);

                if (afterScaling)
                {
                    stick.transform.position = stickStartPosition;
                    stick.transform.localScale = calibratedLocalScale;
                    afterScaling = false;
                }
                if (angle != lastAngle)
                {
                    stick.transform.RotateAround(edgePoint, Vector3.up, -lastAngle);


                    stick.transform.localScale = new Vector3(newLength, stick.transform.localScale.y, stick.transform.localScale.z);
                    stick.transform.position = stickStartPosition - Vector3.left * (newLength - calibratedLocalScale.x) / 2;

                    stick.transform.RotateAround(edgePoint, Vector3.up, angle);
                }
                retargetedPosition.transform.position = Rotate(gameObject.transform.position, angle);
                lastAngle = angle;
                break;
            
            case RetargetingType.TrainingScalingUp:

                afterScaling = true;
                /*
                stick.transform.rotation = Quaternion.AngleAxis(0.0f, Vector3.up);
                Debug.Log(calibratedLocalScale.x);
                tempLocalScale = new Vector3(calibratedLocalScale.x /2 * scale, calibratedLocalScale.y, calibratedLocalScale.z);
                stick.transform.localScale = tempLocalScale;
                Debug.Log(stick.transform.localScale.x);
                //stick.transform.position = stickStartPosition + Vector3.left * (1 - scale / 2) * calibratedLocalScale.x / 2;
                stick.transform.position = stickStartPosition + Vector3.left * (1 - scale / 2) * calibratedLocalScale.x / 2;

                //with illusion or not
                if(SurveySystem.number % 2 == 0)
                {
                    retargetedPosition.transform.position = ScaleUp(gameObject.transform.position, scale);
                } else
                {
                    retargetedPosition.transform.position = gameObject.transform.position;
                }
                */

                stick.transform.rotation = Quaternion.AngleAxis(0.0f, Vector3.up);
                tempLocalScale = new Vector3(calibratedLocalScale.x / 2 * scale, calibratedLocalScale.y, calibratedLocalScale.z);
                stick.transform.localScale = tempLocalScale;
                stick.transform.position = stickStartPosition + Vector3.left * (1 - scale / 2) * calibratedLocalScale.x / 2;
                retargetedPosition.transform.position = ScaleUp(gameObject.transform.position, scale);
                break;


                break;

            case RetargetingType.TrainingRotation:

                newLength = calibratedLocalScale.x / Mathf.Cos(Mathf.Deg2Rad * angle);

                if (afterScaling)
                {
                    stick.transform.position = stickStartPosition;
                    stick.transform.localScale = calibratedLocalScale;
                    afterScaling = false;
                }

                if (angle != lastAngle)
                {
                    stick.transform.RotateAround(edgePoint, Vector3.up, -lastAngle);


                    stick.transform.localScale = new Vector3(newLength, stick.transform.localScale.y, stick.transform.localScale.z);
                    stick.transform.position = stickStartPosition - Vector3.left * (newLength - calibratedLocalScale.x) / 2;

                    stick.transform.RotateAround(edgePoint, Vector3.up, angle);
                }

                if (SurveySystem.number % 2 == 0)
                {
                    retargetedPosition.transform.position = Rotate(gameObject.transform.position, angle);
                }
                else
                {
                    retargetedPosition.transform.position = ScaleUpForRotation(gameObject.transform.position, angle);
                }
                lastAngle = angle;


                break;
            
            default:
                break;
        }
        
    }

    private Vector3 ScaleUp(Vector3 vector, float scale)
    {
        //Vector3 newPosition = new Vector3 ((retargetedStartPoint.x - (startPoint.x - vector.x) / scale), vector.y, vector.z);
        Vector3 newPosition = new Vector3 ((edgePoint.x - (edgePoint.x - vector.x) / 2 * scale), vector.y, vector.z);
        return newPosition;
    }

    private Vector3 Rotate_ConstantLength(Vector3 vector, float angle)
    {
        float x = vector.x;
        float y = vector.y;
        float z = vector.z;
        
        float zOffset = Mathf.Abs(edgePoint.x - x) * Mathf.Sin(Mathf.Deg2Rad * angle * 1.0f);
        float xOffset = zOffset * Mathf.Tan(Mathf.Deg2Rad * angle / 2.0f * 1.0f);
        Vector3 newPosition = new Vector3(x - xOffset, y, z - zOffset);
        return newPosition;
    }

    private Vector3 Rotate(Vector3 vector, float angle)
    {
        float x = vector.x;
        float y = vector.y;
        float z = vector.z;

        float zOffset = (x - edgePoint.x) * Mathf.Tan(Mathf.Deg2Rad * angle * 1.0f);
        Vector3 newPosition = new Vector3(x, y, z - zOffset);
        return newPosition;
    }

    private Vector3 ScaleUpForRotation(Vector3 vector, float angle)
    {
        float x = vector.x;
        float y = vector.y;
        float z = vector.z;

        Vector3 newPosition = new Vector3(edgePoint.x - (edgePoint.x - vector.x) / Mathf.Cos(Mathf.Deg2Rad * angle * 1.0f), vector.y, edgePoint.z - (edgePoint.z - vector.z) / Mathf.Cos(Mathf.Deg2Rad * angle * 1.0f));
        return newPosition;
    }

    private void Initialize()
    {
        if (!isInitialized)
        {
            /*
            //locate a stick between two tracker
            stickStartPosition = (leftEdge.transform.position + rightEdge.transform.position) / 2 - new Vector3 (0.004f, 0.045f, 0.0f);
            stick.transform.position = stickStartPosition;
            
            //scale to match physical stick
            float a = Vector3.Distance(leftEdge.transform.position, rightEdge.transform.position) / 67.5f;
            calibratedLocalScale = new Vector3(a * 58.0f, a * 2.7f, a * 2.7f);
            stick.transform.localScale = calibratedLocalScale;
            */
            

            //When trackers are in the corners of the platform
            //scale to match physical stick
            float a = Vector3.Distance(leftEdge.transform.position, rightEdge.transform.position) / 76.0f;
            calibratedLocalScale = new Vector3(a * 60.0f, a * 2.8f, a * 2.8f);
            stick.transform.localScale = calibratedLocalScale;

            //locate a stick between two tracker
            stickStartPosition = (leftEdge.transform.position + rightEdge.transform.position) / 2 - Vector3.Cross((rightEdge.transform.position - leftEdge.transform.position), Vector3.up).normalized * a * 69.85f;
            //stickStartPosition = (leftEdge.transform.position + rightEdge.transform.position) / 2 + new Vector3(0.04f, 0, a * 68.0f + calibratedLocalScale.z / 2);
            stick.transform.position = stickStartPosition;
            

            floor.transform.position = stickStartPosition - Vector3.up * a * 5.0f - stick.transform.localScale / 2;
            surveySystem.waitingBox.transform.position = stickStartPosition + new Vector3(0, floor.transform.position.y + 0.1f, -90.0f* a);
            //set  the top left edge of the stick 
            Vector3 stickLeftOffset = (-1) * stick.transform.right * (stick.transform.localScale.x / 2f) * (1f);
            Vector3 stickTopOffset = stick.transform.up * (stick.transform.localScale.y / 2f) * (1f);
            //Vector3 retPosOffset = stick.transform.up * (retargetedPosition.transform.localScale.y / 2f) * (1f);
            //edgePoint = stick.transform.position + stickLeftOffset + stickTopOffset + retPosOffset;
            edgePoint = stick.transform.position + stickLeftOffset + stickTopOffset + new Vector3(0.5f * a, 0, 0);

            
            isInitialized = true;
        }
    }

    public void StopSimulation()
    {
        stick.GetComponent<MeshRenderer>().enabled = false;
    }

    public void StartSimulation()
    {
        stick.GetComponent<MeshRenderer>().enabled = true;
    }
}
