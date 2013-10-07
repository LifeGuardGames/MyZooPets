﻿using UnityEngine;
using System.Collections;

//---------------------------------------------------
// Gate
// This is a gate that blocks the user's path.  In
// reality this is more like the personification of
// gate data as a game object.
//---------------------------------------------------

public class Gate : MonoBehaviour {
	// id and resource of this gate
	protected string strID;
	protected string strResource;
	public void Init( string id, DataMonster monster ) {
		if ( string.IsNullOrEmpty( strID ) )
			strID = id;	
		else
			Debug.Log("Something trying to set id on gate twice " + strID);
		
		strResource = monster.GetResourceKey();
	}

	// the % in screen space that the player should walk in front of the gate when approaching it
	public float fPlayerBuffer;	
	
	// the y value the player should move to when approaching the gate
	public float fPlayerY;
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------		
	void Start() {
		_Start();
	}
	
	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------		
	protected virtual void _Start() {
		// children implement this
	}
	
	//---------------------------------------------------
	// GreetPlayer()
	// For when the player enters a room with this gate.
	//---------------------------------------------------		
	public void GreetPlayer() {
		// play a sound
		AudioManager.Instance.PlayClip( "Enter_" + strResource );
	}
	
	//---------------------------------------------------
	// GetPlayerPosition()
	// Returns where the pet should be standing when
	// approaching the gate.
	//---------------------------------------------------	
	public Vector3 GetPlayerPosition() {
		// get the screen location of the gate and find out where the player should be with the buffer
		Vector3 vPos = GetIdealPosition();
		Vector3 vScreenLoc = Camera.main.WorldToScreenPoint( vPos );
		float fMoveWidth = Screen.width * ( fPlayerBuffer / 100 );
		
		// get the target location and then transform it into world coordinates MOVE_DIR
		Vector3 vNewLoc = vScreenLoc;
		vNewLoc.x -= fMoveWidth;
		Vector3 vNewLocWorld = Camera.main.ScreenToWorldPoint(vNewLoc);
		vNewLocWorld.y = fPlayerY;
		
		return vNewLocWorld;		
	}
	
	//---------------------------------------------------
	// GetIdealPosition()
	// Returns the ideal position of this gate.  Note that
	// this may not always be the transform position in
	// some children.
	//---------------------------------------------------		
	protected virtual Vector3 GetIdealPosition() {
		return transform.position;	
	}
	
	//---------------------------------------------------
	// DamageGate()
	// The user has done something to damage the gate.
	//---------------------------------------------------	
	public void DamageGate( int nDamage ) {
		// this is kind of convoluted, but to actually damage the gate we want to edit the info in the data manager
		bool bDestroyed = DataManager.Instance.GameData.GatingProgress.DamageGate( strID, nDamage );
		
		// because the gate was damaged, play a sound
		AudioManager.Instance.PlayClip( "Damage_" + strResource );
		
		// let children know that the gate was damaged so they can react in their own way
		_DamageGate( nDamage );
		
		if ( bDestroyed )
			DestroyGate();
	}
	
	//---------------------------------------------------
	// DestroyGate()
	// Remove this gate.
	//---------------------------------------------------		
	private void DestroyGate() {
		// play a sound
		AudioManager.Instance.PlayClip( "Defeat_" + strResource );
		
		// let the gating manager know
		GatingManager.Instance.GateCleared();
		
		// destroy the object
		Destroy( gameObject );		
	}
	
	//---------------------------------------------------
	// _DamageGate()
	//---------------------------------------------------	
	protected virtual void _DamageGate( int nDamage ) {
		// children should implement this	
	}
}
