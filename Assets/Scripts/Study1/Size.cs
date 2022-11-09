using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Size : MonoBehaviour
{
    [Range(240, 280)]
    public int shoeSize;
    [SerializeField]
    private bool isAdjusted;
    //public GameObject shoe;
    private int delta;

    public GameObject tracker;

    public GameObject shoeEnd;
    public GameObject sole;
    public GameObject stick;

    [SerializeField]
    private bool isCalibrated = false;

    private void OnValidate()
    {
        shoeSize = (shoeSize / 10) * 10;
    }

    // Start is called before the first frame update
    void Start()
    {
        delta =(int) (shoeSize - 240) / 10;
        isAdjusted = false;
    }

    // Update is called once per frame
    void Update()
    {
        float coef = 0.01f;
        sole.transform.GetChild(0).GetChild(0).localScale = new Vector3((9.0f + delta * 0.3f) * coef / 0.11f, 1, shoeSize / 10.0f * coef / 0.279f);

        if (!isCalibrated)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                CalibrateShoe(0.01f);
            }
        }
    }

    void CalibrateShoe(float coef)
    {
        float trackerX = tracker.transform.position.x;
        Debug.Log("tracker " + trackerX);
        float stickLeftOffsetX = (stick.transform.position.x - (stick.transform.localScale.x / 2f) * (1f));
        Debug.Log(stickLeftOffsetX);
        float halfShoeLength = (float)shoeSize / 20.0f * coef;
        Debug.Log(halfShoeLength);
        float shift = halfShoeLength - Mathf.Abs(trackerX - stickLeftOffsetX);
        Debug.Log("z cal:" + shift);
        Debug.Log(sole.transform.GetChild(0).GetChild(0).localPosition);

        float trackerY = tracker.transform.position.y;
        float shiftY = stick.transform.position.y - trackerY + 0.015f;
        Debug.Log("y cal:" + shiftY);
        Debug.Log(sole.transform.GetChild(0).GetChild(0).localPosition);
        sole.transform.GetChild(0).GetChild(0).localPosition = new Vector3(sole.transform.GetChild(0).GetChild(0).localPosition.x, shiftY, shift);




    }
}
