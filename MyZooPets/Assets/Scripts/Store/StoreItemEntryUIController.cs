using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Store item entry.
/// </summary>
public class StoreItemEntryUIController : MonoBehaviour{
	// various elements on the entry
	public Text labelName;
	public Text labelDesc;
	public Text labelCost;
	public Image spriteIcon;
	public Image buttonIcon;
	public Button buttonMessage;

	/// <summary>
	/// Creates the entry.
	/// </summary>
	/// <param name="goGrid">grid to add game object to.</param>
	/// <param name="goPrefab">prefab to instantiate.</param>
	/// <param name="item">Item.</param>
	public static GameObject CreateEntry(GameObject goGrid, GameObject goPrefab, 
	                               Item item, GameObject buyButtonMessageTarget = null,
	                               string buyButtonMessageFunctionName = ""){

		GameObject itemUIObject = GameObjectUtils.AddChildGUI(goGrid, goPrefab);

		//set default buy button message target/function name if they are null
		if(buyButtonMessageTarget == null || string.IsNullOrEmpty(buyButtonMessageFunctionName)){
			buyButtonMessageTarget = StoreUIManager.Instance.gameObject;
			buyButtonMessageFunctionName = "OnBuyButton";
		}

		itemUIObject.GetComponent<StoreItemEntryUIController>().Init(item, 
		                                                             buyButtonMessageTarget,
		                                                             buyButtonMessageFunctionName);

		return itemUIObject;
	}

	/// <summary>
	/// Init the specified itemData.
	/// This function does the work and actually sets the
	/// UI labels, sprites, etc for this entry based on
	/// the incoming item data.
	/// </summary>
	/// <param name="itemData">Item data.</param>
	public void Init(Item itemData, GameObject buyButtonMessageTarget,
	                 string buyButtonMessageFunctionName){
		// set the proper values on the entry
		gameObject.name = itemData.ID;

		string costText = itemData.Cost.ToString();
		labelCost.text = costText;
		labelName.text = itemData.Name;
		spriteIcon.sprite = SpriteCacheManager.GetSprite(itemData.TextureName);
		buttonMessage.onClick.AddListener(() => StoreUIManager.Instance.OnBuyButton(buttonMessage.gameObject));
	
		//Check if wallpaper has already been bought. Disable the buy button if so
		if(itemData.Type == ItemType.Decorations){
			DecorationItem decoItem = (DecorationItem)itemData;

			if(decoItem.DecorationType == DecorationTypes.Wallpaper){
				bool isWallpaperBought = InventoryManager.Instance.IsWallpaperBought(decoItem.ID);

				if(isWallpaperBought)
					buttonMessage.gameObject.GetComponent<UIImageButton>().isEnabled = false;
			}
		}

		// set the description
		SetDesc(itemData);
		
		// if this item is currently locked...
		if(itemData.IsLocked()){
			// show the UI
			LevelLockObject.CreateLock(spriteIcon.gameObject.transform.parent.gameObject, itemData.UnlockAtLevel);
			
			// delete the buy button
			Destroy(buttonMessage.gameObject);
		}
	}

	/// <summary>
	/// Sets the desc.
	/// </summary>
	/// <param name="itemData">Item data.</param>
	protected virtual void SetDesc(Item itemData){
		labelDesc.text = itemData.Description;
	}
}
