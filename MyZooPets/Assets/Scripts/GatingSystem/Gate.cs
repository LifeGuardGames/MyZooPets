using UnityEngine;
using System.Collections;

//---------------------------------------------------
// Gate
// This is a gate that blocks the user's path.  In
// reality this is more like the personification of
// gate data as a game object.
//---------------------------------------------------

public abstract class Gate : MonoBehaviour{
	// ----- Pure Abstract -------------------------
	protected abstract void OnGateDamaged(int nDamage);	// when a gate is damaged
	protected abstract void OnGateDestroyed();			// what to do when this gate is destroyed
	// ---------------------------------------------

	public float fPlayerBuffer;	// the % in screen space that the player should walk in front of the gate when approaching it
	public float fPlayerY; // the y value the player should move to when approaching the gate
	
	// id and resource of this gate
	protected string gateID;
	protected string gateResource;
	protected float maxScreenSpace; // the max screen space the gate covers with 100% HP

	private ItemBoxLogic scriptItemBox; // the item box this gate is blocking (if any)
	
	protected ImmutableDataGate GetGateData(){
		ImmutableDataGate data = DataLoaderGate.GetData(gateID);
		return data;
	}
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------		
	public virtual void Start(){}

	public void Init(string id, ImmutableDataMonster monster, float maxScreenSpace){
		if(string.IsNullOrEmpty(gateID))
			gateID = id;
		else
			Debug.LogError("Something trying to set id on gate twice " + gateID);
		
		gateResource = monster.GetResourceKey();

		this.maxScreenSpace = maxScreenSpace;
		
		// since this gate is getting created, if it is guarding an item box, create the box
		ImmutableDataGate dataGate = GetGateData();
		string strItemBoxID = dataGate.GetItemBoxID();
		if(!string.IsNullOrEmpty(strItemBoxID)){
			GameObject goResource = Resources.Load("ItemBox_Monster") as GameObject;
			GameObject goBox = Instantiate(goResource, 
			                               new Vector3(transform.position.x, transform.position.y, goResource.transform.position.z), 
			                               Quaternion.identity) as GameObject;
			goBox = goBox.FindInChildren("Button");
			
			scriptItemBox = goBox.GetComponent<ItemBoxLogic>();
			if(scriptItemBox)
				scriptItemBox.SetItemBoxID(strItemBoxID);
			else
				Debug.LogError("No logic script on box", goBox);
		}		
	}

	
	//---------------------------------------------------
	// GreetPlayer()
	// For when the player enters a room with this gate.
	//---------------------------------------------------		
	public void GreetPlayer(){
		// play a sound
		AudioManager.Instance.PlayClip("EnterSmokeMonster");
	}
	
	//---------------------------------------------------
	// GetPlayerPosition()
	// Returns where the pet should be standing when
	// approaching the gate.
	//---------------------------------------------------	
	public Vector3 GetPlayerPosition(){
		// get the screen location of the gate and find out where the player should be with the buffer
		Vector3 vPos = GetIdealPosition();
		Vector3 vScreenLoc = Camera.main.WorldToScreenPoint(vPos);
		float fMoveWidth = Screen.width * (fPlayerBuffer / 100);
		
		// get the target location and then transform it into world coordinates MOVE_DIR
		Vector3 vNewLoc = vScreenLoc;
		vNewLoc.x -= fMoveWidth;
		Vector3 vNewLocWorld = Camera.main.ScreenToWorldPoint(vNewLoc);
		vNewLocWorld.y = fPlayerY;
		
		// we need to apply a Z offset to the pet so that the pet is kind of in front of the monster
		float fOffsetZ = Constants.GetConstant<float>("PetOffsetZ");
		vNewLocWorld.z -= fOffsetZ;
		
		return vNewLocWorld;		
	}
	
	//---------------------------------------------------
	// GetIdealPosition()
	// Returns the ideal position of this gate.  Note that
	// this may not always be the transform position in
	// some children.
	//---------------------------------------------------		
	protected virtual Vector3 GetIdealPosition(){
		return transform.position;	
	}
	
	//---------------------------------------------------
	// GetGateHP()
	//---------------------------------------------------		
	public int GetGateHP(){
		return DataManager.Instance.GameData.GatingProgress.GetGateHP(gateID);
	}
	
	//---------------------------------------------------
	// DamageGate()
	// The user has done something to damage the gate.
	//---------------------------------------------------	
	public bool GateDamaged(int nDamage){
		// this is kind of convoluted, but to actually damage the gate we want to edit the info in the data manager
		bool bDestroyed = DamageGate_SaveData(gateID, nDamage);
		
		// because the gate was damaged, play a sound
		AudioManager.Instance.PlayClip("DamageSmokeMonster");
		
		// let children know that the gate was damaged so they can react in their own way
		OnGateDamaged(nDamage);
		
		if(bDestroyed){
			Analytics.Instance.GateUnlocked(gateID);	
			PrepGateDestruction();
		}
		
		return bDestroyed;
	}
	
	//---------------------------------------------------
	// DamageGate_SaveData()
	// Damages a gate with strID for nDamage, and handles
	// the save data side of things.  This used to be in
	// the actual save data class for gates, but was moved
	// out to here.
	//---------------------------------------------------		
	public bool DamageGate_SaveData(string strID, int nDamage){
		// check to make sure the gate exists
		if(!DataManager.Instance.GameData.GatingProgress.GatingProgress.ContainsKey(strID)){
			Debug.LogError("Something trying to access a non-existant gate " + strID);
			return true;
		}
		
		// check to make sure the gate is active
		if(!DataManager.Instance.GameData.GatingProgress.IsGateActive(strID)){
			Debug.LogError("Something trying to damage an inactive gate " + strID);
			return true;
		}
		
		// otherwise, calculate and save the new hp
		int nHP = DataManager.Instance.GameData.GatingProgress.GatingProgress[strID];
		nHP = Mathf.Max(nHP - nDamage, 0);
		DataManager.Instance.GameData.GatingProgress.GatingProgress[strID] = nHP;
		
		// then return whether or not the gate has been destroyed
		bool bDestroyed = nHP <= 0;

		return bDestroyed;
	}	
	
	//---------------------------------------------------
	// PrepGateDestruction()
	// Starts the process of removing this gate.
	//---------------------------------------------------		
	private void PrepGateDestruction(){
		// play a sound
		AudioManager.Instance.PlayClip("DefeatSmokeMonster");
		
		// let the gating manager know
		GatingManager.Instance.GateCleared();
		
		// if there is an item box behind this gate, let it know it is now unclaimed
		if(scriptItemBox)
			scriptItemBox.NowAvailable();
		
		// gates might do their own thing upon destruction
		OnGateDestroyed();	
		
		// add any appropriate task unlocks
		ImmutableDataGate data = GetGateData();
		string[] arrayUnlocks = data.GetTaskUnlocks();
		for(int i = 0; i < arrayUnlocks.Length; ++i)
			WellapadMissionController.Instance.UnlockTask(arrayUnlocks[i]);		
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
	private void OnDestroyAnimComplete(){		
		// destroy the object
		Destroy(gameObject);			
	}
}
