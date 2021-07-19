using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Retarget : MonoBehaviour
{
    public enum RetargetingType
    {
        ScalingUp,
        Rotation
    };

    public RetargetingType type;

    [Range(0.5f, 2)] public float scale;
    [Range(0, 45)] public float angle;
    private float lastAngle;
    public GameObject retargetedPosition;
    public GameObject stick;

    [SerializeField] private Vector3 startPoint;
    private Vector3 retargetedStartPoint;
    private Vector3 lastStartPoint;
    private Vector3 centerPoint;
    private Vector3 edgePoint;


    void OnValidate()
    {
        startPoint = lastStartPoint;
        retargetedStartPoint = lastStartPoint;
    }
    // Start is called before the first frame update
    void Awake()
    {

        startPoint = gameObject.transform.position;
        retargetedStartPoint = startPoint;
        
        //set  the top left edge of the stick 
        Vector3 stickLeftOffset = (-1) * stick.transform.right * (stick.transform.localScale.x / 2f) * (1f);
        Vector3 stickTopOffset = stick.transform.up * (stick.transform.localScale.y / 2f) * (1f);
        Vector3 retPosOffset = stick.transform.up * (retargetedPosition.transform.localScale.y / 2f) * (1f);
        edgePoint = stick.transform.position + stickLeftOffset + stickTopOffset + retPosOffset;
    }

    // Update is called once per frame
    void Update()
    {
        lastStartPoint = gameObject.transform.position;
        switch (type)
        {
            case RetargetingType.ScalingUp:
                retargetedPosition.transform.position = ScaleUp(gameObject.transform.position, scale);
                break;
            case RetargetingType.Rotation:
                if (angle != lastAngle)
                {
                    stick.transform.RotateAround(edgePoint, Vector3.up, lastAngle);
                    stick.transform.RotateAround(edgePoint, Vector3.up, -angle);
                }
                //stick.transform.rotation = Quaternion.AngleAxis(-angle, Vector3.up);
                //stick.transform.position = centerPoint + (retargetedStartPoint - tempStartPoint);
                retargetedPosition.transform.position = Rotate(gameObject.transform.position, angle);
                lastAngle = angle;
                break;
            default:
                break;
        }
        
    }

    private Vector3 ScaleUp(Vector3 vector, float scale)
    {
        Vector3 newPosition = retargetedStartPoint - (startPoint - vector) / scale;
        return newPosition;
    }

    private Vector3 Rotate(Vector3 vector, float angle)
    {
        Vector3 tempPosition = retargetedStartPoint - (startPoint - vector);
        Vector3 newPosition = Quaternion.AngleAxis(angle * (-1), Vector3.up) * (tempPosition - retargetedStartPoint) + startPoint;
        return newPosition;
    }




}
