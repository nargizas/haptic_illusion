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
    public GameObject stickEnd;

    public GameObject sole;
    //public GameObject shoeEnd;


    public static Vector3 calibratedLocalScale;
    private Vector3 stickStartPosition;


    //virtual image (red ball) of actual position(white ball)
    public GameObject retargetedPosition;

    // the edge of the stick
    public static Vector3 edgePoint;

    public static float coef;

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

    [Range(200, 300)]
    public int shoeSize;
    //public GameObject shoe;
    private int delta;

    [SerializeField]
    private bool shoeCalibrated = false;

    private float angleZ;

    //private void OnValidate()
    //{
    //    shoeSize = (shoeSize / 10) * 10;
    //}

    private void Start()
    {
        randomize = GetComponent<Randomize>();
        surveySystem = GetComponent<SurveySystem>();
        delta = (int)(shoeSize - 240) / 10;
    }

    // Update is called once per frame
    void Update()
    {
        
        
        Initialize();

        
        retargetedPosition.transform.rotation = gameObject.transform.rotation;

        if (shoeCalibrated == false)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                float trackerX = transform.position.x;
                Debug.Log("tracker " + trackerX);
                float stickLeftOffsetX = (stick.transform.position.x + (stick.transform.localScale.x / 2f) * (1f));
                Debug.Log(stickLeftOffsetX);
                float halfShoeLength = (float)shoeSize / 20.0f * coef;
                Debug.Log(halfShoeLength);
                float shift = halfShoeLength - Mathf.Abs(trackerX - stickLeftOffsetX);
                Debug.Log("z cal:" + (-1 * shift));
                Debug.Log(sole.transform.GetChild(0).GetChild(0).localPosition);

                float stickEndY = stickEnd.transform.position.y;
                float trackerY = transform.position.y;
                float shiftY = stick.transform.position.y - trackerY + calibratedLocalScale.y / 2;
                Debug.Log("y cal:" + shiftY);
                Debug.Log(sole.transform.GetChild(0).GetChild(0).localPosition);
                sole.transform.GetChild(0).GetChild(0).localPosition = new Vector3(sole.transform.GetChild(0).GetChild(0).localPosition.x, shiftY, -shift);



                shoeCalibrated = true;

            }
        }

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
                //stick.transform.Rotate(0,0, angleZ, Space.Self);
                Vector3 tempLocalScale = new Vector3(calibratedLocalScale.x / 2 * scale, calibratedLocalScale.y, calibratedLocalScale.z);
                stick.transform.localScale = tempLocalScale;
                stick.transform.position = stickStartPosition + Vector3.left * (1 - scale/2) * calibratedLocalScale.x/2;
                retargetedPosition.transform.position = ScaleUp(gameObject.transform.position, scale);
                break;

            case RetargetingType.Rotation:
                float newLength = calibratedLocalScale.x / Mathf.Cos(Mathf.Deg2Rad * angle);
                //stick.transform.rotation = Quaternion.AngleAxis(0.0f, Vector3.up);
                //stick.transform.Rotate(0, 0, angleZ, Space.Self);
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
        if (vector.y < (rightEdge.transform.position.y + leftEdge.transform.position.y) / 2)
        {
            vector.y = (rightEdge.transform.position.y + leftEdge.transform.position.y) / 2;
        }
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
        if (vector.y < (rightEdge.transform.position.y + leftEdge.transform.position.y) / 2)
        {
            vector.y = (rightEdge.transform.position.y + leftEdge.transform.position.y) / 2;
        }
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
            calibratedLocalScale = new Vector3(a * 59.0f, a * 2.8f, a * 2.8f);
            stick.transform.localScale = calibratedLocalScale;

            /*Vector3 edgeVector = rightEdge.transform.position - leftEdge.transform.position;
            angleZ = Vector3.Angle(edgeVector, Vector3.right);*/


            //locate a stick between two tracker
            //stickStartPosition = (leftEdge.transform.position + rightEdge.transform.position) / 2 - Vector3.Cross((rightEdge.transform.position - leftEdge.transform.position), Vector3.up).normalized * a * 69.85f;(leftEdge.transform.position.z + rightEdge.transform.position.z) / 2)
            stickStartPosition = (leftEdge.transform.position + rightEdge.transform.position) / 2 - Vector3.Cross((rightEdge.transform.position - leftEdge.transform.position), Vector3.up).normalized * (stickEnd.transform.position.z - leftEdge.transform.position.z);
            //stickStartPosition = (leftEdge.transform.position + rightEdge.transform.position) / 2 + new Vector3(0.04f, 0, a * 68.0f + calibratedLocalScale.z / 2);

            //-3.0f*a to shift the stick to correct position
            //stickStartPosition = new Vector3((leftEdge.transform.position.x + rightEdge.transform.position.x) / 2 - 6.0f*a, (leftEdge.transform.position.y + rightEdge.transform.position.y) / 2, stickEnd.transform.position.z);
            /*Debug.Log(stickEnd.transform.position.x);
            Debug.Log((stick.transform.position.x - (stick.transform.localScale.x / 2f) * (1f)));
            float shift = stickEnd.transform.position.x - (stick.transform.position.x - (stick.transform.localScale.x / 2f) * (1f))*/
            stick.transform.position = stickStartPosition;
            

            floor.transform.position = stickStartPosition  - stick.transform.localScale / 2; //- Vector3.up * a * 5.0f
            //floor.transform.Rotate(angleZ, 0, 0, Space.Self);
            surveySystem.waitingBox.transform.position = stickStartPosition + new Vector3(0, floor.transform.position.y + 0.1f, -90.0f* a);
            //set  the top left edge of the stick 
            Vector3 stickLeftOffset = (-1) * stick.transform.right * (stick.transform.localScale.x / 2f) * (1f);
            Vector3 stickTopOffset = stick.transform.up * (stick.transform.localScale.y / 2f) * (1f);
            //Vector3 retPosOffset = stick.transform.up * (retargetedPosition.transform.localScale.y / 2f) * (1f);
            //edgePoint = stick.transform.position + stickLeftOffset + stickTopOffset + retPosOffset;
            edgePoint = stick.transform.position + stickLeftOffset + stickTopOffset + new Vector3(0.5f * a, 0, 0);
            Debug.Log(a);

            sole.transform.GetChild(0).GetChild(0).localScale = new Vector3((9.0f + delta * 0.3f) * a / 0.11f, 1, ((float) shoeSize / 10.0f * a )/ 0.279f);
            retargetedPosition.transform.GetChild(0).GetChild(0).localScale = new Vector3((9.0f + delta * 0.3f) * a / 0.11f, 1, ((float)shoeSize / 10.0f * a) / 0.279f);
            coef = a;
            isInitialized = true;

        }
    }

    //shift the cubes for sole to match the position of tracker on the actual shoe
    /*private void CalibrateShoe()
    {
        //find the axis of the shoe
        Vector3 shoeAxis = shoe.transform.GetChild(0).GetChild(2).position - shoe.transform.GetChild(0).GetChild(0).position;

        //find vector from the edge pf the shoe to the tracker
        Vector3 trackerToShoeEdge = gameObject.transform.position - (shoeEnd.transform.position - new Vector3(coef*3.0f, 0, 0));

        //find the projectile
        Vector3 projectile = Vector3.Project(trackerToShoeEdge, shoeAxis);

        //get the difference and shift the shoe according to it
        shoe.transform.GetChild(0).position = projectile - shoeAxis;

        Debug.Log(shoe.transform.GetChild(0).position);
        shoeCalibrated = true;
    }
*/

    //General form
    /*
    private void ShiftTracker(GameObject shoe, GameObject tracker, GameObject. shoeEdge)
    {
        Vector3 shoeAxis = shoe.transform.GetChild(0).GetChild(2).position - shoe.transform.GetChild(0).GetChild(0).position;
        Vector3 trackerToShoeEdge = gameObject.transform.position - (shoeEnd.transform.position - new Vector3(coef * 3.0f, 0, 0));
        Vector3 projectile = Vector3.Project(trackerToShoeEdge, shoeAxis);
        shoe.transform.GetChild(0).position = projectile - shoeAxis;
    }*/

    public void StopSimulation()
    {
        stick.GetComponent<MeshRenderer>().enabled = false;
    }

    public void StartSimulation()
    {
        stick.GetComponent<MeshRenderer>().enabled = true;
    }
}
