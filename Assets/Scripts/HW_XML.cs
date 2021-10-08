using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;


public class HW_XML : MonoBehaviour
{

    public void SaveItems(HW_UserDatabase userDatabase, string userID)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(HW_UserDatabase));
        FileStream fileStream = new FileStream("C:/Users/MAKinteract_Leopard/Documents/user_data_" + userID + ".xml", FileMode.OpenOrCreate);
        serializer.Serialize(fileStream, userDatabase);
        fileStream.Close();
    }

}

[System.Serializable]
public class HW_UserDataEntry
{
    public int number;
    public float sample;
    public float timeWithIllusion;
    public float timeWithoutIllusion;
    public float answerTime;
    public int firstAnswer;
    public int secondAnswer;
    public int thirdAnswer;


    public HW_UserDataEntry()
    {
        this.number = 0;
        this.sample = 0.0f;
        this.timeWithIllusion = 0.0f;
        this.timeWithoutIllusion = 0.0f;
        this.answerTime = 0.0f;
        this.firstAnswer = 1;
        this.secondAnswer = 1;
        this.thirdAnswer = 1;
    }

}

[System.Serializable]
public class HW_UserDatabase
{
    public List<HW_UserDataEntry> hw_dataList;

    public HW_UserDatabase()
    {
        hw_dataList = new List<HW_UserDataEntry>();
    }

}
