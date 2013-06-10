using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiaryUIManager : MonoBehaviour {
	
	RoomGUI roomGui;
	bool showGUI = true;
	List<CalendarEntry> calendar;
	public Texture2D diaryTexture1;
	public Texture2D diaryTexture2;
	public Texture2D diaryTexture3;
	public Texture2D diaryTexture4;
	public Texture2D tickBoxEmpty;
	public Texture2D tickBoxChecked;
	public Texture2D tickBoxMissed;
	
	
	public GUIStyle diaryTabStyle;
	public GUIStyle diaryCheckBoxStyle;
	public GUIStyle diaryTextStyle;
	
	private int diaryPage = 1;
	private Vector2 diaryInitPosition = new Vector2(125,-800);
	private Vector2 diaryFinalPosition = new Vector2(650,100);
	private LTRect diaryRect;

	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;
	
	public void Init(){
		calendar = CalendarLogic.GetCalendarEntries();	
	}

	// Use this for initialization
	void Start () {
		roomGui	= GameObject.Find("UIManager/RoomGUI").GetComponent<RoomGUI>();
		diaryRect = new LTRect(diaryInitPosition.x,diaryInitPosition.y, 600, 650);
	}
	
	// Called from ClickManager
	public void DiaryClicked(){
		roomGui.HideGUIs();
		showGUI = false;
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
	
		CalendarLogic.CalendarOpened();
		if(!showGUI)
		{
			LeanTween.move(diaryRect, diaryFinalPosition, 0.5f, optional);
		}
	}
	
	void OnGUI()
	{
		if(!LoadDataLogic.IsDataLoaded) return;

		// Proportional scaling
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){     
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}

		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		GUI.depth = 0;
		if(diaryPage == 1)
		{
			GUI.DrawTexture(diaryRect.rect,diaryTexture3);	
			GUI.DrawTexture(diaryRect.rect,diaryTexture4);
			GUI.DrawTexture(diaryRect.rect,diaryTexture2);
			GUI.DrawTexture(diaryRect.rect,diaryTexture1);
			GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+100,100,70),"Monday",diaryTextStyle);
			GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+170,100,70),"Tuesday",diaryTextStyle);
			GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+240,100,70),"Wednesday",diaryTextStyle);
			GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+310,100,70),"Thursday",diaryTextStyle);
			GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+380,100,70),"Friday",diaryTextStyle);
			GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+450,100,70),"Saturday",diaryTextStyle);
			GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+520,100,70),"Sunday",diaryTextStyle);
			GUI.TextArea(new Rect (diaryRect.rect.x+100,diaryRect.rect.y+577,100,70),""+CalendarLogic.GetComboCount(),diaryTextStyle);
			
			if(GUI.Button(new Rect(diaryRect.rect.x,diaryRect.rect.y,50,50),"X"))
			{
				showGUI = !showGUI;
				LeanTween.move(diaryRect, diaryInitPosition, 0.5f, optional);
				roomGui.ShowGUIs();
			}
			GUILayout.BeginArea(new Rect(diaryRect.rect.x+115,diaryRect.rect.y+100,500,500), "");
			GUILayout.BeginVertical("");
			for(int i = 0;i < 7; i++)
			{
				GUILayout.BeginHorizontal("");
				diaryCheckBoxStyle.normal.background = tickBoxEmpty;
				if(i < calendar.Count)
				{
					if(calendar[i].Morning == DosageRecord.Hit)  
						diaryCheckBoxStyle.normal.background = tickBoxChecked;
					else if (calendar[i].Morning == DosageRecord.Miss)					
						diaryCheckBoxStyle.normal.background = tickBoxMissed;
				}
				GUILayout.Button("",diaryCheckBoxStyle,GUILayout.Height(70),GUILayout.Width(200));
				diaryCheckBoxStyle.normal.background = tickBoxEmpty;
				if(i < calendar.Count)
				{
					if(calendar[i].Afternoon == DosageRecord.Hit)
						diaryCheckBoxStyle.normal.background = tickBoxChecked;
					else if (calendar[i].Afternoon == DosageRecord.Miss)
						diaryCheckBoxStyle.normal.background = tickBoxMissed;
				}
				GUILayout.Button("",diaryCheckBoxStyle,GUILayout.Height(70),GUILayout.Width(200));
				GUILayout.EndHorizontal();	
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		
//			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+70,40,90),""))
//			{
//				Debug.Log("lalala");
//				diaryPage = 1;	
//			}
			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+190,40,105),"",diaryTabStyle))
			{
//				Debug.Log("lalala");
				diaryPage = 2;	
			}
			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+340,40,90),"",diaryTabStyle))
			{
//				Debug.Log("lalala");
				diaryPage = 3;	
			}
			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+480,40,110),"",diaryTabStyle))
			{
//				Debug.Log("lalala");
				diaryPage = 4;	
			}
			
			
