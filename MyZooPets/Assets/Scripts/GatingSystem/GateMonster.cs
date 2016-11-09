using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gate monster.
/// NOTE: This is a gate that is a monster.
/// </summary>
public class GateMonster : Gate{
	public float tweenTime; // time it takes the monster to tween to its new position after taking damage
	public GameObject[] smokeMonsterHeads;	// Local locations of the heads, base head needs to be first!
	public bool isBoss = false;
	private int currentHealth;
	private GameObject baseHeadToMove;	// Keep track of the first head to check
	private GameObject nextHeadToMove;

	public override void Start(){
		base.Start();
		SetupHeads();
	}

	public void SetupHeads(){
		if(SceneManager.GetActiveScene().name == SceneUtils.YARD){
			isBoss = true;
		}
		// New way to show monster health - having multiple heads
		currentHealth = DataManager.Instance.GameData.GatingProgress.GatingProgress[gateID];
		if(currentHealth <= smokeMonsterHeads.Length){
			for(int i = currentHealth; i <= smokeMonsterHeads.Length - 1; i++){
				smokeMonsterHeads[i].gameObject.SetActive(false);
			}
			// First head to move is always the last one on the list unless there are no more heads
			if(currentHealth != 0){
				nextHeadToMove = smokeMonsterHeads[currentHealth - 1];
			}
			
			// Assign the base head
			baseHeadToMove = smokeMonsterHeads[0];
		}
		else{
			Debug.LogError("Incorrect length size for smoke monster list");
		}
	}
	

	protected override bool GateDamagedDestroy(){
		if(isBoss){
			// If we are fighting the boss at the yard, load the micromix scene and tell gate not to do anything
			LoadLevelManager.Instance.StartLoadTransition(SceneUtils.MICROMIX, additionalTextKey: "MICRO_TITLE");
            return false;
		}
		else{
			// Drop some coins when the gate monster is attacked
			Debug.Log("Fix coin pos to default here");
			StatsManager.Instance.ChangeStats(coinsDelta: 35, is3DObject: true);

			Debug.Log("Damaging head " + (DataManager.Instance.GameData.GatingProgress.GatingProgress[gateID] - 1));
			nextHeadToMove = smokeMonsterHeads[DataManager.Instance.GameData.GatingProgress.GatingProgress[gateID] - 1];
			// Move one of the heads out, ONLY applies to everything thats not the first head
			if(baseHeadToMove != nextHeadToMove) {
				MoveSubHead();
			}
			return true;
		}
	}	

	protected override void GateDestroyed(){
		// Move all the heads that is NOT the base head
		MoveBaseHead();
	}

	private void MoveSubHead(){
		//Debug.Log("Moving sub head " + nextHeadToMove.name + " " + nextHeadToMove.transform.parent.name);
		Vector3 newLoc = nextHeadToMove.transform.localPosition;
		newLoc.x += 80;

		LeanTween.cancel(nextHeadToMove.transform.parent.gameObject);
		LeanTween.moveLocal(nextHeadToMove.transform.parent.gameObject, newLoc, tweenTime)
			.setDestroyOnComplete(true).setEase(LeanTweenType.easeInQuad);

		nextHeadToMove.GetComponent<Animator>().Play("smokeMonsterDie");
	}

	private void MoveBaseHead(){
		// for monsters, just move them fast and far away
		float monsterMoveDistance = CameraManager.Instance.PanScript.partitionOffset;
		
		Vector3 targetPos = gameObject.transform.position;
		targetPos.x += monsterMoveDistance;

		// Cancel the move tweens previous to this
		LeanTween.cancel(gameObject);
		LeanTween.moveLocal(gameObject, targetPos, tweenTime)
			.setDestroyOnComplete(true).setEase(LeanTweenType.easeInQuad);

		baseHeadToMove.GetComponent<Animator>().Play("smokeMonsterDie");
	}
}
