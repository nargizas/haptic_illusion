using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinController : MonoBehaviour
{
    public float xScale;
    public float zScale;

    public enum Geometry
    {
        OnePin,
        Sphere,
        Rod,
        Stairs
    };
    public Geometry geometry;

    //Move one pin
    public float interval;
    [Range(1, 20)]
    [SerializeField] private int targetRow;
    [Range(1, 20)]
    [SerializeField] private int targetCol;
    [Range(0, 10)]
    [SerializeField] private int level;

    //Tilt rod by this angle
    [Range(0, 45)]
    [SerializeField] private float angle;

    private float[,] pinHeight;
    private float yPosition;
    private GameObject[,] pinArray;

    public enum Direction
    {
        Horizontal,
        Vertical
    };
    public Direction direction;

    public enum Mode
    {
        VR,
        Physical
    }
    public Mode mode;

    public GameObject pin;
    public GameObject box;
    public GameObject sphere;
    public GameObject rod;
    public GameObject stairsBox;
    private GameObject tiltedRod;


    private RaycastController raycastController;
    private float tempInterval;

    private void Awake()
    {
        pinHeight = new float[20, 20];
        pinArray = new GameObject[20, 20];
        yPosition = transform.position.y;
        raycastController = GetComponent<RaycastController>();
        tiltedRod = Instantiate(rod, new Vector3(0.0f, 10.0f, 0.0f), Quaternion.identity);
    }
    
    void Start()
    {
        // instantiate 20x20 pins
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                pinArray[i, j] = Instantiate(pin, new Vector3(i * xScale, 0, j * zScale), Quaternion.identity, gameObject.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Choose VR or physical modelling
        switch (mode)
        {
            case Mode.VR:
                box.GetComponent<MeshRenderer>().enabled = true;
                sphere.GetComponent<MeshRenderer>().enabled = true;
                tiltedRod.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                //disable pins in VR mode except One Pin Geometry
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {   
                        if(geometry == Geometry.OnePin)
                        {
                            if (pinHeight[i, j] > 0)
                            {
                                pinArray[i, j].GetComponent<MeshRenderer>().enabled = true;
                            }
                            else
                            {
                                pinArray[i, j].GetComponent<MeshRenderer>().enabled = false;
                            }
                        }
                        else
                        {
                            pinArray[i, j].GetComponent<MeshRenderer>().enabled = false;
                        }
                    }
                }
                //enable stairs in VR mode
                stairsBox.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                stairsBox.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;

                break;
            case Mode.Physical:
                box.GetComponent<MeshRenderer>().enabled = false;
                sphere.GetComponent<MeshRenderer>().enabled = false;
                tiltedRod.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
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

        //Choose the geometry 
        switch (geometry)
        {
            //move one pin
            case Geometry.OnePin:
                tiltedRod.SetActive(false);
                sphere.SetActive(false);
                box.SetActive(true);
                stairsBox.SetActive(false);
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        if (i == targetRow - 1 && j == targetCol - 1)
                        {
                            SetPinHeight(pinArray[i, j], interval * level);
                            pinHeight[i, j] = interval * level;
                        }
                        else
                        {
                            SetPinHeight(pinArray[i, j], yPosition);
                            pinHeight[i, j] = 0;
                        }
                    }
                }
                SetPinHeight(pinArray[targetRow-1, targetCol-1], interval * level);
                break;
            // simulate sphere
            case Geometry.Sphere:
                //Debug.Log(raycastController.max);
                tiltedRod.SetActive(false);
                sphere.SetActive(true);
                box.SetActive(true);
                stairsBox.SetActive(false);
                tempInterval = raycastController.max / 10.0f;
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        pinHeight[i, j] = Map(raycastController.hitDistances[i, j], 0, raycastController.max, 0, 10);
                        if (pinHeight[i, j] >= 0)
                        {
                            SetPinHeight(pinArray[i, j], pinHeight[i, j] * tempInterval);
                        }

                    }
                }
                break;

            //tilt rod
            case Geometry.Rod:
                tiltedRod.SetActive(true);
                sphere.SetActive(false);
                box.SetActive(false);
                stairsBox.SetActive(false);
                float newLength = 60.0f / Mathf.Cos(Mathf.Deg2Rad * angle);
                tiltedRod.transform.GetChild(0).localScale = new Vector3(newLength, 15, 2.9f);
                float y = tiltedRod.transform.GetChild(0).localPosition.y;
                float z = tiltedRod.transform.GetChild(0).localPosition.z;
                switch (direction)
                {
                    case Direction.Horizontal:
                        tiltedRod.transform.position = new Vector3((targetRow - 1) * xScale, 10.0f, 0.0f);
                        tiltedRod.transform.rotation = Quaternion.Euler(0.0f, angle + 90.0f, 0.0f);
                        tiltedRod.transform.GetChild(0).localPosition = new Vector3(1.5f - newLength / 2, y, z);
                        break;
                    case Direction.Vertical:
                        tiltedRod.transform.position = new Vector3(0.0f, 10.0f, (targetCol - 1) * zScale);
                        tiltedRod.transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
                        tiltedRod.transform.GetChild(0).localPosition = new Vector3(newLength / 2 - 1.5f, y, z);
                        break;
                    default:
                        break;
                }
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        pinHeight[i, j] = raycastController.hitDistances[i, j];
                        if (pinHeight[i, j] >= 0)
                        {
                            SetPinHeight(pinArray[i, j], pinHeight[i, j]);
                        }

                    }
                }
                break;
            case Geometry.Stairs:
                tiltedRod.SetActive(false);
                sphere.SetActive(false);
                box.SetActive(false);
                stairsBox.SetActive(true);
                if(level >= 5)
                {
                    level = 5;
                }
                stairsBox.transform.GetChild(0).position = new Vector3(stairsBox.transform.GetChild(0).position.x, level * interval, stairsBox.transform.GetChild(0).position.z);
                stairsBox.transform.GetChild(1).position = new Vector3(stairsBox.transform.GetChild(1).position.x, 2 * level * interval, stairsBox.transform.GetChild(1).position.z);
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        pinHeight[i, j] = raycastController.hitDistances[i, j];
                        if (pinHeight[i, j] >= 0)
                        {
                            SetPinHeight(pinArray[i, j], pinHeight[i, j]);
                        }

                    }
                }
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

    // set each pin's height
    private void SetPinHeight(GameObject pin, float height)
    {
        float x = pin.transform.position.x;
        float z = pin.transform.position.z;
        pin.transform.position = new Vector3(x, height, z);
    }

    //map the hit distances
    private static int Map(float value, float fromLow, float fromHigh, int toLow, int toHigh)
    {
        return (int)((value - fromLow) * (float)(toHigh - toLow) / (fromHigh - fromLow) + (float)(toLow));
    }

}

