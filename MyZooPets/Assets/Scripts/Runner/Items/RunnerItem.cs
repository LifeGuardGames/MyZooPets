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

	// Use this for initialization
	public virtual void Start() { }
	
	// Update is called once per frame
	public virtual void Update() { }
	
	void OnTriggerEnter(Collider inOther) {
		if (inOther.gameObject.tag == "Player") {
			OnPickup();
		
			//Display tutorial if needed	
			if(hasTutorial)
				ItemManager.Instance.DisplayTutorial(ID);

			//Add to minus points depending on trigger
            ScoreManager.Instance.AddPoints(pointValue);


            //Play sound
			if ( !string.IsNullOrEmpty(strSoundPickup) )
				AudioManager.Instance.PlayClip( strSoundPickup );
		}
	}
	
	/// <summary>
	/// Raises the pickup event.
	/// </summary>
	public abstract void OnPickup();
	/// <summary>
	/// Raises the missed event.
	/// </summary>
	public virtual void OnMissed(){
		//Does nothing, and this is sufficient for most items
	}

	/// <summary>
	/// Spawns the floaty text. Replace the tutorial messages
	/// </summary>
	protected void SpawnFloatyText(string toDisplay = ""){
		Hashtable floatyOption = new Hashtable();
//		string hintMessage = Localization.Localize(ID + "_HINT_MESSAGE");

		floatyOption.Add("prefab", "FloatyTextRunner");
		floatyOption.Add("parent", PlayerController.Instance.FloatyLocation);
		floatyOption.Add("floatingUpPos", new Vector3(0, 4, 0));
		floatyOption.Add("floatingTime", 0.5f);
		floatyOption.Add("textSize", 2f);
		floatyOption.Add("text", toDisplay);

		FloatyUtil.SpawnFloatyText(floatyOption);
	}
}
