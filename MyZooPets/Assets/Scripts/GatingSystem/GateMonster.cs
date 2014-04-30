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
	public float fTweenTime; // time it takes the monster to tween to its new position after taking damage
	public Animator smokeMonsterAnimator; // script that controls the anims for this monster
	
	// because we tween monsters, the position we want to get for them is sometimes the position they SHOULD be at
	private Vector3 idealPos;

	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------		
	public override void Start(){
		base.Start();
		// the ideal position of the monster is its transform at the outset
		idealPos = transform.position;

		// the monster's position should be set relative to its hp
		ImmutableDataGate data = DataLoaderGate.GetData(gateID);
		int maxHealth = data.GetMonster().GetMonsterHealth();
		int currentHealth = DataManager.Instance.GameData.GatingProgress.GatingProgress[gateID];
		int damage = maxHealth - currentHealth; 
		
		// if the monster is missing hp, it needs to move
		if(damage > 0)
			Move(damage);

		PlayNormalAnimation();
	}	
	
	/// <summary>
	/// Damages the gate.
	/// </summary>
	/// <param name="damage">Damage.</param>
	protected override void OnGateDamaged(int damage){
		// when a monster is damaged, it physically moves
		// for now, they will always move to the right...
		Move(damage);
	}	
	
	//---------------------------------------------------
	// OnGateDestroyed()
	//---------------------------------------------------	
	protected override void OnGateDestroyed(){
		// for monsters, just move them fast and far away MOVE_DIR
		float fTime = Constants.GetConstant<float>("MonsterDeath_MoveTime");
		float fDistance = CameraManager.Instance.GetPanScript().partitionOffset;
		
		// add hashtable params for alerting the parent object when the move anim is complete
		Hashtable optional = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "OnDestroyAnimComplete");
		
		Vector3 vTarget = gameObject.transform.position;
		vTarget.x += fDistance;

		smokeMonsterAnimator.Play("smokeMonsterHurt");

		// Cancel the move tweens previous to this
		LeanTween.cancel(gameObject);
		LeanTween.moveLocal(gameObject, vTarget, fTime, optional);	
	}

	//---------------------------------------------------
	// GetIdealPosition()
	//---------------------------------------------------		
	protected override Vector3 GetIdealPosition(){
		// because monsters move via tweens, we really want to return the position the monster is moving to in some cases	
		return idealPos;
	}	
	
	//---------------------------------------------------
	// Move()
	// Moves this monster based on the incoming damage.
	//---------------------------------------------------	
	private void Move(int damage){
		// get the monster's data and find out how far it should move based on the damage it just received
		ImmutableDataGate data = DataLoaderGate.GetData(gateID);
		int maxHealth = data.GetMonster().GetMonsterHealth();
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
		LeanTween.moveLocal(gameObject, vNewLocWorld, fTweenTime, optional);	
	}

	private void PlayNormalAnimation(){
		smokeMonsterAnimator.Play("smokeMonsterNormal");
	}

	private void PlayHurtAnimation(){
		smokeMonsterAnimator.Play("smokeMonsterHurt");
	}
	

}
