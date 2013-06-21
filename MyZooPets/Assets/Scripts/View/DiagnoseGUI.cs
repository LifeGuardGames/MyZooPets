using UnityEngine;
using System.Collections;

public class DiagnoseGUI : MonoBehaviour {
	
	public GameObject cameraMoveObject;
	private CameraMove cameraMove;
	
	public GameObject roomGuiObject;
	private RoomGUI roomGui;
	
	public GameObject diaryGuiObject;
	private DiaryGUI diaryGui;
	
	public GUISkin defaultSkin;
	private bool isActive = false;
	
	public Texture2D txPanel;
	public Texture2D txCheck;
	public Texture2D txHappy;
	public Texture2D txNeutral;
	public Texture2D txSad;
	
	private LTRect diagnoseRect;
	private Vector2 diagnoseInitPosition;
	private Vector2 diagnoseFinalPosition;
	
	public GUIStyle diagnoseStyle;
	
	void Start(){
		cameraMove = cameraMoveObject.GetComponent<CameraMove>();
		roomGui = roomGuiObject.GetComponent<RoomGUI>();
		diaryGui = diaryGuiObject.GetComponent<DiaryGUI>();
		
		diagnoseInitPosition = new Vector2(1300, 100);
		diagnoseFinalPosition = new Vector2(Screen.width/2, 100);
		diagnoseRect = new LTRect(diagnoseInitPosition.x, diagnoseInitPosition.y, 611, 611);
	}
	
	void Update(){
	
	}
	
	void OnGUI(){
		if(isActive){
			//GUI.skin = defaultSkin;
			
			GUI.DrawTexture(diagnoseRect.rect, txPanel);
			
			GUI.Label(new Rect(diagnoseRect.rect.x + 20, diagnoseRect.rect.y + 20, diagnoseRect.rect.width - 40, diagnoseRect.rect.height - 40), "How severe is its asthma?", diagnoseStyle);
			
			if(GUI.Button(new Rect(diagnoseRect.rect.x + 10, diagnoseRect.rect.y + 200, 190, 190), txHappy)){
				
			}
			if(GUI.Button(new Rect(diagnoseRect.rect.x + 210, diagnoseRect.rect.y + 200, 190, 190), txNeutral)){
				
			}
			if(GUI.Button(new Rect(diagnoseRect.rect.x + 410, diagnoseRect.rect.y + 200, 190, 190), txSad)){
				
			}
			if(GUI.Button(new Rect(diagnoseRect.rect.x + 390, diagnoseRect.rect.y + 437, 200, 153), txCheck)){
				ShowDiary();
			}
			
			if(GUI.Button(new Rect(10, 10, 100, 100), "X")){
				cameraMove.PetSideZoomToggle();
				roomGui.ShowGUIs();	
				isActive = false;
				
				Hashtable optional = new Hashtable();
				optional.Add("ease", LeanTweenType.easeInOutQuad);
				LeanTween.move(diagnoseRect, diagnoseInitPosition, 0.5f, optional);
			}
		}
	}
	
	private void ShowDiary(){
		diaryGui.ShowDiary(4, true);
	}
	
	public void DiagnoseClicked(){
		if(!ClickManager.isClickLocked && !ClickManager.isModeLocked){
			isActive = true;
			ClickManager.ClickLock();
			ClickManager.ModeLock();
			cameraMove.PetSideZoomToggle();
			roomGui.HideGUIs(true, true, true, true);
			
			Hashtable optional = new Hashtable();
			optional.Add("ease", LeanTweenType.easeInOutQuad);
			LeanTween.move(diagnoseRect, diagnoseFinalPosition, 0.5f, optional);
		}
	}
}
