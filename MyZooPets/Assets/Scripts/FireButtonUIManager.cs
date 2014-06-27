using UnityEngine;
using System.Collections;

public class FireButtonUIManager : MonoBehaviour {

	public GameObject fireOrbDropTarget;
	public Animation buttonPluseAnimation;
	public GameObject sunBeam;
	public UIImageButton imageButton;

	private string activeButtonSpriteName = "buttonFireFull";
	private string inactiveButtonSpriteName = "buttonFireEmpty";

	// Use this for initialization
	void Start () {
		InventoryUIManager.ItemDroppedOnTargetEvent += ItemDroppedOnTargetEventHandler;

		bool canBreatheFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();
		if(!canBreatheFire){
			buttonPluseAnimation.Stop();
			sunBeam.SetActive(false);

			imageButton.hoverSprite = inactiveButtonSpriteName;
			imageButton.normalSprite = inactiveButtonSpriteName;
			imageButton.disabledSprite = inactiveButtonSpriteName;
			imageButton.pressedSprite = inactiveButtonSpriteName;
		}
	}

	void OnDestroy(){
		InventoryUIManager.ItemDroppedOnTargetEvent -= ItemDroppedOnTargetEventHandler;
	}

	private void ItemDroppedOnTargetEventHandler(object sender, InventoryDragDrop.InvDragDropArgs args){
		if(args.TargetCollider.name == fireOrbDropTarget.name){
			Debug.Log("orb on target");

			string invItemID = args.ItemTransform.name; //get id from listener args
			InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);
			
			// check to make sure the item can be used
			if(ItemLogic.Instance.CanUseItem(invItemID)){
				args.IsValidTarget = true;

				//notify inventory logic that this item is being used
				InventoryLogic.Instance.UsePetItem(invItemID);
			}

			//change button image 
			imageButton.hoverSprite = activeButtonSpriteName;
			imageButton.normalSprite = activeButtonSpriteName;
			imageButton.disabledSprite = activeButtonSpriteName;
			imageButton.pressedSprite = activeButtonSpriteName;

			//enable sun beam
			sunBeam.SetActive(true);

			//enable pusle
			buttonPluseAnimation.Play();
		}
	}
}
