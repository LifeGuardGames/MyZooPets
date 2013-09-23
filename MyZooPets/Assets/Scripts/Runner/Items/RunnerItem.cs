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
	public string Name = "";
	public int PointValue = 0;
	
	// sound to play on pickup, if any
	public string strSoundPickup;

	// Use this for initialization
	public virtual void Start() { }
	
	// Update is called once per frame
	public virtual void Update() { }

	void OnTriggerEnter(Collider inOther) {
		if (inOther.gameObject.tag == "Player") {
			OnPickup();
			
			if ( !string.IsNullOrEmpty(strSoundPickup) )
				AudioManager.Instance.PlayClip( strSoundPickup );

            ScoreManager scoreManager = RunnerGameManager.GetInstance().ScoreManager;
            scoreManager.AddPoints(PointValue);
		}
	}

	// Define what you want the item to do on pickup here
	public abstract void OnPickup();
}
