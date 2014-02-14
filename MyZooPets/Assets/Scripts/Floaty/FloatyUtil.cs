using UnityEngine;
using System;
using System.Collections;

public class FloatyUtil {
    private const int NGUI_FLOAT_YPOSITION = 100; //use this to make floaty object move up
    private const float FLOAT_TIME = 3.0f; //duration of the float

    private static GameObject floatyText = null;
    private static GameObject floatyStats = null;
	private static GameObject floatyImageText = null;
	
    //---------------------------------------------------- 
    // SpawnFloatyText()
    // This spawns a floaty text that disappears in FLOAT_TIME
    // Option Params:
    //  prefab (GameObject): gameObject that you want to be spawned
    //  parent (GameObject): the parent/location that you want the float to spawn under
    //  position (Vector3): the position that you want to spawn the floaty 
    //  textSize (int): size of the floaty
    //  text (string): the text to be displayed
    //---------------------------------------------------- 
    public static void SpawnFloatyText(Hashtable option){
        //if you pass in a prefab you need to make sure the floatingUpPos, floatingUpTime is
        //set in the prefab
		if ( option.ContainsKey("prefab") ) {
			string strPrefab = (string) option["prefab"];
			floatyText = (GameObject) Resources.Load( strPrefab );
		}
        else{
            floatyText = (GameObject) Resources.Load("FloatyText");
        }

        GameObject floaty;

        if(option.ContainsKey("parent")){
            floaty = LgNGUITools.AddChildWithPosition((GameObject) option["parent"], floatyText);
        }else{
            Debug.LogError("SpawnfloatyText needs a parent");
            return;
        }

        if(option.ContainsKey("position")){
			Vector3 vPos = (Vector3) option["position"];
			floaty.transform.localPosition = vPos;
		}

        if(option.ContainsKey("textSize")){
            int textSize = (int) option["textSize"];
            floaty.transform.Find("Label").localScale = new Vector3(textSize, textSize, 1);
        }

        if(option.ContainsKey("text")){
            floaty.transform.Find("Label").GetComponent<UILabel>().text = (string) option["text"];
        }
		
		if(option.ContainsKey("color")){
			floaty.transform.Find("Label").GetComponent<UILabel>().color = (Color) option["color"];
		}
		
		// If NOT prefab
		if(!option.ContainsKey("prefab")){
            FloatyController floatyController = floaty.GetComponent<FloatyController>();

            if(option.ContainsKey("floatingUpPos"))
                floatyController.floatingUpPos = (Vector3) option["floatingUpPos"];
            else
                floatyController.floatingUpPos = new Vector3(0, NGUI_FLOAT_YPOSITION, 0);

            if(option.ContainsKey("floatingTime"))
                floatyController.floatingTime = (float) option["floatingTime"];
            else
    	        floatyController.floatingTime = FLOAT_TIME;
		}
    }
	
	//---------------------------------------------------- 
    // SpawnFloatyStats()
    // Spawns floaty image and text above pet's head to show
    // change in stats
    // Option Params:
    //  parent (GameObject): the parent/location that you want the floaty to spawn under
    //  deltaPoints (string): changes in points
    //  deltaHealth (string): changes in health
    //  deltaMood (string): changes in mood
    //---------------------------------------------------- 
	public static void SpawnFloatyStats(Hashtable option){
		if(floatyStats == null)
			floatyStats = (GameObject) Resources.Load("FloatyStats");
		
		GameObject floaty;
		if(option.ContainsKey("parent")){
            floaty = NGUITools.AddChild((GameObject) option["parent"], floatyStats);
        }
        else{
            Debug.Log("floatyImageText requires a parent");
            return;
        }
		
		// Reset all the children in floaty first
		foreach(Transform child in floaty.transform){
			child.gameObject.SetActive(false);
		}
		
		int offsetTracker = 1; // Each stat that is not 0 will offset from previous one
		
		if(option.ContainsKey("deltaPoints")){
			UILabel label = floaty.transform.Find("Label_StatsChange" + offsetTracker).GetComponent<UILabel>();
			label.gameObject.SetActive(true);
			label.text = (string)option["deltaPoints"];
			label.gameObject.GetComponent<NGUIAlphaTween>().StartAlphaTween();
			
			UISprite sprite = floaty.transform.Find("Sprite_StatsIcon" + offsetTracker).GetComponent<UISprite>();
			sprite.gameObject.SetActive(true);
			sprite.spriteName = (string) option["spritePoints"];
			sprite.gameObject.GetComponent<NGUIAlphaTween>().StartAlphaTween();
			
			
			offsetTracker++;
		}
		if(option.ContainsKey("deltaHealth")){
			UILabel label = floaty.transform.Find("Label_StatsChange" + offsetTracker).GetComponent<UILabel>();
			label.gameObject.SetActive(true);
			label.text = (string)option["deltaHealth"];
			label.gameObject.GetComponent<NGUIAlphaTween>().StartAlphaTween();
			
			UISprite sprite = floaty.transform.Find("Sprite_StatsIcon" + offsetTracker).GetComponent<UISprite>();
			sprite.gameObject.SetActive(true);
			sprite.spriteName = (string) option["spriteHealth"];
			sprite.gameObject.GetComponent<NGUIAlphaTween>().StartAlphaTween();
			
			offsetTracker++;
		}
		if(option.ContainsKey("deltaMood")){
			UILabel label = floaty.transform.Find("Label_StatsChange" + offsetTracker).GetComponent<UILabel>();
			label.gameObject.SetActive(true);
			label.text = (string)option["deltaMood"];
			label.gameObject.GetComponent<NGUIAlphaTween>().StartAlphaTween();
			
			UISprite sprite = floaty.transform.Find("Sprite_StatsIcon" + offsetTracker).GetComponent<UISprite>();
			sprite.gameObject.SetActive(true);
			sprite.spriteName = (string) option["spriteHunger"];
			sprite.gameObject.GetComponent<NGUIAlphaTween>().StartAlphaTween();
			
			offsetTracker++;
		}
		// Add more stats here in the future if needed
		
		if(offsetTracker != 4){	// Check if every stat was modified, else need to hide them
			
		}
	}
	
    //---------------------------------------------------- 
    // SpawnPetFloatyImageText()
    // Spawns floaty image and text above pet's head, generic single entry template
    // Params: parent, text, spriteName
    //---------------------------------------------------- 
    public static void SpawnFloatyImageText(Hashtable option){
        if(floatyImageText == null)
            floatyImageText = (GameObject) Resources.Load("FloatyImageText");

        GameObject floaty;
        if(option.ContainsKey("parent")){
            floaty = NGUITools.AddChild((GameObject) option["parent"], floatyImageText);
        }
        else{
            Debug.Log("floatyImageText requires a parent");
            return;
        }

        if(option.ContainsKey("text"))
            floaty.transform.Find("Label_StatsChange").GetComponent<UILabel>().text = (string) option["text"];

        if(option.ContainsKey("spriteName"))
            floaty.transform.Find("Sprite_StatsIcon").GetComponent<UISprite>().spriteName = (string) option["spriteName"];
    }
}