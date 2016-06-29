using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatyUtil {
    private const int NGUI_FLOAT_YPOSITION = 100; //use this to make floaty object move up
    private const float FLOAT_TIME = 3.0f; //duration of the float

    private static GameObject floatyText = null;
    private static GameObject floatyStats = null;
	private static GameObject floatyImageText = null;
	private static GameObject floatyFireCrystal = null;
	
	// NOTE: using a hashtable is actually a pretty bad idea. No type check so hard
	// for other ppl to use
    //---------------------------------------------------- 
    // SpawnFloatyText()
    // This spawns a floaty text that disappears in FLOAT_TIME
    // Option Params:
    //  prefab (string): gameObject that you want to be spawned
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
			floaty = GameObjectUtils.AddChildWithPositionAndScale((GameObject) option["parent"], floatyText);
        }else{
            Debug.LogError("SpawnfloatyText needs a parent");
            return;
        }

        if(option.ContainsKey("position")){
			Vector3 vPos = (Vector3) option["position"];
			floaty.transform.localPosition = vPos;
		}

        if(option.ContainsKey("textSize")){
			float textSize = 5f;
			try{
				textSize = (float) option["textSize"];
			}
			catch(InvalidCastException e){
				Debug.LogError("textSize cast invalid error: " + e.Message);
			}
            
            floaty.transform.Find("Label").GetComponent<Text>().fontSize = (int)textSize;
        }

        if(option.ContainsKey("text")){
            floaty.transform.Find("Label").GetComponent<Text>().text = (string) option["text"];
        }
		
		if(option.ContainsKey("color")){
			floaty.transform.Find("Label").GetComponent<Text>().color = (Color) option["color"];
		}
		
		// If NOT prefab
		if(!option.ContainsKey("prefab")){
            FloatyController floatyController = floaty.GetComponent<FloatyController>();

            if(option.ContainsKey("floatingUpPos"))
                floatyController.floatingUpPos = (Vector3) option["floatingUpPos"];
            else
                floatyController.floatingUpPos = new Vector3(floatyController.transform.position.x, NGUI_FLOAT_YPOSITION, floatyController.transform.position.z);

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
			floaty = GameObjectUtils.AddChild((GameObject) option["parent"], floatyStats);
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
			Text label = floaty.transform.Find("Label_StatsChange" + offsetTracker).GetComponent<Text>();
			label.gameObject.SetActive(true);
			label.text = (string)option["deltaPoints"];
			
			Image sprite = floaty.transform.Find("Sprite_StatsIcon" + offsetTracker).GetComponent<Image>();
			sprite.gameObject.SetActive(true);
			sprite.sprite = SpriteCacheManager.GetSprite((string) option["spritePoints"]);
			
			offsetTracker++;
		}
		if(option.ContainsKey("deltaHealth")){
			Text label = floaty.transform.Find("Label_StatsChange" + offsetTracker).GetComponent<Text>();
			label.gameObject.SetActive(true);
			label.text = (string)option["deltaHealth"];
			
			Image sprite = floaty.transform.Find("Sprite_StatsIcon" + offsetTracker).GetComponent<Image>();
			sprite.gameObject.SetActive(true);
			sprite.sprite = SpriteCacheManager.GetSprite((string) option["spriteHealth"]);
			
			offsetTracker++;
		}
		if(option.ContainsKey("deltaMood")){
			Text label = floaty.transform.Find("Label_StatsChange" + offsetTracker).GetComponent<Text>();
			label.gameObject.SetActive(true);
			label.text = (string)option["deltaMood"];
			
			Image sprite = floaty.transform.Find("Sprite_StatsIcon" + offsetTracker).GetComponent<Image>();
			sprite.gameObject.SetActive(true);
			sprite.sprite = SpriteCacheManager.GetSprite((string) option["spriteHunger"]);
			
			offsetTracker++;
		}
		// Add more stats here in the future if needed
		if(option.ContainsKey("deltaStars")){
			Text label = floaty.transform.Find("Label_StatsChange" + offsetTracker).GetComponent<Text>();
			label.gameObject.SetActive(true);
			label.text = (string)option["deltaStars"];
			
			Image sprite = floaty.transform.Find("Sprite_StatsIcon" + offsetTracker).GetComponent<Image>();
			sprite.gameObject.SetActive(true);
			sprite.sprite = SpriteCacheManager.GetSprite((string) option["spriteStars"]);
			
			offsetTracker++;
		}
		
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
			floaty = GameObjectUtils.AddChild((GameObject) option["parent"], floatyImageText);
        }
        else{
            Debug.Log("floatyImageText requires a parent");
            return;
        }

        if(option.ContainsKey("text"))
            floaty.transform.Find("Label_StatsChange").GetComponent<Text>().text = (string) option["text"];

        if(option.ContainsKey("spriteName"))
            floaty.transform.Find("Sprite_StatsIcon").GetComponent<Image>().sprite = SpriteCacheManager.GetSprite((string) option["spriteName"]);
    }

	public static void SpawnFloatyFireCrystal(Hashtable option){
		if(floatyFireCrystal == null)
			floatyFireCrystal = (GameObject) Resources.Load("FloatyFireCrystal");
		
		GameObject floaty;
		if(option.ContainsKey("parent")){
			floaty = GameObjectUtils.AddChild((GameObject) option["parent"], floatyFireCrystal);
		}
		else{
			Debug.Log("floatyImageText requires a parent");
			return;
		}
		
		// Reset all the children in floaty first
		foreach(Transform child in floaty.transform){
			child.gameObject.SetActive(false);
		}

		if(option.ContainsKey("deltaShards")){
			Text label = floaty.transform.Find("Label_StatsChange1").GetComponent<Text>();
			label.gameObject.SetActive(true);
			label.text = (string)option["deltaShards"];
			
			Image sprite = floaty.transform.Find("Sprite_StatsIcon1").GetComponent<Image>();
			sprite.gameObject.SetActive(true);
		}
	}
}