using UnityEngine;
using System.Collections;

public class StoreGUI : MonoBehaviour {

	public Texture2D backgroundTexture;
	public Texture2D page1Texture, page2Texture, page3Texture, page4Texture;
	public GUIStyle itemTitleStyle;
	public GUIStyle itemInfoStyle;
	public GUIStyle itemBackgroundStyle;
	public GUIStyle smallBoxStyle;
	public GUIStyle buyIconStyle;
	
	private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;
	private Vector2 backgroundloc = new Vector2(20, 20);
	private Vector2 tabLoc = new Vector2(50, 50);
	private Vector2 tabSize = new Vector2(1125, 727);
	private Vector2 menuItem1Loc;
	private Vector2 menuItem2Loc;
	
	private int storePage = 1;
	private bool StoreGUIOn = false;
//	private bool StoreGUIOn = true;
	
	private float slideValue;
	private ItemLogic itemlogic; 
	private Inventory inventory;
	
	
	void Start () {
	
		itemlogic =  GameObject.Find("GameManager").GetComponent<ItemLogic>();
		inventory =  GameObject.Find("GameManager").GetComponent<Inventory>();
		
	}
	
	void Update () {
	
	}
	
	public void showStore(){
		StoreGUIOn = true;
	}
	
	public void hideStore(){
		StoreGUIOn = false;
	}
	
	void OnGUI(){
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}
		
		//GUI layouts
		if(StoreGUIOn){
			ClickManager.ModeLock();
			ClickManager.ClickLock();
			GUI.DrawTexture(new Rect(backgroundloc.x,backgroundloc.y, backgroundTexture.width, backgroundTexture.height), backgroundTexture);

			if(storePage == 1){
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page1Texture);
			}
			if(storePage == 2){
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page2Texture);
			}
			if(storePage == 3){
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page3Texture);
			}
			if(storePage == 4){
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page4Texture);
			}
			
			if(GUI.Button(new Rect(tabLoc.x + 50, tabLoc.y + 10, 200, 50), "page1")){
				storePage = 1;
			}
			if(GUI.Button(new Rect(tabLoc.x + 300, tabLoc.y + 10, 200, 50), "page2")){
				storePage = 2;
			}
			if(GUI.Button(new Rect(tabLoc.x + 550, tabLoc.y + 10, 200, 50), "page3")){
				storePage = 3;
			}
			if(GUI.Button(new Rect(tabLoc.x + 800, tabLoc.y + 10, 200, 50), "page4")){
				storePage = 4;
			}
			
			if(GUI.Button(new Rect(backgroundloc.x + 20,backgroundloc.y + 20, 50, 50), "X")){
				hideStore();
				ClickManager.ReleaseClickLock();
				ClickManager.ReleaseModeLock();
			}
			
			//Central Item Group
			GUILayout.BeginArea(new Rect(tabLoc.x + 50, tabLoc.y + 100, 1000, 600));
			for(int i = 0; i< itemlogic.items.Count;i+=2){
				if(Input.touchCount>0){
					Touch touch = Input.GetTouch(0);
					if(touch.position.x > tabLoc.x && touch.position.x < tabLoc.x +1000&& touch.position.y > tabLoc.y && touch.position.y < tabLoc.y+600){
						if(Mathf.Abs(touch.deltaPosition.y) > 5){
							slideValue -= touch.deltaPosition.y *2;
						}	
					}
				}
				
				//This code is buggy ....some one please take a look ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				if(slideValue <= 0 /*&& slideValue >= -600 + 100* itemlogic.items.Count*/){
					
					menuItem1Loc = new Vector2(0,i*100+slideValue);
					menuItem2Loc = new Vector2(500,i*100+slideValue);
				}
				else if( slideValue > 0){
					menuItem1Loc = new Vector2(0,i*100);
					menuItem2Loc = new Vector2(500,i*100);
					slideValue = 0;
				}
//				else{
//					menuItem1Loc = new Vector2(0,-600+100*i);
//					menuItem2Loc = new Vector2(500,-600+i*100);
//					
//				}

				//Each line contains 2 items
				GUI.Box (new Rect(menuItem1Loc.x,menuItem1Loc.y,480,200),"");
				GUI.Box (new Rect(menuItem1Loc.x,menuItem1Loc.y,200,180),itemlogic.textures[i]);
				GUI.Label(new Rect(menuItem1Loc.x + 220,menuItem1Loc.y ,220,100),itemlogic.items[i].Name,itemTitleStyle);
				GUI.Label (new Rect(menuItem1Loc.x + 220,menuItem1Loc.y + 40,220,100),"Health + 10",itemInfoStyle);
				GUI.Label( new Rect(menuItem1Loc.x + 220,menuItem1Loc.y + 60,200,100)," Cost: " + itemlogic.items[i].Cost.ToString(),itemInfoStyle);
				if(GUI.Button( new Rect(menuItem1Loc.x + 250,menuItem1Loc.y + 100,200,80),"Buy")){
					inventory.addItem(i,1);
				}
				
				GUI.Box (new Rect( menuItem2Loc.x,menuItem2Loc.y,480,200),"");
				GUI.Box (new Rect(menuItem2Loc.x,menuItem2Loc.y ,200,180),itemlogic.textures[i+1]);
				GUI.Label(new Rect(menuItem2Loc.x + 220,menuItem2Loc.y,220,100),itemlogic.items[i+1].Name,itemTitleStyle);
				GUI.Label (new Rect(menuItem2Loc.x + 220,menuItem2Loc.y + 40,220,100),"Health + 10",itemInfoStyle);
				GUI.Label( new Rect(menuItem2Loc.x + 220,menuItem2Loc.y + 60,200,100)," Cost: " + itemlogic.items[i+1].Cost.ToString(),itemInfoStyle);
				if(GUI.Button( new Rect(menuItem2Loc.x + 250,menuItem2Loc.y + 100,200,80),"Buy")){
					inventory.addItem(i+1,1);
				}
			}
			GUILayout.EndArea();
			
			
		}
	}
}
