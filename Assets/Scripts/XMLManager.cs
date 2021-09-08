using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class XMLManager : MonoBehaviour
{
    
    public void SaveItems(UserDatabase userDatabase, string userID)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(UserDatabase));
        FileStream fileStream = new FileStream("C:/Users/MAKinteract_Leopard/Documents/user_data_"+ userID +".xml", FileMode.OpenOrCreate);
        serializer.Serialize(fileStream, userDatabase);
        fileStream.Close();
    }
    


}

[System.Serializable]
public class UserDataEntry
{
    public int number;
    public float time;
    public float sample;
    public string isIllusion;
    public string hasPerceivedIllusion;
    public int rating;

    
    public UserDataEntry()
    {
        this.number = 0;
        this.time = 0.0f;
        this.sample = 0.0f;
        this.isIllusion = "Y";
        this.hasPerceivedIllusion = "Y";
        this.rating = 0;
    }
    
}

[System.Serializable]
public class UserDatabase
{
    public List<UserDataEntry> dataList;

    public UserDatabase()
    {
        dataList = new List<UserDataEntry>();
    }

}