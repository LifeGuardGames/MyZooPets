using UnityEngine;
using System;
using System.Collections;

public class FloatyUtil {
    private const int NGUI_FLOAT_YPOSITION = 100; //use this to make floaty object move up
    private const float FLOAT_TIME = 3.0f; //duration of the float

    private static GameObject floatyText = null;
    private static GameObject statsFloatyImageText = null;
    //---------------------------------------------------- 
    // SpawnFloatyText()
    // This spawns a floaty text that disappears in FLOAT_TIME
    // Params: parent, textSize, text 
    //---------------------------------------------------- 
    public static void SpawnFloatyText(Hashtable option){
        if(floatyText == null)
            floatyText = (GameObject) Resources.Load("FloatyText");

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


    //---------------------------------------------------- 
    // SpawnPetFloatyImageText()
    // Spawns floaty image and text above pet's head to show
    // change in stats
    // Params: parent, text, spriteName
    //---------------------------------------------------- 
    public static void SpawnStatsFloatyImageText(Hashtable option){
        if(statsFloatyImageText == null)
            statsFloatyImageText = (GameObject) Resources.Load("StatsFloatyImageText");

        GameObject floaty;
        if(option.ContainsKey("parent")){
            floaty = NGUITools.AddChild((GameObject) option["parent"], statsFloatyImageText);
        }
        else{
            Debug.Log("SpawnStatsFloatyImageText requires a parent");
            return;
        }

        if(option.ContainsKey("text"))
            floaty.transform.Find("Label_StatsChange").GetComponent<UILabel>().text = (string) option["text"];

        if(option.ContainsKey("spriteName"))
            floaty.transform.Find("Sprite_StatsIcon").GetComponent<UISprite>().spriteName = (string) option["spriteName"];

    }

}