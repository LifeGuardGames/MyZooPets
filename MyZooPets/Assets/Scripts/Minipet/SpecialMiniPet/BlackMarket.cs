using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BlackMarket :SingletonUI<BlackMarket> {

	public static EventHandler<EventArgs> OnDecorationItemBought;
	public GameObject grid;
	public GameObject itemStorePrefab;		//basic ui setup for an individual item
	public GameObject itemStorePrefabStats;	// a stats item entry
	public GameObject itemSpritePrefab; 	// item sprite for inventory
	public GameObject itemArea; 			//Where the items will be display
	public GameObject storeBgPanel;			// the bg of the store (sub panel and base panel)
	public GameObject backButton; 			// exit button reference
	public string soundBuy;
	public GameObject storeSubPanel; 		//Where you choose item sub category

	protected override void Awake(){
		base.Awake();
		eModeType = UIModeTypes.Store;
	}

	protected override void Start(){
		base.Start();
		// Reposition all the things nicely to stretch to the end of the screen
		
		// Position the UIPanel clipping range
		UIPanel itemAreaPanel = itemArea.GetComponent<UIPanel>();
		Vector4 oldRange = itemAreaPanel.clipRange;
		
		// The 52 comes from some wierd scaling issue.. not sure what it is but compensate now
		itemAreaPanel.transform.localPosition = new Vector3(52f, itemAreaPanel.transform.localPosition.y, 0f);
		itemAreaPanel.clipRange = new Vector4(52f, oldRange.y, (float)(CameraManager.Instance.NativeWidth), oldRange.w);
		
		// Position the grid origin to the left of the screen
		Vector3 gridPosition = grid.transform.localPosition;
		grid.transform.localPosition = new Vector3(
			(-1f * (CameraManager.Instance.NativeWidth / 2)) - itemArea.transform.localPosition.x,
			gridPosition.y, gridPosition.z);
	}

	/// <summary>
	/// Gets the exit button.
	/// </summary>
	/// <returns>The exit button.</returns>
	public GameObject GetBackButton(){
		return backButton;
	}

	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		RoomArrowsUIManager.Instance.HidePanel();
		HUDUIManager.Instance.ShowLabels();
		storeBgPanel.GetComponent<TweenToggleDemux>().Show();
	}
	
	protected override void _CloseUI(){
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();
		RoomArrowsUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		HUDUIManager.Instance.HideLabels();
		storeBgPanel.GetComponent<TweenToggleDemux>().Hide();
		storeSubPanel.GetComponent<TweenToggleDemux>().Hide();
	}

	/// <summary>
	/// This function is called when buying an item. I creates an icon for the item
	/// and move it to Inventory
	/// </summary>
	/// <param name="itemData">Item data.</param>
	/// <param name="sprite">Sprite.</param>
	public void OnBuyAnimation(Item itemData, GameObject sprite){
		Vector3 origin = new Vector3(sprite.transform.position.x, sprite.transform.position.y, -0.1f);
		string itemID = sprite.transform.parent.transform.parent.name;
		Vector3 itemPosition = origin;
		
		//-0.22
		// depending on what type of item the user bought, the animation has the item going to different places
		ItemType eType = itemData.Type;
		switch(eType){
		case ItemType.Decorations:
			itemPosition = DecoInventoryUIManager.Instance.GetPositionOfDecoInvItem(itemID);
			break;
		default:
			itemPosition = InventoryUIManager.Instance.GetPositionOfInvItem(itemID);
			break;
		}
		
		//adjust moving speed here
		float speed = 1f;
		
		//Change the 3 V3 to where icon should move
		Vector3[] path = new Vector3[4];
		path[0] = origin;
		path[1] = origin + new Vector3(0f, 1.5f, -0.1f);
		path[2] = origin;
		path[3] = itemPosition;
		
		
		Hashtable optional = new Hashtable();
		GameObject animationSprite = NGUITools.AddChild(storeSubPanel, itemSpritePrefab);
		
		// hashtable for completion params for the callback (stash the icon we are animating)
		Hashtable completeParamHash = new Hashtable();
		completeParamHash.Add("Icon", animationSprite);		
		
		optional.Add("ease", LeanTweenType.easeOutQuad);
		optional.Add("onComplete", "DestroySprite");
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onCompleteParam", completeParamHash);
		animationSprite.transform.position = origin;
		animationSprite.transform.localScale = new Vector3(90, 90, 1);
		animationSprite.GetComponent<UISprite>().spriteName = sprite.GetComponent<UISprite>().spriteName;
		LeanTween.move(animationSprite, path, speed, optional);
	}

	//---------------------------------------------------
	// DestroySprite()
	// Callback for buy animation -- will destroy the
	// sprite icon clone we create and animated.
	//---------------------------------------------------
	public void DestroySprite(Hashtable hash){
		// delete the icon we moved
		if(hash.ContainsKey("Icon")){
			GameObject goSprite = (GameObject)hash["Icon"];
			Destroy(goSprite);
		}	
	}
	
	/// <summary>
	/// Raises the buy button event.
	/// </summary>
	/// <param name="button">Button.</param>
	public void OnBuyButton(GameObject button){
		Transform buttonParent = button.transform.parent.parent;
		string itemID = buttonParent.name;
		Item itemData = ItemLogic.Instance.GetItem(itemID);
		switch(itemData.CurrencyType){
		case CurrencyTypes.WellaCoin:
			if(DataManager.Instance.GameData.Stats.Stars >= itemData.Cost){
				//Special case to handle here. Since only one wallpaper can be used at anytime
				//There is no point for the user to buy more than one of each diff wallpaper
				if(itemData.Type == ItemType.Decorations){
					DecorationItem decoItem = (DecorationItem)itemData;
					
					if(decoItem.DecorationType == DecorationTypes.Wallpaper){
						UIImageButton buyButton = button.GetComponent<UIImageButton>();
						
						//Disable the buy button so user can't buy the same wallpaper anymore 
						if(buyButton)
							buyButton.isEnabled = false;
					}
				}
				
				InventoryLogic.Instance.AddItem(itemID, 1);
				StatsController.Instance.ChangeStats(deltaStars: (int)itemData.Cost * -1);
				OnBuyAnimation(itemData, buttonParent.gameObject.FindInChildren("ItemTexture"));
				
				//Analytics
				Analytics.Instance.ItemEvent(Analytics.ITEM_STATUS_BOUGHT, itemData.Type, itemData.ID);
				
				// play a sound since an item was bought
				AudioManager.Instance.PlayClip(soundBuy);
			}
			else{
				AudioManager.Instance.PlayClip("buttonDontClick");
			}
			break;
		}
	}
}
