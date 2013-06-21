using UnityEngine;
using System.Collections;

public class InhalerGameGUI : MonoBehaviour {

	// Native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

	public GUIStyle inhalerStyle;
	public Texture2D circleGray;
	public Texture2D circleRed;
	public float speed;

	private int currentNode;
	private float currentPercentage;
	private float targetPercentage;
	private float tParam;

	private int numberOfNodes;
    private Vector2 pos;
    private Vector2 size = new Vector2(1020, 40);

	private float segmentChunkPx;	// Pixels in between chunks
	private bool[] boolList;		// List to keep track of current state rendering
	private bool isUpdating = false;

	private bool showButtons = true;

    public GameObject slotMachine;
    NotificationUIManager notificationUIManager;
    public InhalerGameManager inhalerGameManager;

	void Start(){
        notificationUIManager = GameObject.Find("NotificationUIManager").GetComponent<NotificationUIManager>();

		RestartProgressBar();
	}

	public void RestartProgressBar(){
		currentNode = InhalerLogic.CurrentStep - 1; // Starting out with step 0 here

		SetNumOfNodes();

		if(numberOfNodes < 2){
			Debug.LogError("Number of nodes cannot be less than 2");
		}
		pos = new Vector2(NATIVE_WIDTH/2 - size.x/2, 700);
		currentNode = 0;

		segmentChunkPx = size.x / numberOfNodes;

		boolList = new bool[numberOfNodes];
		for(int i = 0; i < numberOfNodes; i++){
			boolList[i] = false;
		}
	}

	public void HideButtons(){
		showButtons = false;
	}
	public void ShowButtons(){
		showButtons = true;
	}

	public void DisplayMessage(){
		notificationUIManager.PopupTexture("great", 0, 0, 0, 0, 0);
	}

	void SetNumOfNodes(){
		if (InhalerLogic.CurrentInhalerType == InhalerType.Advair){
			numberOfNodes = 5;
		}
		if (InhalerLogic.CurrentInhalerType == InhalerType.Rescue){
			numberOfNodes = 6;
		}
	}

	void Update(){
		if(currentNode != InhalerLogic.CurrentStep - 1){
			UpdateBar();
		}
		if(currentPercentage != targetPercentage){
			if (tParam < 1){
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
		// Proportional scaling
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}

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
			GUI.Label(new Rect((pos.x - circleGray.width / 2) + (i * segmentChunkPx) + 30, 682, circleGray.width, circleGray.height), i.ToString(), inhalerStyle);
		}

		if (showButtons){
			// Show "Play Again" button after showing (and spinning) slot machine
	        if (inhalerGameManager.gameEnded && inhalerGameManager.showPlayAgain){
	            if(GUI.Button(new Rect(NATIVE_WIDTH - 240, 10, 100, 100), "Play Again")){
	                inhalerGameManager.ResetInhalerGame();
	                RestartProgressBar();
	            }
	        }
	        if (!InhalerLogic.CanPlayGame) {
	            int x = 200;
	            int y = 150;
	            GUI.Label(new Rect(NATIVE_WIDTH / 2 - x/2, NATIVE_HEIGHT / 2 - y/2, x, y), "Come play again tomorrow!");
	            if(GUI.Button(new Rect(NATIVE_WIDTH / 2 - 50, NATIVE_HEIGHT / 2 - 50, 100, 100), "Quit Game")){
	                QuitInhalerGame();
	            }
	        }
	        else {
	        	// draw Quit Button in upper right corner
	            if(GUI.Button(new Rect(NATIVE_WIDTH - 120, 10, 100, 100), "Quit Game")){
	            	QuitInhalerGame();
	            }
	        }
		}
	}

	void QuitInhalerGame(){
        RestartProgressBar();
        Application.LoadLevel("BedRoom");
    }

	public void UpdateBar(){
		if(currentNode < numberOfNodes){
			currentNode = InhalerLogic.CurrentStep - 1;
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
			Debug.LogError("Illegal node index: " + i);
		}
	}
}
