using UnityEngine;
using System.Collections;

/// <summary>
/// Gate monster.
/// NOTE: This is a gate that is a monster.
/// </summary>
public class GateMonster : Gate{
	public float tweenTime; // time it takes the monster to tween to its new position after taking damage
	public GameObject[] smokeMonsterHeads;	// Local locations of the heads, base head needs to be first!
	public bool isBoss = false;
	// because we tween monsters, the position we want to get for them is sometimes the position they SHOULD be at
	private Vector3 idealPos;
	private int currentHealth;
	private GameObject baseHeadToMove;	// Keep track of the first head to check
	private GameObject nextHeadToMove;
	private GameObject nextHeadToDestroy;	// Aux to disable the head when finished tweening

	public override void Start(){
		base.Start();
		// the ideal position of the monster is its transform at the outset
		idealPos = transform.position;

		SetupHeads();
	}

	void OnApplicationPause(bool isPaused){
		if(!isPaused){
			SetupHeads();
		}
	}

	public void SetupHeads(){
		if(Application.loadedLevelName == SceneUtils.YARD){
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
	
	/// <summary>
	/// Damages the gate.
	/// </summary>
	/// <param name="damage">Damage.</param>
	protected override void GateDamaged(int damage){
//		AudioManager.Instance.PlayClip("coinDrop");
//		int randomNumberOfCoins = Random.Range(2, 6);
//		for(int i = 0; i < randomNumberOfCoins; ++i){
//			// spawn the item to be coming out of this box
//			GameObject goPrefab = Resources.Load("DroppedStat") as GameObject;
//			GameObject goDroppedItem = Instantiate(goPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
//			DroppedObjectStat droppedObjectStat = goDroppedItem.GetComponent<DroppedObjectStat>();
//
//			droppedObjectStat.Init(HUDElementType.Stars, 5);
//			droppedObjectStat.modeTypes.Add(UIModeTypes.GatingSystem);
//			
//			// set the position of the newly spawned item to be wherever this item box is
//			Vector3 vPosition = gameObject.transform.position;
//			vPosition.y -= 8; //drop the stat underneath the smoke monster
//			goDroppedItem.transform.position = new Vector3(vPosition.x, vPosition.y, 20);
//			
//			// make the item "burst" out
//			droppedObjectStat.Burst(isBurstToLeftOnly:true, burstStreamOrder : i);
//		}

		// Drop some coins when the gate monster is attacked
		if(!isBoss){
		StatsController.Instance.ChangeStats(deltaStars: 35, starsLoc: nextHeadToMove.transform.position, is3DObject: true);
		}
		else{
			StatsController.Instance.ChangeStats(deltaStars: 150, starsLoc: nextHeadToMove.transform.position, is3DObject: true);
		}
		// Move one of the heads out, ONLY applies to everything thats not the first head
		if(baseHeadToMove != nextHeadToMove){
			MoveSubHead();
			// Update the pointer to the next head
			nextHeadToDestroy = nextHeadToMove;
			nextHeadToMove = smokeMonsterHeads[DataManager.Instance.GameData.GatingProgress.GatingProgress[gateID] - 1];
		}
	}	

	protected override void GateDestroyed(){
		// Move all the heads that is NOT the base head
		MoveBaseHead();
	}
	
	/// <summary>
	/// Gets the ideal position.
	/// </summary>
	/// <returns>The ideal position.</returns>
	protected override Vector3 GetIdealPosition(){
		// because monsters move via tweens, we really want to return the position the monster is moving to in some cases	
		return idealPos;
	}	

	private void MoveSubHead(){
		Vector3 newLoc = nextHeadToMove.transform.localPosition;
		newLoc.x += 80;

		LeanTween.moveLocal(nextHeadToMove.transform.parent.gameObject, newLoc, tweenTime)
			.setOnComplete(DisableSubHead).setEase(LeanTweenType.easeInQuad);

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
			.setOnComplete(OnDestroyAnimComplete).setEase(LeanTweenType.easeInQuad);

		baseHeadToMove.GetComponent<Animator>().Play("smokeMonsterDie");
	}

	private void DisableSubHead(){
		nextHeadToDestroy.SetActive(false);
	}
}
