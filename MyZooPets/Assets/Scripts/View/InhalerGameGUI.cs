using UnityEngine;
using System.Collections;

public class InhalerGameGUI : MonoBehaviour {
	
	public int numberOfNodes;
	public float speed;
	
	public GUISkin defaultSkin;
	public Texture2D circleGray;
	public Texture2D circleRed;
	
	private int currentNode;
	public float currentPercentage;
	public float targetPercentage;
	public float tParam;
	
    private Vector2 pos;
    private Vector2 size = new Vector2(1020, 40);
    public Texture2D emptyTex;
    public Texture2D fullTex;
	
	private float segmentChunkPx;	// Pixels in between chunks
	
	private bool[] boolList;
	
	private bool isUpdating = false;
	
	void Awake(){
	}
	
	void Start(){
		if(numberOfNodes < 2){
			Debug.LogError("Number of nodes cannot be less than 2");	
		}
		pos = new Vector2(Screen.width/2 - size.x/2, 700);
		currentNode = 0;
		
		segmentChunkPx = size.x / numberOfNodes;
		
		boolList = new bool[numberOfNodes];
		for(int i = 0; i < numberOfNodes; i++){
			boolList[i] = false;	
		}
	}
	
	void Update(){
		if(currentPercentage != targetPercentage){
			if (tParam < 1) {
	   			tParam += speed;
				currentPercentage = Mathf.Lerp(currentPercentage, targetPercentage, tParam);
			}
		}
		else{
			if(isUpdating){
				isUpdating = false;
				TickNodeOn(currentNode);
			}
			tParam = 0;
		}
	}
	
	void OnGUI(){
		GUI.skin = defaultSkin;
		
		//draw the background
		GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
		//GUI.Box(new Rect(0,0, size.x, size.y), emptyTex);
		GUI.Box(new Rect(0,0, size.x, size.y), "");
	
	     //draw the filled-in part
		GUI.BeginGroup(new Rect(0,0, size.x * currentPercentage, size.y));
		
		//GUI.Box(new Rect(0,0, size.x, size.y), fullTex);
		GUI.Box(new Rect(0,0, size.x, size.y), "");
		
		GUI.EndGroup();
	   	GUI.EndGroup();
		
		GUI.DrawTexture(new Rect(pos.x - circleGray.width / 2, 670, circleGray.width, circleGray.height), circleRed);
		for(int i = 1; i <= numberOfNodes; i++){
			if(boolList[i - 1]){
				GUI.DrawTexture(new Rect((pos.x - circleGray.width / 2) + (i * segmentChunkPx), 670, circleGray.width, circleGray.height), circleRed);	
			}
			else{
				GUI.DrawTexture(new Rect((pos.x - circleGray.width / 2) + (i * segmentChunkPx), 670, circleGray.width, circleGray.height), circleGray);	
			}
		}
	}

	public void IncreaseBar(){
		if(currentNode < numberOfNodes){
			currentNode++;
			targetPercentage = currentNode / (numberOfNodes * 1.0f);
			isUpdating = true;
		}
	}
	
	private void TickNodeOn(int i){
		i--; // Account for array index offset
		if(i < numberOfNodes && i >= 0){
			boolList[i] = true;
		}
		else{
			Debug.LogError("Illegal node index");
		}
	}
}
