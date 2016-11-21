using UnityEngine;
using System.Collections;

/// <summary>
/// This is like a piece of fruit from Fruit Ninja
/// it is a positive object that the player wants to destroy.
/// </summary>
public class NinjaTriggerTarget : NinjaTrigger {

	public int points = 1;          // how much is this trigger worth when the player cuts it?
	public Renderer rendererFace;   // renderer for cockroach face
	public bool isOnScreen = false;

	protected override void Start() {
		base.Start();

		// pick a face for this roach
		int totalFacesCount = 5;
		SetFace("triggerFace" + Random.Range(1, totalFacesCount + 1));
	}

	//---------------------------------------------------
	// Sets this roach's face to the incoming string
	// referenced material.
	//---------------------------------------------------	
	private void SetFace(string faceString) {
		Material loadedMaterial = Resources.Load(faceString) as Material;

		if(loadedMaterial != null) {
			rendererFace.material = loadedMaterial;
		}
		else {
			Debug.LogError("Attempting to set cockroach face to non-existant material with face " + faceString);
		}
	}

	protected override void _OnCut() {
		// award points
		NinjaGameManager.Instance._UpdateScore(points);

		if(!NinjaGameManager.Instance.bonusRound) {
			NinjaGameManager.Instance.IncreaseChain();
		}
		else if(NinjaGameManager.Instance.bonusRoundEnemies != 0) {
			NinjaGameManager.Instance.bonusRoundEnemies--;
			NinjaGameManager.Instance.CheckEndBonus();
		}
		// increase the player's combo
		NinjaGameManager.Instance.IncreaseCombo(1);

		Destroy(gameObject);
	}
	
	protected override void _OnMissed() {
		if(!NinjaGameManager.Instance.isTutorialRunning) {
			// the player loses a life
			if(NinjaGameManager.Instance.bonusRound == false) {
				BloodPanelManager.Instance.PlayBlood();
				NinjaGameManager.Instance.UpdateLife(-1);
				NinjaGameManager.Instance.ResetChain();
			}
			else if(NinjaGameManager.Instance.bonusRoundEnemies != 0) {
				NinjaGameManager.Instance.bonusRoundEnemies--;
				NinjaGameManager.Instance.CheckEndBonus();
			}
		}
	}

	void OnTriggerEnter(Collider col) {
		if(isOnScreen && !NinjaGameManager.Instance.bonusRound) {
			GetComponent<Rigidbody>().velocity = new Vector3(-GetComponent<Rigidbody>().velocity.x, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
		}
	}

	void OnBecameVisible() {
		isOnScreen = true;
		GetComponent<Rigidbody>().detectCollisions = true;
	}
}
