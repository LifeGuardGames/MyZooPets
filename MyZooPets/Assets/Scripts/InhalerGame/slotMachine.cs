using UnityEngine;
using System.Collections;

public class slotMachine : MonoBehaviour {
	
	public Texture2D slotMachineTexture, slotTexture, itemsTexture;
	
	private int slot1, slot2, slot3;
	private float slot1y = 0;
	private LTRect slot1Rect = new LTRect (0,0,1f,0.25f);
	private float i = 0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void init()
	{
		slot1 = Random.Range(1,4);
		slot2 = Random.Range(1,4);
		slot2 = Random.Range(1,4);
	}
	
	bool checkWin()
	{
		if(slot1 == slot2 && slot2 == slot3) return true;
		else return false;
	}
	
	void OnGUI()
	{
//		GUI.DrawTexture(new Rect(100,0,1200,800),slotTexture);
//		GUI.DrawTexture(new Rect(100+158,0,1200,800),slotTexture);
//		GUI.DrawTexture(new Rect(100+158*2,0,1200,800),slotTexture);
//
		Debug.Log(slot1Rect.rect.y);
		if(i > 1) i = 0;
		
		i+=0.25f;
		LeanTween.move(slot1Rect,new Vector2(0f,i),0.25f);
		GUI.DrawTextureWithTexCoords(new Rect(425,200,160,200),itemsTexture,slot1Rect.rect);
		GUI.DrawTexture(new Rect(100,0,1200,800),slotMachineTexture);
	}
}
