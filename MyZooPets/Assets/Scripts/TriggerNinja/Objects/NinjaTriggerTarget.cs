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
		NinjaManager.Instance._UpdateScore(points);

		if(!NinjaManager.Instance.bonusRound) {
			NinjaManager.Instance.IncreaseChain();
		}
		else if(NinjaManager.Instance.bonusRoundEnemies != 0) {
			NinjaManager.Instance.bonusRoundEnemies--;
			NinjaManager.Instance.CheckEndBonus();
		}
		// increase the player's combo
		NinjaManager.Instance.IncreaseCombo(1);

		Destroy(gameObject);
	}
	
	protected override void _OnMissed() {
		if(!NinjaManager.Instance.isTutorialRunning) {
			// the player loses a life
			if(NinjaManager.Instance.bonusRound == false) {
				NinjaManager.Instance.UpdateLife(-1);
				NinjaManager.Instance.ResetChain();
			}
			else if(NinjaManager.Instance.bonusRoundEnemies != 0) {
				NinjaManager.Instance.bonusRoundEnemies--;
				NinjaManager.Instance.CheckEndBonus();
			}
		}
	}

	void OnTriggerEnter(Collider col) {
		if(isOnScreen && !NinjaManager.Instance.bonusRound) {
			GetComponent<Rigidbody>().velocity = new Vector3(-GetComponent<Rigidbody>().velocity.x, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
		}
	}

	void OnBecameVisible() {
		isOnScreen = true;
		GetComponent<Rigidbody>().detectCollisions = true;
	}
}
