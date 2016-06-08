using UnityEngine;
using System;
using System.Collections;

public class FireButtonUIManager : Singleton<FireButtonUIManager> {
	public static EventHandler<EventArgs> FireButtonActive;

	public FireButtonAnimHelper animHelper;
	public GameObject toggleParent;
	public GameObject fireOrbDropTarget;
	public GameObject sunBeam;

	public GameObject fireButton;
	public GameObject FireButton{
		get{ return fireButton; }
	}

	// Components of fireButton
	private UIImageButton imageButton;
	private ButtonFireButton fireButtonScript;
	public ButtonFireButton FireButtonScript{
		get{ return fireButtonScript; }
	}

	private Collider fireButtonCollider;
	public Collider FireButtonCollider{
		get{ return fireButtonCollider; }
	}

	private string activeButtonSpriteName = "fireButtonOn";
	private string inactiveButtonSpriteName = "fireButtonOff";

	private bool isActive = false;
	public bool IsActive{
		get{ return isActive; }
	}

	void Start () {
		imageButton = fireButton.GetComponent<UIImageButton>();
		fireButtonCollider = fireButton.GetComponent<Collider>();
		fireButtonScript = fireButton.GetComponent<ButtonFireButton>();

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
		InventoryUIManager.ItemDroppedOnTargetEvent += ItemDroppedOnTargetEventHandler;
		
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
		InventoryUIManager.ItemDroppedOnTargetEvent -= ItemDroppedOnTargetEventHandler;
	}

	public void FireEffectOff(){
		animHelper.buttonAnimation.Stop();
		sunBeam.SetActive(false);
		
		imageButton.hoverSprite = inactiveButtonSpriteName;
		imageButton.normalSprite = inactiveButtonSpriteName;
		imageButton.disabledSprite = inactiveButtonSpriteName;
		imageButton.pressedSprite = inactiveButtonSpriteName;

		imageButton.gameObject.SetActive(false);
		imageButton.gameObject.SetActive(true);
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
		imageButton.hoverSprite = activeButtonSpriteName;
		imageButton.normalSprite = activeButtonSpriteName;
		imageButton.disabledSprite = activeButtonSpriteName;
		imageButton.pressedSprite = activeButtonSpriteName;

		imageButton.gameObject.SetActive(false);
		imageButton.gameObject.SetActive(true);
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
}
