using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HW_PinController : MonoBehaviour
{
    //scale of the pin
    public float xScale;
    public float zScale;
    public float raycastHeight;

    //locate trackers
    public GameObject leftTopCorner;
    public GameObject rightTopCorner;

    //distance from right top corner
    public float distanceX;
    public float distanceZ;
    public float distanceBetweenTrackers;
    [HideInInspector]
    public Vector3 calibratedLocalScale;
    private Vector3 rodStartPosition;
    [HideInInspector]
    public Vector3 pivotPoint;

    //scale factor (VR / physical world)
    [HideInInspector]
    public float a;

    private bool isInitialized = false;

    //private Vector3 stickStartPosition;
    
    private bool hasInstantiated = false;

    private float max = 0.0f;
    [HideInInspector]
    public float[,] hitDistances = new float[20, 20];
    private float[,] pinHeight;
    private float yPosition;
    private GameObject[,] pinArray;

    //Move one pin
    public float interval;
    //Tilt rod by this angle
    [Range(0, 45)]
    public float angle;
    private float prevAngle;
    [Range(1, 2)]
    public float scale;
    [Range(0, 10)]
    [SerializeField] private int level;

    public GameObject pin;
    public GameObject sphere;
    public GameObject sphereVR;
    public GameObject box;
    public GameObject rod;
    public GameObject stairsBox;
    private GameObject tiltedRod;
    private GameObject tiltedRodVR;

    public enum Geometry
    {
        Sphere,
        Rod,
        Stairs
    };
    public Geometry geometry;

    public enum Mode
    {
        VR,
        Physical
    }
    public Mode mode;

    public enum RetargetingType
    {
        ScalingUp,
        Rotation
    };
    public RetargetingType type;

    //virtual image (red ball) of actual position(white ball)
    public GameObject tracker;
    public GameObject retargetedPosition;
    public enum InputMode
    {
        Manual,
        Automatic
    }
    public InputMode inputMode;

    private Vector3 sphereStartScale;

    private void Awake()
    {
        sphere.GetComponent<MeshRenderer>().enabled = false;
        pinHeight = new float[20, 20];
        pinArray = new GameObject[20, 20];
        sphereStartScale = sphere.transform.localScale;
    }

    void Update()
    {
        Initialize();
        InstantiatePins();
        StartRaycast();
        ChooseMode();
        ChooseGeometry();
        Retarget();
    }

    // set each pin's height
    private void SetPinHeight(GameObject pin, float height)
    {
        pin.transform.position = new Vector3(pin.transform.position.x, yPosition + height, pin.transform.position.z);
    }

    //map the hit distances
    private static int Map(float value, float fromLow, float fromHigh, int toLow, int toHigh)
    {
        return (int)((value - fromLow) * (float)(toHigh - toLow) / (fromHigh - fromLow) + (float)(toLow));
    }

    //initialize the position and scake of pins, rod
    private void Initialize()
    {
        if (!isInitialized)
        {
            //scale to match physical stick
            a = Vector3.Distance(leftTopCorner.transform.position, rightTopCorner.transform.position) / distanceBetweenTrackers;
            calibratedLocalScale = new Vector3(a * 60.0f, a * 15.0f, a * 2.7f);

            
            //locate a stick between two tracker
            //signs change depending on the orientation of axes
            transform.position = leftTopCorner.transform.position +  new Vector3(a * distanceX, 0, a * distanceZ);
            rodStartPosition = transform.position + new Vector3(28.5f * a, 0.05f, 57.0f * a);
            tiltedRod = Instantiate(rod, rodStartPosition, Quaternion.identity);
            tiltedRod.transform.localScale = calibratedLocalScale;

            tiltedRodVR = Instantiate(rod, rodStartPosition, Quaternion.identity);
            tiltedRodVR.transform.localScale = calibratedLocalScale;
            tiltedRodVR.gameObject.layer = 1 << 2;

            //rotation pivot point
            pivotPoint = transform.position + new Vector3(0, 0, 57.0f * a);

            yPosition = transform.position.y;


            //box
            box.transform.localScale = new Vector3(a * 60.0f, a * 15.0f, a * 60.0f);
            box.transform.position = transform.position + new Vector3(28.5f * a, 0, 28.5f * a);
            sphere.transform.position = transform.position + new Vector3(28.5f * a, calibratedLocalScale.y/2, 28.5f * a);
            sphereVR.transform.position = transform.position + new Vector3(28.5f * a, calibratedLocalScale.y/2, 28.5f * a);
            stairsBox.transform.position = transform.position + new Vector3(28.5f * a, 0, 28.5f * a);
            stairsBox.transform.localScale = stairsBox.transform.localScale * a;
            isInitialized = true;
        }
    }

    //Instantiate Pins
    private void InstantiatePins()
    {
        if (!hasInstantiated)
        {
            // instantiate 20x20 pins
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    pinArray[i, j] = Instantiate(pin, transform.position + new Vector3(i * xScale * a, 0, j * zScale*a), Quaternion.identity, gameObject.transform);
                    pinArray[i, j].transform.localScale = pinArray[i, j].transform.localScale * a;
                }
            }
        }
        hasInstantiated = true;
    }

    //Hit raycast
    private void StartRaycast()
    {
        int layerMask = 1 << 4;
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                Vector3 raycastOrigin = transform.position + new Vector3(i * 3 * a, raycastHeight, j * 3 * a);
                RaycastHit hit;
                if (Physics.Raycast(raycastOrigin, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
                {
                    hitDistances[i, j] = raycastHeight - pinArray[i, j].transform.localScale.y / 2 - hit.distance;
                    if (hitDistances[i, j] < 0)
                    {
                        hitDistances[i, j] = 0;
                    }
                    //Debug.DrawRay(raycastOrigin, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                }
                else
                {
                    hitDistances[i, j] = 0;
                }

                if (hitDistances[i, j] > max)
                {
                    max = hitDistances[i, j];
                }
            }
        }
    }

    //VR or Physical
    private void ChooseMode()
    {
        switch (mode)
        {
            case Mode.VR:
                box.GetComponent<MeshRenderer>().enabled = true;
                sphereVR.GetComponent<MeshRenderer>().enabled = true;
                tiltedRod.GetComponent<MeshRenderer>().enabled = false;
                tiltedRodVR.GetComponent<MeshRenderer>().enabled = true;

                //disable pins in VR mode except One Pin Geometry
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        pinArray[i, j].GetComponent<MeshRenderer>().enabled = false;
                        
                    }
                }

                //enable stairs in VR mode
                stairsBox.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                stairsBox.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;
                break;
            case Mode.Physical:
                box.GetComponent<MeshRenderer>().enabled = false;
                sphereVR.GetComponent<MeshRenderer>().enabled = false;
                tiltedRod.GetComponent<MeshRenderer>().enabled = false;
                tiltedRodVR.GetComponent<MeshRenderer>().enabled = false;

                //enable all pins
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        pinArray[i, j].GetComponent<MeshRenderer>().enabled = true;
                    }
                }
                //disable stairs
                stairsBox.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                stairsBox.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
                break;
            default:
                break;
        }
    }
    
    //Sphere, Rod, Stairs
    private void ChooseGeometry()
    {
        //Choose the geometry 
        switch (geometry)
        {
            // simulate sphere
            case Geometry.Sphere:
                type = RetargetingType.ScalingUp;
                ImitateSphere();
                break;

            //tilt rod
            case Geometry.Rod:
                type = RetargetingType.Rotation;
                ImitateRod();             
                break;
            
            case Geometry.Stairs:
                type = RetargetingType.ScalingUp;
                ImitateStairs();
                break;
            
            default:
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        SetPinHeight(pinArray[i, j], yPosition);
                    }
                }
                break;
            }
    }

    //Sphere
    private void ImitateSphere()
    {
        tiltedRod.SetActive(false);
        //tiltedRodVR.SetActive(false);
        sphere.SetActive(true);
        sphereVR.SetActive(true);
        box.SetActive(true);
        stairsBox.SetActive(false);

        float tempInterval = (sphere.transform.position.y + sphere.transform.localScale.y/2) / 10.0f;
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                pinHeight[i, j] = Map(hitDistances[i, j], 0, sphere.transform.position.y + sphere.transform.localScale.y/2, 0, 10);
               
                if (pinHeight[i, j] >= 0)
                {
                    SetPinHeight(pinArray[i, j], pinHeight[i,j] * tempInterval);
                }
            }
        }
    }

    //Rod
    private void ImitateRod()
    {
        sphere.SetActive(false);
        sphereVR.SetActive(false);
        box.SetActive(false);
        stairsBox.SetActive(false);
        tiltedRod.SetActive(true);
        tiltedRodVR.SetActive(true);
        float newLength = calibratedLocalScale.x / Mathf.Cos(Mathf.Deg2Rad * angle);
        if (prevAngle != angle)
        {
            tiltedRod.transform.RotateAround(pivotPoint, Vector3.up, -prevAngle);

            tiltedRod.transform.localScale = new Vector3(newLength, tiltedRod.transform.localScale.y, tiltedRod.transform.localScale.z);
            tiltedRod.transform.position = rodStartPosition - Vector3.left * (newLength - calibratedLocalScale.x) / 2;

            tiltedRod.transform.RotateAround(pivotPoint, Vector3.up, angle);
        }

        
        if (prevAngle != angle)
        {
            tiltedRodVR.transform.RotateAround(pivotPoint, Vector3.up, -prevAngle);

            tiltedRodVR.transform.localScale = new Vector3(newLength, tiltedRod.transform.localScale.y, tiltedRod.transform.localScale.z);
            tiltedRodVR.transform.position = rodStartPosition - Vector3.left * (newLength - calibratedLocalScale.x) / 2;

            tiltedRodVR.transform.RotateAround(pivotPoint, Vector3.up, angle);
        }
        
        //if there is an illusion, then only virtual rod rotates
        
        if (HW_Randomize.illusions[HW_SurveySystem.number])
        {
            tiltedRod.transform.RotateAround(pivotPoint, Vector3.up, -prevAngle);
            tiltedRod.transform.localScale = new Vector3(newLength, tiltedRod.transform.localScale.y, tiltedRod.transform.localScale.z);
            tiltedRod.transform.position = rodStartPosition - Vector3.left * (newLength - calibratedLocalScale.x) / 2;
        } else
        {
            tiltedRod.transform.RotateAround(pivotPoint, Vector3.up, -prevAngle);

            tiltedRod.transform.localScale = new Vector3(newLength, tiltedRod.transform.localScale.y, tiltedRod.transform.localScale.z);
            tiltedRod.transform.position = rodStartPosition - Vector3.left * (newLength - calibratedLocalScale.x) / 2;

            tiltedRod.transform.RotateAround(pivotPoint, Vector3.up, angle);
        }
        
        prevAngle = angle;

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                pinHeight[i, j] = hitDistances[i, j];
                if (pinHeight[i, j] >= 0)
                {
                    SetPinHeight(pinArray[i, j], pinHeight[i, j]);
                }
            }
        }
    }

    //Stairs
    private void ImitateStairs()
    {
        //tiltedRodVR.SetActive(false);
        tiltedRod.SetActive(false);
        sphere.SetActive(false);
        sphereVR.SetActive(false);
        box.SetActive(false);
        stairsBox.SetActive(true);
        if (level >= 5)
        {
            level = 5;
        }
        stairsBox.transform.GetChild(0).position = new Vector3(stairsBox.transform.GetChild(0).position.x, yPosition + level * interval * a, stairsBox.transform.GetChild(0).position.z);
        stairsBox.transform.GetChild(1).position = new Vector3(stairsBox.transform.GetChild(1).position.x, yPosition + 2 * level * interval*a, stairsBox.transform.GetChild(1).position.z);
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                pinHeight[i, j] = hitDistances[i, j];
                if (pinHeight[i, j] >= 0)
                {
                    SetPinHeight(pinArray[i, j], pinHeight[i, j]);
                }
            }
        }
    }

    //Angle redirection
    private Vector3 Rotate(Vector3 vector, float angle)
    {
        float x = vector.x;
        float y = vector.y;
        float z = vector.z;
        Vector3 newPosition;
        float zOffset = (x - pivotPoint.x) * Mathf.Tan(Mathf.Deg2Rad * angle * 1.0f);
        newPosition = new Vector3(x, y, z - zOffset);
        /*
        if (HW_Randomize.illusions[HW_SurveySystem.number])
        {
            float zOffset = (x - pivotPoint.x) * Mathf.Tan(Mathf.Deg2Rad * angle * 1.0f);
            newPosition = new Vector3(x, y, z - zOffset);
            
        } else
        {
            newPosition = new Vector3(pivotPoint.x - (pivotPoint.x - vector.x) / Mathf.Cos(Mathf.Deg2Rad * angle * 1.0f), vector.y, pivotPoint.z - (pivotPoint.z - vector.z) / Mathf.Cos(Mathf.Deg2Rad * angle * 1.0f));
        }*/
        return newPosition;

    }

    //Scale up redirection
    private Vector3 ScaleUp(Vector3 vector, float scale)
    {
        //Vector3 newPosition = new Vector3 ((retargetedStartPoint.x - (startPoint.x - vector.x) / scale), vector.y, vector.z);
        Vector3 newPosition = new Vector3((sphere.transform.position.x - (sphere.transform.position.x - vector.x) / scale), sphere.transform.position.y - (sphere.transform.position.y - vector.y) / scale, vector.z);
        sphere.transform.localScale = sphereStartScale * scale;
        return newPosition;
    }

    //Manual or Automatic
    private void ChooseInputMode()
    {
        switch (inputMode)
        {
            // set angle and scale factor manually from inspector
            case InputMode.Manual:
                HW_SurveySystem.timeIsRunning = false;
                break;

            //set sample angles automatically
            case InputMode.Automatic:
                if (HW_SurveySystem.number < HW_Randomize.samples.Length)
                {
                    angle = HW_Randomize.samples[HW_SurveySystem.number];
                    scale = HW_Randomize.samples[HW_SurveySystem.number];
                }
                else
                {
                    Debug.Log("This is the end of the experiment");
                    inputMode = InputMode.Manual;
                    HW_SurveySystem.timeIsRunning = false;
                }
                break;
            default:
                break;
        }
    }

    private void ChooseRedirectionType()
    {
        switch (type)
        {
            
            case RetargetingType.ScalingUp:
                retargetedPosition.transform.position = ScaleUp(tracker.gameObject.transform.position, scale);
                break;
            case RetargetingType.Rotation:
                
                retargetedPosition.transform.position = Rotate(tracker.gameObject.transform.position, angle);
                break;
            default:
                break;
        }
    }

    //Redirecting tracker
    private void Retarget()
    {
        retargetedPosition.transform.rotation = tracker.gameObject.transform.rotation;

        ChooseInputMode();

        ChooseRedirectionType();
    }
}


