using UnityEngine;
using System.Collections;

//---------------------------------------------------
// GateMonster
// This is a gate that is a monster.
// If we ever change which direction a monster is
// blocking, be sure to update the code where MOVE_DIR
// is placed.
//---------------------------------------------------

public class GateMonster : Gate{
//	public float fMove; // the screen % this monster moves per % of health
	public float tweenTime; // time it takes the monster to tween to its new position after taking damage
	public Animator smokeMonsterAnimator; // script that controls the anims for this monster
	
	// because we tween monsters, the position we want to get for them is sometimes the position they SHOULD be at
	private Vector3 idealPos;


	public override void Start(){
		base.Start();
		// the ideal position of the monster is its transform at the outset
		idealPos = transform.position;

		// the monster's position should be set relative to its hp
		ImmutableDataGate data = DataLoaderGate.GetData(gateID);
		int maxHealth = data.GetMonster().MonsterHealth;
		int currentHealth = DataManager.Instance.GameData.GatingProgress.GatingProgress[gateID];
		int damage = maxHealth - currentHealth; 
		
		// if the monster is missing hp, it needs to move
		if(damage > 0)
//			Move(damage);

		PlayNormalAnimation();
	}	
	
	/// <summary>
	/// Damages the gate.
	/// </summary>
	/// <param name="damage">Damage.</param>
	protected override void GateDamaged(int damage){

		//drop some coins when the gate monster is attacked
		AudioManager.Instance.PlayClip("coinDrop");
		int randomNumberOfCoins = Random.Range(2, 6);
		for(int i = 0; i < randomNumberOfCoins; ++i){
			// spawn the item to be coming out of this box
			GameObject goPrefab = Resources.Load("DroppedStat") as GameObject;
			GameObject goDroppedItem = Instantiate(goPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			DroppedObjectStat droppedObjectStat = goDroppedItem.GetComponent<DroppedObjectStat>();

			droppedObjectStat.Init(HUDElementType.Stars, 5);
			droppedObjectStat.modeTypes.Add(UIModeTypes.GatingSystem);
			
			// set the position of the newly spawned item to be wherever this item box is
			Vector3 vPosition = gameObject.transform.position;
			vPosition.y -= 8; //drop the stat underneath the smoke monster
			goDroppedItem.transform.position = new Vector3(vPosition.x, vPosition.y, 20);
			
			// make the item "burst" out
			droppedObjectStat.Burst(burstToLeftOnly:true);
		}

		// when a monster is damaged, it physically moves
		// for now, they will always move to the right...
//		Move(damage);
	}	

	protected override void GateDestroyed(){
		// for monsters, just move them fast and far away MOVE_DIR
		float monsterDeathMoveTime = Constants.GetConstant<float>("MonsterDeath_MoveTime");
		float monsterMoveDistance = CameraManager.Instance.GetPanScript().partitionOffset;
		
		// add hashtable params for alerting the parent object when the move anim is complete
		Hashtable optional = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "OnDestroyAnimComplete");
		
		Vector3 targetPos = gameObject.transform.position;
		targetPos.x += monsterMoveDistance;

		smokeMonsterAnimator.Play("smokeMonsterHurt");

		// Cancel the move tweens previous to this
		LeanTween.cancel(gameObject);
		LeanTween.moveLocal(gameObject, targetPos, monsterDeathMoveTime, optional);	
	}
	
	/// <summary>
	/// Gets the ideal position.
	/// </summary>
	/// <returns>The ideal position.</returns>
	protected override Vector3 GetIdealPosition(){
		// because monsters move via tweens, we really want to return the position the monster is moving to in some cases	
		return idealPos;
	}	

	/// <summary>
	/// Move the specified damage.
	/// </summary>
	/// <param name="damage">Damage.</param>
	private void Move(int damage){
		// get the monster's data and find out how far it should move based on the damage it just received
		ImmutableDataGate data = DataLoaderGate.GetData(gateID);
		int maxHealth = data.GetMonster().MonsterHealth;
		float damagePercentage = ((float)damage / (float)maxHealth);
		
		// get the screen location of the monster and find out where it should move based on the width of the screen
		Vector3 screenLoc = Camera.main.WorldToScreenPoint(transform.position);

		float moveWidth = maxScreenSpace * damagePercentage;

		// get the new screen location of the monster, then tranform it into world location MOVE_DIR
		Vector3 newLoc = screenLoc;
		newLoc.x += moveWidth;
		Vector3 vNewLocWorld = Camera.main.ScreenToWorldPoint(newLoc);

		// play a hurt animation on the monster
		PlayHurtAnimation();

		// update our ideal position with where we are moving too
		idealPos = vNewLocWorld;
		
		LeanTween.cancel(gameObject);
		// phew...now move this bad boy!
		Hashtable optional = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "PlayNormalAnimation");
		LeanTween.moveLocal(gameObject, vNewLocWorld, tweenTime, optional);	
	}

	private void PlayNormalAnimation(){
		smokeMonsterAnimator.Play("smokeMonsterNormal");
	}

	private void PlayHurtAnimation(){
		smokeMonsterAnimator.Play("smokeMonsterHurt");
	}
	

}
