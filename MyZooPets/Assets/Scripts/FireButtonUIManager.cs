using UnityEngine;
using UnityEngine.UI;
using System;

public class FireButtonUIManager : Singleton<FireButtonUIManager> {
	public static EventHandler<EventArgs> FireButtonActive;

	public FireButtonAnimHelper animHelper;
	public GameObject toggleParent;
	public GameObject fireOrbDropTarget;
	public GameObject sunBeam;

	public GameObject fireButtonObject;
	public GameObject FireButtonObject{
		get{ return fireButtonObject; }
	}

	public FireMeter fireMeterScript;

	// Components of fireButton
	public Image imageButton;

	public Sprite activeFireButtonSprite;
	public Sprite inactiveFireButtonSprite;

	private bool isActive = false;
	public bool IsActive{
		get{ return isActive; }
	}

	void Start () {
		toggleParent.SetActive(false);
		CameraManager.Instance.PanScript.OnPartitionChanging += OnPartitionChanging;
	}

	void OnDestroy(){
		if(CameraManager.Instance){
			CameraManager.Instance.PanScript.OnPartitionChanging -= OnPartitionChanging;	
		}
	}

	public void OnPartitionChanging(object sender, PartitionChangedArgs args){
		// if the partition is changing at all, destroy this UI
		Deactivate();
	}

	// Called when the pet reaches a smoke monster room
	public void Activate(){
		isActive = true;
		toggleParent.SetActive(true);
//		InventoryUIManager.ItemDroppedOnTargetEvent += ItemDroppedOnTargetEventHandler;
		
		bool canBreatheFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();
		if(!canBreatheFire)
			FireEffectOff();
		else
			FireEffectOn(0);
	}

	// Called when the pet leaves a smoke monster room
	public void Deactivate(){
		isActive = false;
		toggleParent.SetActive(false);
//		InventoryUIManager.ItemDroppedOnTargetEvent -= ItemDroppedOnTargetEventHandler;
	}

	public void FireEffectOff(){
		animHelper.buttonAnimation.Stop();
		sunBeam.SetActive(false);

		imageButton.sprite = inactiveFireButtonSprite;
	}

	// This is called from the animation event / or from start
	public void FireEffectOn(int isCalledFromAnimation){

		// Turn on the pulsing manually because its not done from animation
		if(isCalledFromAnimation == 0){
			animHelper.FireButtonAnimationActivate();
		}

		if(FireButtonActive != null)
			FireButtonActive(this, EventArgs.Empty);

		sunBeam.SetActive(true);

		//change button image 
		imageButton.sprite = activeFireButtonSprite;
	}

	private void ItemDroppedOnTargetEventHandler(object sender, InventoryDragDrop.InvDragDropArgs args){
		if(args.TargetCollider.name == fireOrbDropTarget.name){
			string invItemID = args.ItemTransform.name; //get id from listener args
			InventoryItem invItem = InventoryManager.Instance.GetItemInInventory(invItemID);
			int numOfFireBreaths = DataManager.Instance.GameData.PetInfo.FireBreaths;

			//only works if item is flame crystal and pet can't breathe fire yet
			if(invItem.ItemID == "Usable1" && numOfFireBreaths == 0){
				// check to make sure the item can be used
				if(ItemManager.Instance.CanUseItem(invItemID)){
					args.IsValidTarget = true;
					
					//notify inventory logic that this item is being used
					InventoryManager.Instance.UsePetItem(invItemID);
					
					animHelper.StartFireButtonAnimation();
				}
			}
		}
	}







	

	private AttackGate attackScript;        // attack gate script
	private Gate gate;                      // the gate that this button is for
	private bool isLegal;                   // is this button being pressed legally? Mainly used as a stop gap for now

	public void SetGate(Gate gate) {
		this.gate = gate;
	}

	/// <summary>
	/// When the user presses down on the fire meter button. This will begin
	/// some pet animation prep and start to fill the attached meter
	/// </summary>
	public void OnClick() {
		isLegal = false;
		bool canBreathFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();

		// if can breathe fire, attack the gate!!
		if(canBreathFire) {
			if(SceneUtils.CurrentScene != SceneUtils.YARD) {
				isLegal = true;

				// kick off the attack script
				attackScript = PetAnimationManager.Instance.gameObject.AddComponent<AttackGate>();
				attackScript.Init(gate);

				PetAnimationManager.Instance.StartFireBlow();

				// turn the fire meter on
				fireMeterScript.StartFilling();
			}
			else {
				LoadLevelManager.Instance.StartLoadTransition(SceneUtils.MICROMIX);
			}
		}
		// else can't breathe fire. explain why
		else {
			if(!TutorialManager.Instance.IsTutorialActive()) {
				GatingManager.Instance.IndicateNoFire();
			}
		}
	}



	public void OnButtonReleased() {
		if(!isLegal) {
			//			Debug.Log("Something going wrong with the fire button.  Aborting");
			return;
		}

		if(fireMeterScript.IsMeterFull()) {
			// if the meter was full on release, complete the attack!
			attackScript.FinishAttack();

			// because the user can only ever breath fire once, the only time we don't want to destroy the fire button is when the infinite
			// fire mode cheat is active and the gate is still alive
			if(gate.GetGateHP() <= 1) {
				Deactivate();
			}
			else {
				//disable button
				FireEffectOff();
			}
		}
		else {
			// if the meter was not full, cancel the attack
			attackScript.Cancel();
		}
		// regardless we want to empty the meter
		fireMeterScript.Reset();
	}
}
