using UnityEngine;
using System.Collections;

public class StoreGUI : MonoBehaviour {

	public Texture2D backgroundTexture;
	public Texture2D page1Texture,page2Texture,page3Texture,page4Texture;
	public GUIStyle itemTitleStyle;
	public GUIStyle itemInfoStyle;
	public GUIStyle itemBackgroundStyle;
	public GUIStyle smallBoxStyle;
	public GUIStyle buyIconStyle;

	private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;
	private Vector2 backgroundloc = new Vector2(40,20);
	private Vector2 page1loc = new Vector2(100,100);
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
			GUI.DrawTexture(new Rect(backgroundloc.x,backgroundloc.y,1200,760),backgroundTexture);
			if(GUI.Button(new Rect (page1loc.x+50,page1loc.y-50,200,50),"page1")){
				storePage = 1;
			}
			if(GUI.Button(new Rect (page1loc.x+300,page1loc.y-50,200,50),"page2")){
				storePage = 2;
			}
			if(GUI.Button(new Rect (page1loc.x+550,page1loc.y-50,200,50),"page3")){
				storePage = 3;
			}
			if(GUI.Button(new Rect (page1loc.x+800,page1loc.y-50,200,50),"page4")){
				storePage = 4;
			}

			if(storePage == 1){
				GUI.DrawTexture(new Rect(page1loc.x,page1loc.y,1080,650),page1Texture);
			}
			if(storePage == 2){
				GUI.DrawTexture(new Rect(page1loc.x,page1loc.y,1080,650),page2Texture);
			}
			if(storePage == 3){
				GUI.DrawTexture(new Rect(page1loc.x,page1loc.y,1080,650),page3Texture);
			}
			if(storePage == 4){
				GUI.DrawTexture(new Rect(page1loc.x,page1loc.y,1080,650),page4Texture);
			}

			if(GUI.Button(new Rect(backgroundloc.x+50,backgroundloc.y+50,50,50),"X")){
				hideStore();
				ClickManager.ReleaseClickLock();
				ClickManager.ReleaseModeLock();
			}

			//Central Item Group
			GUILayout.BeginArea(new Rect(page1loc.x+50,page1loc.y+50,1000,600));
			for(int i = 0; i< itemlogic.items.Count ;i+=2){
				if(Input.touchCount>0){
					Touch touch = Input.GetTouch(0);
					if(touch.position.x > page1loc.x && touch.position.x < page1loc.x +1000&& touch.position.y > page1loc.y && touch.position.y < page1loc.y+600){
						if(Mathf.Abs(touch.deltaPosition.y) > 5){
							slideValue -= touch.deltaPosition.y *2;
						}
					}
				}

				int numRows = (itemlogic.items.Count + 1)/ 2;
				int itemHeight = 200;
				int minVal = -(numRows - 3) * itemHeight;
				//This code is buggy ....some one please take a look ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				// if(slideValue <= 0 /*&& slideValue >= -600 + 100* itemlogic.items.Count*/){
					print(slideValue);
				if(slideValue <= minVal /*&& slideValue >= -600 + 100* itemlogic.items.Count*/){

					// menuItem1Loc = new Vector2(0,i*100+slideValue);
					// menuItem2Loc = new Vector2(500,i*100+slideValue);
					menuItem1Loc = new Vector2(0,minVal + i*100);
					menuItem2Loc = new Vector2(500,minVal + i*100);
					slideValue = minVal;
				}
				else if( slideValue >= 0){
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
