using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreGUI : MonoBehaviour {
	public GUISkin skin;
	public Texture2D backgroundTexture;
	public Texture2D page1Texture, page2Texture, page3Texture, page4Texture;
	public Texture2D backButton;
	public GUIStyle blankButtonStyle;
	public GUIStyle itemTitleStyle;
	public GUIStyle itemInfoStyle;
	public GUIStyle itemBackgroundStyle;
	public GUIStyle smallBoxStyle;
	public GUIStyle buyIconStyle;

	private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;
	private Vector2 bgLoc = new Vector2(100, 20);
	private Vector2 tabLoc = new Vector2(130, 50);
	private Vector2 tabSize = new Vector2(1125, 727);
	private Vector2 backButtonLoc = new Vector2(40, 440);
	private Vector2 menuItem1Loc;
	private Vector2 menuItem2Loc;

	private int storePage = 1;
	private bool StoreGUIOn = false;
//	private bool StoreGUIOn = true;

	private float slideValue= 0;
	private ItemLogic itemlogic;
	private Inventory inventory;
	private	List<int> categoryList = new List<int>();


	void Start(){
		itemlogic =  GameObject.Find("GameManager").GetComponent<ItemLogic>();
		inventory =  GameObject.Find("GameManager").GetComponent<Inventory>();
	}

	void Update(){
		if(Input.touchCount > 0) slideValue -= Input.GetTouch(0).deltaPosition.y;
		if(slideValue > 0) slideValue = 0;
		if(slideValue < -(categoryList.Count/2*200 - 600)) slideValue = - (categoryList.Count/2*200 -600);
		if(categoryList.Count <= 6){ slideValue = 0;}
		
	}

	public void showStore(){
		StoreGUIOn = true;
	}

	public void hideStore(){
		StoreGUIOn = false;
	}

	void OnGUI(){
		GUI.skin = skin;
		
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}

		//GUI layouts
		if(StoreGUIOn){
			ClickManager.ModeLock();
			ClickManager.ClickLock();
			GUI.DrawTexture(new Rect(bgLoc.x,bgLoc.y, backgroundTexture.width, backgroundTexture.height), backgroundTexture);

			if(storePage == 1){
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page2Texture);
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page3Texture);
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page4Texture);
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page1Texture);
				displayItems(ItemCategory.Foods);
			}
			if(storePage == 2){
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page1Texture);
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page3Texture);
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page4Texture);
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page2Texture);
				displayItems(ItemCategory.Items);
			}
			if(storePage == 3){
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page1Texture);
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page2Texture);
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page4Texture);
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page3Texture);
				displayItems(ItemCategory.Inhalers);
			}
			if(storePage == 4){
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page1Texture);
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page2Texture);
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page3Texture);
				GUI.DrawTexture(new Rect(tabLoc.x, tabLoc.y, tabSize.x, tabSize.y), page4Texture);
				displayItems(ItemCategory.Decorations);
			}

			if(GUI.Button(new Rect(tabLoc.x + 50, tabLoc.y + 15, 200, 50), "Food", blankButtonStyle)){
				storePage = 1;
			}
			if(GUI.Button(new Rect(tabLoc.x + 300, tabLoc.y + 15, 200, 50), "Items", blankButtonStyle)){
				storePage = 2;
			}
			if(GUI.Button(new Rect(tabLoc.x + 550, tabLoc.y + 15, 200, 50), "Inhalers", blankButtonStyle)){
				storePage = 3;
			}
			if(GUI.Button(new Rect(tabLoc.x + 800, tabLoc.y + 15, 200, 50), "Decoration", blankButtonStyle)){
				storePage = 4;
			}

			if(GUI.Button(new Rect(backButtonLoc.x,backButtonLoc.y, backButton.width, backButton.height), backButton, blankButtonStyle)){
				hideStore();
				ClickManager.ReleaseClickLock();
				ClickManager.ReleaseModeLock();
			}
			
			
			
		}
	}
	
	private void displayItems(ItemCategory c){
		if(c == ItemCategory.Foods) categoryList = itemlogic.foodlist;
		if(c == ItemCategory.Decorations) categoryList = itemlogic.decolist;
		if(c == ItemCategory.Inhalers) categoryList = itemlogic.inhalerlist; 
		if(c == ItemCategory.Items) categoryList = itemlogic.itemlist;
		
		//Window
		GUI.BeginGroup(new Rect(tabLoc.x+50,tabLoc.y+100,1000,600));
		//Movable group that display
		GUI.BeginGroup(new Rect(0,slideValue,1000,100*itemlogic.items.Count));
			
		for(int i = 0;i< categoryList.Count ;i+=2){
				
			menuItem1Loc = new Vector2(0,i*100);
			menuItem2Loc = new Vector2(500,i*100);
				
				
//			Each line contains 2 items
			GUI.Box (new Rect(menuItem1Loc.x + 20, menuItem1Loc.y + 20, 440, 160), "");
			GUI.Box (new Rect(menuItem1Loc.x + 40, menuItem1Loc.y + 40, 120, 120), "");	// TODO-s Merge this into one draw call
			GUI.DrawTexture(new Rect(menuItem1Loc.x + 40, menuItem1Loc.y + 40, 120, 120), itemlogic.textures[categoryList[i]]);
			GUI.Label(new Rect(menuItem1Loc.x + 220, menuItem1Loc.y + 10 ,220, 100), itemlogic.items[categoryList[i]].Name,itemTitleStyle);
			GUI.Label(new Rect(menuItem1Loc.x + 220, menuItem1Loc.y + 55, 220, 100), "Health + 10",itemInfoStyle);
			GUI.Label(new Rect(menuItem1Loc.x + 220, menuItem1Loc.y + 75, 200, 100), " Cost: " + itemlogic.items[categoryList[i]].Cost.ToString(), itemInfoStyle);
			if(GUI.Button(new Rect(menuItem1Loc.x + 250, menuItem1Loc.y + 120, 180, 60), "Buy")){
				inventory.addItem(categoryList[i], 1);
			}
			
			GUI.Box (new Rect(menuItem2Loc.x + 20, menuItem2Loc.y + 20, 440, 160), "");
			GUI.Box (new Rect(menuItem2Loc.x + 40, menuItem2Loc.y + 40, 120 ,120), "");
			GUI.DrawTexture(new Rect(menuItem2Loc.x + 40, menuItem2Loc.y + 40, 120, 120), itemlogic.textures[categoryList[i+1]]);
			GUI.Label(new Rect(menuItem2Loc.x + 220, menuItem2Loc.y + 10, 220, 100), itemlogic.items[categoryList[i+1]].Name, itemTitleStyle);
			GUI.Label (new Rect(menuItem2Loc.x + 220, menuItem2Loc.y + 55, 220, 100), "Health + 10", itemInfoStyle);
			GUI.Label( new Rect(menuItem2Loc.x + 220, menuItem2Loc.y + 75, 200, 100), " Cost: " + itemlogic.items[categoryList[i+1]].Cost.ToString(), itemInfoStyle);
			if(GUI.Button( new Rect(menuItem2Loc.x + 250, menuItem2Loc.y + 120, 180, 60), "Buy")){
				inventory.addItem(categoryList[i+1], 1);
			}
		}				
		GUI.EndGroup();
		GUI.EndGroup();
	}
}
