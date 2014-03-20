﻿using UnityEngine;
using System.Collections;

//---------------------------------------------------
// GateMonster
// This is a gate that is a monster.
// If we ever change which direction a monster is
// blocking, be sure to update the code where MOVE_DIR
// is placed.
//---------------------------------------------------

public class GateMonster : Gate {
	// the screen % this monster moves per % of health
	public float fMove;
	
	// time it takes the monster to tween to its new position after taking damage
	public float fTweenTime;
	
	// script that controls the anims for this monster
	public Animator smokeMonsterAnimator;
	
	// because we tween monsters, the position we want to get for them is sometimes the position they SHOULD be at
	private Vector3 vIdealPos;
	
	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------		
	protected override void _Start() {
		// the ideal position of the monster is its transform at the outset
		vIdealPos = transform.position;
		
		// the monster's position should be set relative to its hp
		DataGate data = DataGateLoader.GetData( strID );
		int nMax = data.GetMonster().GetMonsterHealth();
		int nCurrent = DataManager.Instance.GameData.GatingProgress.GatingProgress[strID];
		int nDamage = nMax - nCurrent;
		
		// if the monster is missing hp, it needs to move
		if ( nDamage > 0 )
			Move( nDamage );

		smokeMonsterAnimator.Play("smokeMonsterNormal");
	}	
	
	//---------------------------------------------------
	// _DamageGate()
	//---------------------------------------------------	
	protected override void _DamageGate( int nDamage ) {
		// when a monster is damaged, it physically moves
		// for now, they will always move to the left...
		Move( nDamage );
	}	
	
	//---------------------------------------------------
	// OnGateDestroyed()
	//---------------------------------------------------	
	protected override void OnGateDestroyed() {
		// for monsters, just move them fast and far away MOVE_DIR
		float fTime = Constants.GetConstant<float>( "MonsterDeath_MoveTime" );
		float fDistance = CameraManager.Instance.GetPanScript().partitionOffset;
		
		// add hashtable params for alerting the parent object when the move anim is complete
        Hashtable optional = new Hashtable();
        optional.Add("onCompleteTarget", gameObject);
        optional.Add("onComplete", "OnDestroyAnimComplete");
		
		Vector3 vTarget = gameObject.transform.position;
		vTarget.x += fDistance;
		
		LeanTween.moveLocal(gameObject, vTarget, fTime, optional );	
	}	
	
	//---------------------------------------------------
	// Move()
	// Moves this monster based on the incoming damage.
	//---------------------------------------------------	
	private void Move( int nDamage ) {
		// get the monster's data and find out how far it should move based on the damage it just received
		DataGate data = DataGateLoader.GetData( strID );
		int nMax = data.GetMonster().GetMonsterHealth();
		float fPercent = ( (float) nDamage / (float) nMax ) * 100;
		float fMovePercent = fPercent * fMove;
		
		// get the screen location of the monster and find out where it should move based on the width of the screen
		Vector3 vScreenLoc = Camera.main.WorldToScreenPoint( transform.position );
		float fMoveWidth = Screen.width * ( fMovePercent / 100 );
		
		// get the new screen location of the monster, then tranform it into world location MOVE_DIR
		Vector3 vNewLoc = vScreenLoc;
		vNewLoc.x += fMoveWidth;
		Vector3 vNewLocWorld = Camera.main.ScreenToWorldPoint(vNewLoc);

		// play a hurt animation on the monster
		//controller.PlayHurtAnimation();
		smokeMonsterAnimator.Play("smokeMonsterHurt");

		// update our ideal position with where we are moving too
		vIdealPos = vNewLocWorld;
		
		// phew...now move this bad boy!
		LeanTween.moveLocal(gameObject, vNewLocWorld, fTweenTime );	
	}
	
	//---------------------------------------------------
	// GetIdealPosition()
	//---------------------------------------------------		
	protected override Vector3 GetIdealPosition() {
		// because monsters move via tweens, we really want to return the position the monster is moving to in some cases	
		return vIdealPos;
	}	
}
