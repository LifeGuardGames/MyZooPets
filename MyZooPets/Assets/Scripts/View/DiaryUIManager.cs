using UnityEngine;
using System.Collections;

public class DiaryUIManager : MonoBehaviour {

	bool showGUI = true;
	
	public Texture2D diaryTexture1;
	public Texture2D diaryTexture2;
	public GUIStyle diaryTabStyle;
	public GUIStyle diaryCheckBoxStyle;
	
	private int diaryPage = 1;
	private Vector2 diaryInitPosition = new Vector2(125,-800);
	private Vector2 diaryFinalPosition = new Vector2(650,100);
	private LTRect diaryRect;
	// Use this for initialization
	void Start () {
	
		diaryRect = new LTRect(diaryInitPosition.x,diaryInitPosition.y, 600, 650);
	//	diaryRect = new LTRect(diaryFinalPosition.x,diaryFinalPosition.y, 600, 650);
	}
	
	// Update is called once per frame
	void Update () {
		Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(myRay,out hit))
		{
			if(hit.collider.name == "room_table"&&Input.GetMouseButtonUp(1))
			{
				//print("You clicked table!");
				showGUI = !showGUI;
				Hashtable optional = new Hashtable();
				optional.Add("ease", LeanTweenType.easeInOutQuad);
		
				if(!showGUI)
				{
					LeanTween.move(diaryRect, diaryFinalPosition, 0.5f, optional);
				//	LeanTween.move(diaryRect, diaryInitPosition, 0.5f, optional);

				}
//				else
//				{
//					LeanTween.move(diaryRect, diaryFinalPosition, 0.5f, optional);
//				}
			}
		}
	}
	
	void OnGUI()
	{
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		GUI.depth = 0;
		if(diaryPage == 1)
		{
			GUI.DrawTexture(diaryRect.rect,diaryTexture2);
			GUI.DrawTexture(diaryRect.rect,diaryTexture1);
			if(GUI.Button(new Rect(diaryRect.rect.x,diaryRect.rect.y,50,50),"X"))
			{
				showGUI = !showGUI;
				LeanTween.move(diaryRect, diaryInitPosition, 0.5f, optional);
			}
			GUILayout.BeginArea(new Rect(diaryRect.rect.x+115,diaryRect.rect.y+100,500,500), "");
			GUILayout.BeginVertical("");
			for(int i = 0;i < 7; i++)
			{
				GUILayout.BeginHorizontal("");
				GUILayout.Button("",diaryCheckBoxStyle,GUILayout.Height(60),GUILayout.Width(200));
				GUILayout.Button("",diaryCheckBoxStyle,GUILayout.Height(60),GUILayout.Width(200));
				GUILayout.EndHorizontal();	
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		
			
			GUILayout.BeginArea(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+60,1000,1000), "");
			GUILayout.BeginVertical("");			
			if(GUILayout.Button("",diaryTabStyle,GUILayout.Height(100),GUILayout.Width(40)))
			{
				Debug.Log("lalala");
				diaryPage = 2;
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		else if (diaryPage == 2)
		{
			GUI.DrawTexture(diaryRect.rect,diaryTexture1);	
			GUI.DrawTexture(diaryRect.rect,diaryTexture2);
			GUILayout.BeginArea(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+185,1000,1000), "");
			GUILayout.BeginVertical("");			
			if(GUILayout.Button("",diaryTabStyle,GUILayout.Height(110),GUILayout.Width(40)))
			{
				Debug.Log("lalala");
				diaryPage = 1;
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();	
			if(GUI.Button(new Rect(diaryRect.rect.x,diaryRect.rect.y,50,50),"X"))
			{
				showGUI = !showGUI;
				LeanTween.move(diaryRect, diaryInitPosition, 0.5f, optional);
			}
		}

		
	}
}
