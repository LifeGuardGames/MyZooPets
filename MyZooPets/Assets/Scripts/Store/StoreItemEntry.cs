using UnityEngine;
using System.Collections;

//---------------------------------------------------
// StoreItemEntry
// An individual item entry in the store UI.
//---------------------------------------------------

public class StoreItemEntry : MonoBehaviour {
	// various elements on the entry
	public UILabel labelName;
	public UILabel labelDesc;
	public UILabel labelCost;
	public UISprite spriteIcon;
	public UIButtonMessage buttonMessage;
	
	//---------------------------------------------------
	// CreateEntry()
	// Instantiates and inits the entry with an incoming
	// grid, prefab, and item data.
	//---------------------------------------------------	
	public static void CreateEntry( GameObject goGrid, GameObject goPrefab, Item item ) {
		GameObject itemUIObject = NGUITools.AddChild( goGrid, goPrefab );
		itemUIObject.GetComponent<StoreItemEntry>().Init( item );
	}
	
	//---------------------------------------------------
	// Init()
	// This function does the work and actually sets the
	// UI labels, sprites, etc for this entry based on
	// the incoming item data.
	//---------------------------------------------------	
	public void Init(Item itemData){
		// set the proper values on the entry
		gameObject.name = itemData.ID;
		labelCost.text = itemData.Cost.ToString();
		labelName.text = itemData.Name;
		spriteIcon.spriteName = itemData.TextureName;
		buttonMessage.target = StoreUIManager.Instance.gameObject;
		buttonMessage.functionName = "OnBuyButton";		
	
		//Check if wallpaper has already been bought. Disable the buy button
		//if so	
		if(itemData.Type == ItemType.Decorations){
			DecorationItem decoItem = (DecorationItem) itemData;

			if(decoItem.DecorationType == DecorationTypes.Wallpaper){
				bool isWallpaperBought = InventoryLogic.Instance.CheckForWallpaper(decoItem.ID);

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
	
	//---------------------------------------------------
	// SetDesc()
	// Set the description for this item.
	//---------------------------------------------------		
	protected virtual void SetDesc(Item itemData){
		labelDesc.text = itemData.Description;
	}
}
