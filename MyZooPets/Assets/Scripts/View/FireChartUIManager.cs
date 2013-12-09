using UnityEngine;
using System.Collections;

/// <summary>
/// Fire chart user interface manager.
/// UI Manager for the firechart in the wellapad
/// It needs to be able to do 2 things:
/// - Initialize with logic in the beginning
/// - Listen to the logic for updates on fire level
/// </summary>
public class FireChartUIManager : MonoBehaviour {
	
	public GameObject[] fireParentList;		// List of fire box parents
	public GameObject[] fireBarList;		// List of the progress bars to fill
	
	public Color activeColor;
	public Color inactiveColor;
	
	
	public int currentFlameLevel;
	
	void Start(){
		// Check fire levels
		
		// Populate level label?
	}
	
	// Call this on start and on new fire skill get
	private void SetColors(){
		
		//int currentFlameLevel = flameLevel;
		
		for(int i = 0; i < fireParentList.Length; i++){
			
			// Set the box color according to current level
			GameObject box = fireParentList[i].transform.Find("box").gameObject;
			UISprite boxSprite = box.GetComponent<UISprite>();
			boxSprite.color = (i < currentFlameLevel) ? activeColor : inactiveColor;
			
			// Set the box color according to current level
			Transform bar = fireParentList[i].transform.Find("bar");
			// The bar may or may not exist in the group
			if(bar != null){
				UISprite barSprite = bar.gameObject.GetComponent<UISprite>();
				barSprite.color = (i < currentFlameLevel) ? activeColor : inactiveColor;
			}
			
			// Lock and lock label check
			GameObject lockSprite = fireParentList[i].transform.Find("Sprite (padlock)").gameObject;
			GameObject level = fireParentList[i].transform.Find("L_Level").gameObject;
			lockSprite.SetActive((i < currentFlameLevel) ? false : true);
			level.SetActive((i < currentFlameLevel) ? false : true);
		}
	}
	
	/*
	void OnGUI(){
		if(GUI.Button(new Rect(100, 100, 100, 100), "1")){
			currentFlameLevel = 1;
			SetColors();
		}
		
		if(GUI.Button(new Rect(100, 200, 100, 100), "2")){
			currentFlameLevel = 2;
			SetColors();
		}
		
		if(GUI.Button(new Rect(100, 300, 100, 100), "3")){
			currentFlameLevel = 3;
			SetColors();
		}
		
		if(GUI.Button(new Rect(100, 400, 100, 100), "4")){
			currentFlameLevel = 4;
			SetColors();
		}
		
		if(GUI.Button(new Rect(100, 500, 100, 100), "5")){
			currentFlameLevel = 5;
			SetColors();
		}
	}
	*/
	
	// TODO: Make Listener to set color on fire skill up
}
