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
	
	private int storePage = 1;
	private bool StoreGUIOn = false;
//	private bool StoreGUIOn = true;
	
	//pop window
	private bool windowOn = false;
	private int itemId = 0;
	
	private float sliderValue;
	private ItemLogic itemlogic; 
	
	private Rect windowRect = new Rect(0,0,100,100);
	
	// Use this for initialization
	void Start () {
	
		itemlogic =  GameObject.Find("GameManager").GetComponent<ItemLogic>();
	}
	
	// Update is called once per frame
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
		
		if(StoreGUIOn){
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
			}
			
			GUILayout.BeginArea(new Rect(page1loc.x+50,page1loc.y+50,1000,600));
			for(int i = 0; i< itemlogic.items.Count ;i+=2){
				GUI.Box (new Rect(0,i*100,480,200),"");
				GUI.Box (new Rect(0,i*100,200,180),itemlogic.textures[i]);
				GUI.Label(new Rect(220,i*100,220,100),itemlogic.items[i].Name,itemTitleStyle);
				GUI.Label (new Rect(220,i*100+ 40,220,100),"Health + 10",itemInfoStyle);
				GUI.Label( new Rect(220,i*100+ 60,200,100)," Cost: " + itemlogic.items[i].Cost.ToString(),itemInfoStyle);
				GUI.Button( new Rect(250,i*100+ 100,200,80),"Buy");
				
//				GUI.Box (new Rect (500,i*100,200,180),itemlogic.textures[i+1]);
				GUI.Box (new Rect(500,i*100,480,200),"");
				GUI.Box (new Rect(500,i*100,200,180),itemlogic.textures[i+1]);
				GUI.Label(new Rect(720,i*100,220,100),itemlogic.items[i+1].Name,itemTitleStyle);
				GUI.Label (new Rect(720,i*100+ 40,220,100),"Health + 10",itemInfoStyle);
				GUI.Label( new Rect(720,i*100+ 60,200,100)," Cost: " + itemlogic.items[i+1].Cost.ToString(),itemInfoStyle);
				GUI.Button( new Rect(750,i*100+ 100,200,80),"Buy");
			}
			GUILayout.EndArea();
			
		}
	}
}
