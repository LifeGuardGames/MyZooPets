using UnityEngine;
using System;
using System.Collections;

public class FloatyUtil {
    private const int NGUI_FLOAT_YPOSITION = 100; //use this to make floaty object move up
    private const float FLOAT_TIME = 3.0f; //duration of the float

    //---------------------------------------------------- 
    // SpawnFloatyText()
    // This spawns a floaty text that disappears in FLOAT_TIME
    // Params: parent, textSize, text 
    //---------------------------------------------------- 
    public static void SpawnFloatyText(Hashtable option){
        GameObject floatyText = (GameObject) Resources.Load("FloatyText");
        GameObject floaty;

        if(option.ContainsKey("parent")){
            floaty = LgNGUITools.AddChildWithPosition((GameObject) option["parent"], floatyText);
        }
        else{
            Debug.Log("SpawnFloatyText requires a parent");
            return;
        }

        if(option.ContainsKey("textSize")){
            int textSize = (int) option["textSize"];
            floaty.transform.Find("Label").localScale = new Vector3(textSize, textSize, 1);
        }

        if(option.ContainsKey("text")){
            floaty.transform.Find("Label").GetComponent<UILabel>().text = (string) option["text"];
        }

        floaty.GetComponent<FloatyController>().floatingUpPos = new Vector3(0, NGUI_FLOAT_YPOSITION, 0);
        floaty.GetComponent<FloatyController>().floatingTime = FLOAT_TIME;
    }

    public static void SpawnFloatyImage(Hashtable option){

    }

}