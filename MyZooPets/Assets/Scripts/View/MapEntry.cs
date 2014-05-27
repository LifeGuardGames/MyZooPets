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
		roomBoxAndStripUnlockColor = new Color32(46, 71, 161, 255);
		roomBoxAndStripLockColor = Color.gray;
		roomIconUnlockColor = Color.white;
		roomIconLockColor = Color.black;

//		Lock();
	}

	public void Unlock(){
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
