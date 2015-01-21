using UnityEngine;
using System.Collections;

public class ShooterGameManager : MinigameManager<ShooterGameManager>{

	// the number of times the user has missed the optimal use window
	//public int NumMissed=0;
	public Camera nguiCamera;
	public GameObject EController;
	// our score
	public int Score=0;
	public GameObject ScoreLabel;
	void Awake(){
		quitGameScene = SceneUtils.BEDROOM;
	}

	protected override void _Start(){
	}

	protected override void _OnDestroy(){
		Application.targetFrameRate = 30;
	}

	protected override string GetMinigameKey(){
		return "ShooterGame";
	}

	protected override bool IsTutorialOn(){
		return false;	//TODO Change
	}
	
	protected override bool HasCutscene(){
		return false;	//TODO Change
	}

	protected override void _NewGame(){
		ScoreLabel.GetComponent<UILabel>().text = Score.ToString();
		EController.GetComponent<EnemyController>().reset();
	}
		
	public override int GetReward(MinigameRewardTypes eType){
		return GetStandardReward(eType);
	}

	void OnTap(TapGesture e){
		if(!IsTouchingNGUI(e.Position)){

#if !UNITY_EDITOR
		
			Vector3 touchPos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 1);
			PlayerShooterController.Instance.shoot(TouchPos);
#endif
#if UNITY_EDITOR
		Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
			PlayerShooterController.Instance.shoot(mousePos);
#endif
		}
	}
	public void AddScore(int amount)
	{
		Score+=amount;
		ScoreLabel.GetComponent<UILabel>().text = Score.ToString();
	}
	
	// Update is called once per frame
	protected override void _Update(){
	
	}
	//True: if finger touches NGUI 
	/// <summary>
	/// Determines whether if the touch is touching NGUI element
	/// </summary>
	/// <returns><c>true</c> if this instance is touching NGUI; otherwise, <c>false</c>.</returns>
	/// <param name="screenPos">Screen position.</param>
	private bool IsTouchingNGUI(Vector2 screenPos){
		Ray ray = nguiCamera.ScreenPointToRay(screenPos);
		RaycastHit hit;
		int layerMask = 1 << 10; 
		bool isOnNGUILayer = false;
		
		// Raycast
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
			isOnNGUILayer = true;
		}
		return isOnNGUILayer;
	}
}