//			GUILayout.BeginArea(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+185,1000,1000), "");
//			GUILayout.BeginVertical("");
//			
//			if(GUILayout.Button("",diaryTabStyle,GUILayout.Height(100),GUILayout.Width(40)))
//			{
//				Debug.Log("lalala");
//				diaryPage = 2;
//			}
//			if(GUILayout.Button("",diaryTabStyle,GUILayout.Height(100),GUILayout.Width(40)))
//			{
//				Debug.Log("lalala");
//				diaryPage = 2;
//			}	
//			GUILayout.EndVertical();
//			GUILayout.EndArea();
		}
		else if (diaryPage == 2)
		{
			GUI.DrawTexture(diaryRect.rect,diaryTexture1);	
			GUI.DrawTexture(diaryRect.rect,diaryTexture3);
			GUI.DrawTexture(diaryRect.rect,diaryTexture4);	
			GUI.DrawTexture(diaryRect.rect,diaryTexture2);
			
			if(GUI.Button(new Rect(diaryRect.rect.x,diaryRect.rect.y,50,50),"X"))
			{
				showGUI = !showGUI;
				LeanTween.move(diaryRect, diaryInitPosition, 0.5f, optional);
				roomGui.ShowGUIs();
			}
			
			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+70,40,90),"",diaryTabStyle))
			{
//				Debug.Log("lalala");
				diaryPage = 1;	
			}
//			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+190,40,105),""))
//			{
//				Debug.Log("lalala");
//				diaryPage = 2;	
//			}
			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+340,40,90),"",diaryTabStyle))
			{
//				Debug.Log("lalala");
				diaryPage = 3;	
			}
			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+480,40,110),"",diaryTabStyle))
			{
//				Debug.Log("lalala");
				diaryPage = 4;	
			}	
			
//			GUILayout.BeginArea(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+60,1000,1000), "");
//			GUILayout.BeginVertical("");			
//			if(GUILayout.Button("",diaryTabStyle,GUILayout.Height(110),GUILayout.Width(40)))
//			{
//				Debug.Log("lalala");
//				diaryPage = 1;
//			}
//			GUILayout.EndVertical();
//			GUILayout.EndArea();	
//			if(GUI.Button(new Rect(diaryRect.rect.x,diaryRect.rect.y,50,50),"X"))
//			{
//				showGUI = !showGUI;
//				LeanTween.move(diaryRect, diaryInitPosition, 0.5f, optional);
//				roomGui.ShowGUIs();
//			}
		}
		else if (diaryPage == 3)
		{
			GUI.DrawTexture(diaryRect.rect,diaryTexture1);	
			GUI.DrawTexture(diaryRect.rect,diaryTexture2);
			GUI.DrawTexture(diaryRect.rect,diaryTexture4);	
			GUI.DrawTexture(diaryRect.rect,diaryTexture3);
			
			if(GUI.Button(new Rect(diaryRect.rect.x,diaryRect.rect.y,50,50),"X"))
			{
				showGUI = !showGUI;
				LeanTween.move(diaryRect, diaryInitPosition, 0.5f, optional);
				roomGui.ShowGUIs();
			}
			
			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+70,40,90),"",diaryTabStyle))
			{
//				Debug.Log("lalala");
				diaryPage = 1;	
			}
			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+190,40,105),"",diaryTabStyle))
			{
//				Debug.Log("lalala");
				diaryPage = 2;	
			}
//			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+340,40,90),""))
//			{
//				Debug.Log("lalala");
//				diaryPage = 3;	
//			}
			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+480,40,110),"",diaryTabStyle))
			{
//				Debug.Log("lalala");
				diaryPage = 4;	
			}
		}
		else if (diaryPage == 4)
		{
			GUI.DrawTexture(diaryRect.rect,diaryTexture1);	
			GUI.DrawTexture(diaryRect.rect,diaryTexture2);
			GUI.DrawTexture(diaryRect.rect,diaryTexture3);	
			GUI.DrawTexture(diaryRect.rect,diaryTexture4);
			
			if(GUI.Button(new Rect(diaryRect.rect.x,diaryRect.rect.y,50,50),"X"))
			{
				showGUI = !showGUI;
				LeanTween.move(diaryRect, diaryInitPosition, 0.5f, optional);
				roomGui.ShowGUIs();
			}
			
			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+70,40,90),"",diaryTabStyle))
			{
//				Debug.Log("lalala");
				diaryPage = 1;	
			}
			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+190,40,105),"",diaryTabStyle))
			{
//				Debug.Log("lalala");
				diaryPage = 2;	
			}
			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+340,40,90),"",diaryTabStyle))
			{
//				Debug.Log("lalala");
				diaryPage = 3;	
			}
//			if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+480,40,110),""))
//			{
//				Debug.Log("lalala");
//				diaryPage = 4;	
//			}
		}
	}
}
