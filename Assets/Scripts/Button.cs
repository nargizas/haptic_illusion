using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{  
    public void ChangeColor()
    {
        gameObject.GetComponent<Image>().color = Color.white;
    }
}
