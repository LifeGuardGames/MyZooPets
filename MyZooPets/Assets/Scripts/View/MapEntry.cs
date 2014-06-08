using UnityEngine;
using System.Collections;

public class MapEntry : MonoBehaviour {


	public UISprite roomBox;
	public UISprite roomStrip;
	public UISprite roomIcon;

	[SerializeField]
	private string area; //Bedroom or yard. Location in Gates.xml
	[SerializeField]
	private int roomPartition; //Partition in Gates.xml

	public Color bedroomActiveColor;
	public Color bedroomInactiveColor;
	public Color yardActiveColor;
	public Color yardInactiveColor;

	private Color roomIconLockColor;
	private Color roomIconUnlockColor;
	private Color roomBoxAndStripLockColor;
	private Color roomBoxAndStripUnlockColor;

	public string Area{
		get{ return area; }
	}

	public int RoomPartition{
		get{ return roomPartition; }
	}

	void Awake(){
		if(Area == "Bedroom"){
			roomBoxAndStripUnlockColor = bedroomActiveColor;
			roomBoxAndStripLockColor = bedroomInactiveColor;
		}
		else if(Area == "Yard"){
			roomBoxAndStripUnlockColor = yardActiveColor;
			roomBoxAndStripLockColor = yardInactiveColor;
		}

		roomIconUnlockColor = Color.white;
		roomIconLockColor = Color.black;

//		Lock();
	}

	public void Unlock(){
//		Debug.Log(roomBoxAndStripUnlockColor);
		roomBox.color = roomBoxAndStripUnlockColor;
		roomStrip.color = roomBoxAndStripUnlockColor;
		roomIcon.color = roomIconUnlockColor;
	}

	public void Lock(){
		roomBox.color = roomBoxAndStripLockColor;
		roomStrip.color = roomBoxAndStripLockColor;
		roomIcon.color = roomIconLockColor;
	}
}
