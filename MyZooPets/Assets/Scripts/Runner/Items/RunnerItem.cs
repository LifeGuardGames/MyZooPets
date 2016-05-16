/* Sean Duane
 * RunnerItem.cs
 * 8:26:2013   14:24
 * Description:
 * The basis for all player items.
 * It just checks when the collider hits the player, then does OnPickup.
 */

using UnityEngine;
using System.Collections;

public abstract class RunnerItem : MonoBehaviour {
	public string ID = "";
	public int pointValue = 0;
	public string strSoundPickup;  	// sound to play on pickup, if any
	public bool hasTutorial; //Whether this item has a tutorial or not 
	protected bool hazard = false;
	// Use this for initialization
	public virtual void Start() { }
	
	// Update is called once per frame
	public virtual void Update() { }
	
	void OnTriggerEnter(Collider inOther) {
		if (inOther.gameObject.tag == "Player" && (!hazard||(hazard&&!PlayerController.Instance.Invincible))) { //Make sure we are either not a hazard, or that we are a hazard but not invincible
			OnPickup();
		
			//Display tutorial if needed	
			if(hasTutorial) 
				ItemManager.Instance.DisplayTutorial(ID,true);

			//Each item handles adding points

            //Play sound
			if (!string.IsNullOrEmpty(strSoundPickup)) //If this sound exists play it if we are not a hazard, 
				AudioManager.Instance.PlayClip( strSoundPickup ); //or if we are a hazard, we must not be invicibile
		}
	}
	
	/// <summary>
	/// Raises the pickup event.
	/// </summary>
	public abstract void OnPickup();

	/// <summary>
	/// Spawns the floaty text. Replace the tutorial messages
	/// </summary>
	protected void SpawnFloatyText(string toDisplay = "", float floatingTime = -1){
		Hashtable floatyOption = new Hashtable();
//		string hintMessage = Localization.Localize(ID + "_HINT_MESSAGE");

		floatyOption.Add("prefab", "FloatyTextRunner");
		floatyOption.Add("parent", PlayerController.Instance.FloatyLocation);
		floatyOption.Add("textSize", 2f);
		floatyOption.Add("text", toDisplay);
		if (floatingTime!=-1) {
			floatyOption.Add("floatingTime", floatingTime);
		}
		FloatyUtil.SpawnFloatyText(floatyOption);
	}
}
