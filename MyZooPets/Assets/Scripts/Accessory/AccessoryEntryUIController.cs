using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum AccessoryButtonType{
	UnboughtLocked,		// Not yet bought, use BuyButton but locked
	Unbought,			// Not yet bought, use BuyButton
	BoughtEquipped,		// Bought and equipped, use UnequipButton
	BoughtUnequipped	// Bought and not equipped, use EquipButton
}

/// <summary>
/// Store item entry.
/// </summary>
public class AccessoryEntryUIController : MonoBehaviour{
	private AccessoryItem itemData;

	// various elements on the entry
	public Text labelName;
	public Text labelCost;
	public Image spriteIcon;

	public Image buyButtonIcon;
	public Button buyButton;
	public Button equipButton;
	public Button unequipButton;

	private bool islockExists = false;

	public static GameObject CreateEntry(GameObject goGrid, GameObject goPrefab, Item item){
		GameObject itemUIObject = GameObjectUtils.AddChild(goGrid, goPrefab);
		AccessoryEntryUIController entryController = itemUIObject.GetComponent<AccessoryEntryUIController>();
		entryController.Init(item);	// Assigning unequip button
		return itemUIObject;
	}
	
	/// <summary>
	/// This function does the work and actually sets the UI labels, sprites, etc for this entry based on
	/// the incoming item data.
	/// </summary>
	public void Init(Item newItemData){
		// set the proper values on the entry
		gameObject.name = newItemData.ID;

		// Cache this information
		itemData = (AccessoryItem)newItemData;

		string costText = newItemData.Cost.ToString();
		labelCost.text = costText;
		labelName.text = newItemData.Name;
		spriteIcon.sprite = SpriteCacheManager.GetSprite(newItemData.TextureName);

		CheckState();
	}

	public void CheckState(){
		// Check if accessory has already been bought. Change the button if so
		if(itemData.Type == ItemType.Accessories){
			// Check if the item has been equipped, this is mutually exclusive from owning it
			if(DataManager.Instance.GameData.Accessories.PlacedAccessories.ContainsValue(itemData.ID)){
				// Bought and equipped, show unequipped button
				SetState(AccessoryButtonType.BoughtEquipped);
			}
			// Check if the item has been bought
			else if(InventoryManager.Instance.IsAccessoryBought(itemData.ID)){
				// Show the equip button
				SetState(AccessoryButtonType.BoughtUnequipped);
			}
			// If this item is currently locked...
			else if(itemData.IsLocked()){
				// Hide all the buttons
				SetState(AccessoryButtonType.UnboughtLocked);
			}
			// Unlocked but not bought yet
			else{
				SetState(AccessoryButtonType.Unbought);
			}
		}
		else{
			Debug.LogError("Non-Accessory detected");
		}
	}

	public void SetState(AccessoryButtonType buttonType){
		switch(buttonType){
		case AccessoryButtonType.UnboughtLocked:
			// Only want to spawn one lock
			if(!islockExists){
				islockExists = true;

				// Show the lock
				GameObject goLock = LevelLockObject.CreateLock(spriteIcon.gameObject.transform.parent.gameObject, itemData.UnlockAtLevel);
				goLock.transform.localPosition = new Vector3(196f, 0f, -2f);
				goLock.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
			}

			buyButton.gameObject.SetActive(false);
			unequipButton.gameObject.SetActive(false);
			equipButton.gameObject.SetActive(false);
			break;
		case AccessoryButtonType.Unbought:
			buyButton.gameObject.SetActive(true);
			unequipButton.gameObject.SetActive(false);
			equipButton.gameObject.SetActive(false);
			break;
		case AccessoryButtonType.BoughtEquipped:
			buyButton.gameObject.SetActive(false);
			unequipButton.gameObject.SetActive(true);
			equipButton.gameObject.SetActive(false);
			break;
		case AccessoryButtonType.BoughtUnequipped:
			buyButton.gameObject.SetActive(false);
			unequipButton.gameObject.SetActive(false);
			equipButton.gameObject.SetActive(true);
			break;
		default:
			Debug.LogError("Invalid state for button type");
			break;
		}
	}

	public void OnBuyButton(){
		AccessoryUIManager.Instance.BuyAccessory(itemData);
	}

	public void OnEquipButton(){
		AccessoryUIManager.Instance.EquipAccessory(itemData.ID);
	}

	public void OnUnequipButton(){
		AccessoryUIManager.Instance.UnequipAccessory(itemData.ID);
	}
}
