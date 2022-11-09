using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class SoleXMLManager : MonoBehaviour
{
    public void SaveItems(SoleDatabase userDatabase, string userID)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(SoleDatabase));
        FileStream fileStream = new FileStream("C:/Users/MAKinteract_Leopard/Documents/sole_data_" + userID + ".xml", FileMode.OpenOrCreate);
        serializer.Serialize(fileStream, userDatabase);
        fileStream.Close();
    }
}


[System.Serializable]
public class SoleDataEntry
{
    public string studyPart;
    public float sample;
    public bool isTesting;
    public float time;
    public int soleA;
    public int soleB;
    public int soleC;
    public int soleD;
    public int soleE;
    public int soleF;
    public int noTouch;

    public SoleDataEntry()
    {
        this.studyPart = "";
        this.sample = 0.0f;
        this.isTesting = false;
        this.time = 0.0f;
        soleA = 0;
        soleB = 0;
        soleC = 0;
        soleD = 0;
        soleE = 0;
        soleF = 0;
        noTouch = 0;

    }

    public SoleDataEntry(string studyPart, float sample, bool isTesting, float time, int[] arr)
    {
        this.studyPart = studyPart;
        this.sample = sample;
        this.isTesting = isTesting;
        this.time = time;
        soleA = arr[0];
        soleB = arr[1];
        soleC = arr[2];
        soleD = arr[3];
        soleE = arr[4];
        soleF = arr[5];
        noTouch = arr[6];

    }

}

[System.Serializable]
public class SoleDatabase
{
    public List<SoleDataEntry> dataList;

    public SoleDatabase()
    {
        dataList = new List<SoleDataEntry>();
    }

}
