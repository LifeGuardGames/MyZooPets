using UnityEngine;
using System.Collections;

//---------------------------------------------------
// Gate
// This is a gate that blocks the user's path.  In
// reality this is more like the personification of
// gate data as a game object.
//---------------------------------------------------

public abstract class Gate : MonoBehaviour {
	// ----- Pure Abstract -------------------------
	protected abstract void _DamageGate( int nDamage );	// when a gate is damaged
	protected abstract void OnGateDestroyed();			// what to do when this gate is destroyed
	// ---------------------------------------------
	
	// the item box this gate is blocking (if any)
	ItemBoxLogic scriptItemBox;
	
	// id and resource of this gate
	protected string strID;
	protected string strResource;
	public void Init( string id, DataMonster monster ) {
		if ( string.IsNullOrEmpty( strID ) )
			strID = id;	
		else
			Debug.Log("Something trying to set id on gate twice " + strID);
		
		strResource = monster.GetResourceKey();
		
		// since this gate is getting created, if it is guarding an item box, create the box
		DataGate dataGate = GetGateData();
		string strItemBoxID = dataGate.GetItemBoxID();
		if ( !string.IsNullOrEmpty( strItemBoxID ) ) {
			GameObject goResource = Resources.Load( "ItemBox_Monster" ) as GameObject;
			GameObject goBox = Instantiate( goResource, transform.position, Quaternion.identity ) as GameObject;
			goBox = goBox.FindInChildren( "Button" );
			
			scriptItemBox = goBox.GetComponent<ItemBoxLogic>();
			if ( scriptItemBox )
				scriptItemBox.SetItemBoxID( strItemBoxID );
			else
				Debug.Log("No logic script on box", goBox);
		}		
	}
	
	protected DataGate GetGateData() {
		DataGate data = DataGateLoader.GetData( strID );
		return data;
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
		
		// we need to apply a Z offset to the pet so that the pet is kind of in front of the monster
		float fOffsetZ = Constants.GetConstant<float>( "PetOffsetZ" );
		vNewLocWorld.z -= fOffsetZ;
		
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
	// GetGateHP()
	//---------------------------------------------------		
	public int GetGateHP() {
		return DataManager.Instance.GameData.GatingProgress.GetGateHP( strID );
	}
	
	//---------------------------------------------------
	// DamageGate()
	// The user has done something to damage the gate.
	//---------------------------------------------------	
	public bool DamageGate( int nDamage ) {
		// this is kind of convoluted, but to actually damage the gate we want to edit the info in the data manager
		bool bDestroyed = DataManager.Instance.GameData.GatingProgress.DamageGate( strID, nDamage );
		
		// because the gate was damaged, play a sound
		AudioManager.Instance.PlayClip( "Damage_" + strResource );
		
		// let children know that the gate was damaged so they can react in their own way
		_DamageGate( nDamage );
		
		if ( bDestroyed )
			PrepGateDestruction();
		
		return bDestroyed;
	}
	
	//---------------------------------------------------
	// PrepGateDestruction()
	// Starts the process of removing this gate.
	//---------------------------------------------------		
	private void PrepGateDestruction() {
		// play a sound
		AudioManager.Instance.PlayClip( "Defeat_" + strResource );
		
		// let the gating manager know
		GatingManager.Instance.GateCleared();
		
		// if there is an item box behind this gate, let it know it is now unclaimed
		if ( scriptItemBox )
			scriptItemBox.NowAvailable();
		
		// gates might do their own thing upon destruction
		OnGateDestroyed();	
		
		// add any appropriate task unlocks
		DataGate data = GetGateData();
		string[] arrayUnlocks = data.GetTaskUnlocks();
		for ( int i = 0; i < arrayUnlocks.Length; ++i )
			WellapadMissionController.Instance.UnlockTask( arrayUnlocks[i] );		
	}
	
	//---------------------------------------------------
	// OnDestroyAnimComplete()
	// It's up for child gates to properly call this 
	// function when their destroy animation is complete.
	// NOTE: I don't think anything important should go
	// here because at present, the game could exit before
	// the animation is complete, but the gate is already
	// marked as destroyed.
	//---------------------------------------------------		
	private void OnDestroyAnimComplete() {		
		// destroy the object
		Destroy( gameObject );			
	}
	
}
