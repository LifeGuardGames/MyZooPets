using UnityEngine;
//---------------------------------------------------
// Gate
// This is a gate that blocks the user's path.  In
// reality this is more like the personification of
// gate data as a game object.
//---------------------------------------------------

public abstract class Gate : MonoBehaviour {
	// ----- Pure Abstract -------------------------
	protected abstract bool GateDamagedDestroy();				// when a gate is damaged
	protected abstract void GateDestroyed();            // what to do when this gate is destroyed
	// ---------------------------------------------

	public float playerBuffer;  // the % in screen space that the player should walk in front of the gate when approaching it
	public float playerY; // the y value the player should move to when approaching the gate

	// id and resource of this gate
	public string gateID;
	protected string gateResource;
	protected float maxScreenSpace; // the max screen space the gate covers with 100% HP

	protected ImmutableDataGate GetGateData() {
		ImmutableDataGate data = DataLoaderGate.GetData(gateID);
		return data;
	}

	public virtual void Start() { }

	/// <summary>
	/// Init the specified id, monster and maxScreenSpace.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="monster">Monster.</param>
	/// <param name="maxScreenSpace">Max screen space.</param>
	public void Init(string id, ImmutableDataMonster monster, float maxScreenSpace) {
		if(string.IsNullOrEmpty(gateID)) {
			gateID = id;
		}
		else {
			Debug.LogError("Something trying to set id on gate twice " + gateID);
		}

		gateResource = monster.ResourceKey;

		this.maxScreenSpace = maxScreenSpace;

	}

	/// <summary>
	/// For when the player enters a room with this gate.
	/// </summary>
	public void GreetPlayer() {
		// play a sound
		AudioManager.Instance.PlayClip("EnterSmokeMonster");
		InventoryUIManager.Instance.ShowPanel(true);
	}

	/// <summary>
	/// Gets the player position.
	/// Returns where the pet should be standing when approching the gate.
	/// </summary>
	/// <returns>The player position.</returns>
	public Vector3 GetPlayerPosition() {
		// get the screen location of the gate and find out where the player should be with the buffer
		Vector3 screenLoc = Camera.main.WorldToScreenPoint(transform.position);
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

	public int GetGateHP() {
		return DataManager.Instance.GameData.GatingProgress.GetGateHP(gateID);
	}

	public void DamageGate() {
		DataManager.Instance.GameData.PetInfo.amountOfFireBreathsDone++;
		Analytics.Instance.BlowFire(DataManager.Instance.GameData.PetInfo.amountOfFireBreathsDone.ToString());
		
		// because the gate was damaged, play a sound
		AudioManager.Instance.PlayClip("DamageSmokeMonster");

		// let children know that the gate was damaged so they can react in their own way
		// If it is boss, destructive = false, need to load MicroMix
		bool isDestructive = GateDamagedDestroy();

		if(isDestructive) {		// Regular smoke monster gate
			// this is kind of convoluted, but to actually damage the gate we want to edit the info in the data manager
			bool isDestroyed = GatingManager.Instance.DamageGate(gateID);
			if(isDestroyed) {
				Analytics.Instance.GateUnlocked(gateID);
				PrepGateDestruction();
			}
		}
	}

	/// <summary>
	/// Preps the gate destruction.
	/// </summary>
	private void PrepGateDestruction() {
		// play a sound
		AudioManager.Instance.PlayClip("unlockRoom");
		AudioManager.Instance.PlayClip("DefeatSmokeMonster");

		// let the gating manager know
		GatingManager.Instance.GateCleared();

		Invoke("UnlockRoomArrows", 0.5f);

		// gates might do their own thing upon destruction
		GateDestroyed();
	}

	private void UnlockRoomArrows() {
		RoomArrowsUIManager.Instance.ShowPanel();
	}
}
