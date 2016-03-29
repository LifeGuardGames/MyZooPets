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
	protected abstract void GateDamaged(int nDamage);	// when a gate is damaged
	protected abstract void GateDestroyed();			// what to do when this gate is destroyed
	// ---------------------------------------------

	public float playerBuffer;	// the % in screen space that the player should walk in front of the gate when approaching it
	public float playerY; // the y value the player should move to when approaching the gate
	
	// id and resource of this gate
	protected string gateID;
	protected string gateResource;
	protected float maxScreenSpace; // the max screen space the gate covers with 100% HP

	private ItemBoxLogic scriptItemBox; // the item box this gate is blocking (if any)
	
	protected ImmutableDataGate GetGateData(){
		ImmutableDataGate data = DataLoaderGate.GetData(gateID);
		return data;
	}

	public virtual void Start(){}

	/// <summary>
	/// Init the specified id, monster and maxScreenSpace.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="monster">Monster.</param>
	/// <param name="maxScreenSpace">Max screen space.</param>
	public void Init(string id, ImmutableDataMonster monster, float maxScreenSpace){
		if(string.IsNullOrEmpty(gateID))
			gateID = id;
		else
			Debug.LogError("Something trying to set id on gate twice " + gateID);
		
		gateResource = monster.ResourceKey;

		this.maxScreenSpace = maxScreenSpace;
		
//		// since this gate is getting created, if it is guarding an item box, create the box
//		ImmutableDataGate dataGate = GetGateData();
//		string itemBoxID = dataGate.ItemBoxID;
//		if(!string.IsNullOrEmpty(itemBoxID)){
//			GameObject goResource = Resources.Load("ItemBox_Monster") as GameObject;
//			GameObject goBox = Instantiate(goResource, 
//			                               new Vector3(transform.position.x + dataGate.ItemBoxPositionOffset, transform.position.y, goResource.transform.position.z), 
//			                               Quaternion.identity) as GameObject;
//			goBox = goBox.FindInChildren("Button");
//			
//			scriptItemBox = goBox.GetComponent<ItemBoxLogic>();
//			if(scriptItemBox)
//				scriptItemBox.SetItemBoxID(itemBoxID);
//			else
//				Debug.LogError("No logic script on box", goBox);
//		}		
	}
			
	/// <summary>
	/// Greets the player.
	/// For when the player enters a room with this gate.
	/// </summary>
	public void GreetPlayer(){
		// play a sound
		AudioManager.Instance.PlayClip("EnterSmokeMonster");
	}

	/// <summary>
	/// Gets the player position.
	/// Returns where the pet should be standing when approching the gate.
	/// </summary>
	/// <returns>The player position.</returns>
	public Vector3 GetPlayerPosition(){
		// get the screen location of the gate and find out where the player should be with the buffer
		Vector3 idealPos = GetIdealPosition();
		Vector3 screenLoc = Camera.main.WorldToScreenPoint(idealPos);
		float moveWidth = Screen.width * (playerBuffer / 100);
		
		// get the target location and then transform it into world coordinates MOVE_DIR
		Vector3 newScreenLoc = screenLoc;
		newScreenLoc.x -= moveWidth;
		Vector3 newWorldLoc = Camera.main.ScreenToWorldPoint(newScreenLoc);
		newWorldLoc.y = playerY;
		
		// we need to apply a Z offset to the pet so that the pet is kind of in front of the monster
		float fOffsetZ = Constants.GetConstant<float>("PetOffsetZ");
		newWorldLoc.z -= fOffsetZ;
		
		return newWorldLoc;		
	}

	/// <summary>
	/// Gets the ideal position.
	/// Returns the ideal position of this gate.  Note that
	/// this may not always be the transform position in
	/// some children.
	/// </summary>
	/// <returns>The ideal position.</returns>
	protected virtual Vector3 GetIdealPosition(){
		return transform.position;	
	}

	public int GetGateHP(){
		return DataManager.Instance.GameData.GatingProgress.GetGateHP(gateID);
	}

	/// <summary>
	/// Damages the gate.
	/// </summary>
	/// <returns><c>true</c>, if gate is destroyed, <c>false</c> otherwise.</returns>
	/// <param name="damage">Damage.</param>
	public bool DamageGate(int damage){
		DataManager.Instance.GameData.PetInfo.amountOfFireBreathsDone++;
		Analytics.Instance.BlowFire(DataManager.Instance.GameData.PetInfo.amountOfFireBreathsDone.ToString());
		// this is kind of convoluted, but to actually damage the gate we want to edit the info in the data manager
		bool isDestroyed = GatingManager.Instance.DamageGate(gateID, damage);
		
		// because the gate was damaged, play a sound
		AudioManager.Instance.PlayClip("DamageSmokeMonster");
		
		// let children know that the gate was damaged so they can react in their own way
		GateDamaged(damage);
		
		if(isDestroyed){
			Analytics.Instance.GateUnlocked(gateID);	
			PrepGateDestruction();
		}
		
		return isDestroyed;
	} 

	/// <summary>
	/// Preps the gate destruction.
	/// </summary>
	private void PrepGateDestruction(){
		// play a sound
		AudioManager.Instance.PlayClip("unlockRoom");
		AudioManager.Instance.PlayClip("DefeatSmokeMonster");
		
		// let the gating manager know
		GatingManager.Instance.GateCleared();

		Invoke("UnlockRoomArrows", 0.5f);
		
		// gates might do their own thing upon destruction
		GateDestroyed();	
		
		// add any appropriate task unlocks
		ImmutableDataGate data = GetGateData();
		string[] arrayUnlocks = data.TaskUnlocks;

		if(arrayUnlocks != null){
			for(int i = 0; i < arrayUnlocks.Length; ++i){
				WellapadMissionController.Instance.UnlockTask(arrayUnlocks[i]);		
			}
		}
	}

	private void UnlockRoomArrows(){
		RoomArrowsUIManager.Instance.ShowPanel();
	}

	/// <summary>
	/// It's up for child gates to properly call this 
	/// function when their destroy animation is complete.
	/// NOTE: I don't think anything important should go
	/// here because at present, the game could exit before
	/// the animation is complete, but the gate is already
	/// marked as destroyed.
	/// </summary>
	protected void OnDestroyAnimComplete(){		
		// destroy the object
		Destroy(gameObject, 2f);
	}
}
