using UnityEngine;
using System.Collections;

public class DiaryUIManager : MonoBehaviour {

	bool showGUI = false;
	
	public Texture2D DiaryTexture;
	
	private Vector2 diaryInitPosition = new Vector2(125,-700);
	private Vector2 diaryFinalPosition = new Vector2(125,100);
	private LTRect diaryRect;
	// Use this for initialization
	void Start () {
	
		diaryRect = new LTRect(diaryInitPosition.x,diaryInitPosition.y, 1000, 500);
	//	diaryRect = new LTRect(diaryFinalPosition.x,diaryFinalPosition.y, 1100, 650);
	}
	
	// Update is called once per frame
	void Update () {
		Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(myRay,out hit))
		{
			if(hit.collider.name == "room_table"&&Input.GetMouseButtonUp(1))
			{
				print("You clicked table!");
				showGUI = !showGUI;
				Hashtable optional = new Hashtable();
				optional.Add("ease", LeanTweenType.easeInOutQuad);
		
				if(!showGUI)
				{
					LeanTween.move(diaryRect, diaryInitPosition, 0.5f, optional);

				}
				else
				{
					LeanTween.move(diaryRect, diaryFinalPosition, 0.5f, optional);
				}
			}
		}
	}
	
	void OnGUI()
	{
		GUI.depth = 0;
		GUI.DrawTexture(diaryRect.rect,DiaryTexture);
		
		GUILayout.BeginArea(new Rect(diaryRect.rect.x+115,diaryRect.rect.y+130,400,500), "");
		GUILayout.BeginVertical("");
		for(int i = 0;i < 7; i++)
		{
			GUILayout.BeginHorizontal("");
			GUILayout.Button("testing",GUILayout.Height(60),GUILayout.Width(175));
			GUILayout.Button("testing",GUILayout.Height(60),GUILayout.Width(175));
			GUILayout.EndHorizontal();	
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();

		
	}
}
