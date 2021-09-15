using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;


public class HW_XMLManager : MonoBehaviour
{

    public void SaveItems(SecondUserDatabase userDatabase, string userID)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(SecondUserDatabase));
        FileStream fileStream = new FileStream("C:/Users/MAKinteract_Leopard/Documents/user_data_" + userID + ".xml", FileMode.OpenOrCreate);
        serializer.Serialize(fileStream, userDatabase);
        fileStream.Close();
    }



}

[System.Serializable]
public class SecondUserDataEntry
{
    public int number;
    public float time;
    public float sample;
    public bool isIllusion;
    public int firstAnswer;
    public int secondAnswer;
    public int thirdAnswer;


    public SecondUserDataEntry()
    {
        this.number = 0;
        this.time = 0.0f;
        this.sample = 0.0f;
        this.isIllusion = true;
        this.firstAnswer = 1;
        this.secondAnswer = 1;
        this.thirdAnswer = 1;
}

}

[System.Serializable]
public class SecondUserDatabase
{
    public List<SecondUserDataEntry> dataList;

    public SecondUserDatabase()
    {
        dataList = new List<SecondUserDataEntry>();
    }

}
