using UnityEngine;
using System.Collections;

public class DiaryGUI : MonoBehaviour {

	bool showGUI = false;
	
	public Texture2D DiaryTexture;
	
	private Vector2 diaryInitPosition = new Vector2(150,-700);
	private Vector2 diaryFinalPosition = new Vector2(150,100);
	private LTRect diaryRect;
	// Use this for initialization
	void Start () {
	
		diaryRect = new LTRect(diaryInitPosition.x,diaryInitPosition.y, 600, 500);
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
		GUI.DrawTexture(diaryRect.rect,DiaryTexture);
		//GUI.Box(diaryRect.rect,"Diary");
		

		
	}
}
