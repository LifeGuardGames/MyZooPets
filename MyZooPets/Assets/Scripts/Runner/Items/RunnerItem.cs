using UnityEngine;

/// <summary>
/// The basis for all player items.
/// It just checks when the collider hits the player, then does OnPickup.
/// </summary>
public abstract class RunnerItem : MonoBehaviour {
	public int pointValue = 0;
	public string strSoundPickup;   // Sound to play on pickup, if any
	public bool hasTutorial;        // Whether this item has a tutorial or not
	protected bool hazard = false;

	void OnTriggerEnter(Collider inOther) {
		//Make sure we are either not a hazard, or that we are a hazard and the player is vulnerable
		if(inOther.gameObject.tag == "Player" && (!hazard || (hazard && !PlayerController.Instance.Invincible))) {
			//Each item handles their own pickup
			OnPickup();

			//If this sound exists, play it if we are not a hazard
			if(!string.IsNullOrEmpty(strSoundPickup)) {
				AudioManager.Instance.PlayClip(strSoundPickup);
			}
		}
	}

	/// <summary>
	/// Raises the pickup event.
	/// </summary>
	public abstract void OnPickup();

	protected void SpawnFloatyText(string toDisplay = "") {
		Vector3 playerPos = PlayerController.Instance.FloatyLocation.transform.position;
		GameObject starFloaty = GameObjectUtils.AddChildGUI(RunnerGameManager.Instance.floatyParent.gameObject, RunnerGameManager.Instance.floatyPrefabText);

		//Because Canvas is a Screen Space - Camera there is no need for WorldToScreenSpace
		starFloaty.GetComponent<UGUIFloaty>().StartFloaty(playerPos, toDisplay, 0.5f, new Vector3(0, 10));
	}

	protected void SpawnFloatyCoin() {
		Vector3 playerPos = PlayerController.Instance.FloatyLocation.transform.position;
		GameObject starFloaty = GameObjectUtils.AddChildGUI(RunnerGameManager.Instance.floatyParent.gameObject, RunnerGameManager.Instance.floatyPrefabCoin);

		//Because Canvas is a Screen Space - Camera there is no need for WorldToScreenSpace
		starFloaty.GetComponent<UGUIFloaty>().StartFloaty(playerPos, "", 0.5f, new Vector3(0, 10));
	}
}
