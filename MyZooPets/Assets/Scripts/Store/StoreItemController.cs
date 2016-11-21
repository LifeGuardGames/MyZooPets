using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Store item entry.
/// </summary>
public class StoreItemController : MonoBehaviour {
	// various elements on the entry
	public Text labelName;
	public Text labelDesc;
	public Text labelCost;
	public Image spriteIcon;
	public Button button;

	private Item itemData;  // Internal cache
	public Item ItemData {
		get { return itemData; }
	}

	public void Init(Item _itemData) {
		itemData = _itemData;

		// set the proper values on the entry
		gameObject.name = itemData.ID;

		string costText = itemData.Cost.ToString();
		labelCost.text = costText;
		labelName.text = itemData.Name;
		spriteIcon.sprite = SpriteCacheManager.GetSprite(itemData.TextureName);

		BuyButtonStateCheck();

		// set the description
		SetDescription(itemData);

		// if this item is currently locked...
		if(itemData.IsLocked()) {
			// show the UI
			LevelLockObject.CreateLock(spriteIcon.gameObject.transform.parent.gameObject, itemData.UnlockAtLevel);

			// delete the buy button
			button.gameObject.SetActive(false);
		}
	}

	public void BuyButtonStateCheck() {
		//Check if wallpaper has already been bought. Disable the buy button if so
		if(itemData.Type == ItemType.Decorations) {
			DecorationItem decoItem = (DecorationItem)itemData;
			if(decoItem.DecorationType == DecorationTypes.Wallpaper) {
				if(InventoryManager.Instance.IsWallpaperBought(decoItem.ID)) {
					button.interactable = false;
					button.GetComponent<Image>().sprite = SpriteCacheManager.GetSprite("ButtonGray");
				}
			}
		}
	}

	protected virtual void SetDescription(Item itemData) {
		labelDesc.text = itemData.Description;
	}

	// Button call
	public void OnBuyButton() {
		StoreUIManager.Instance.BuyButtonLogic(this);
	}

	// Return the position of the sprite icon, used for buy animation
	public Vector3 GetSpritePosition() {
		return spriteIcon.rectTransform.position;
	}
}
