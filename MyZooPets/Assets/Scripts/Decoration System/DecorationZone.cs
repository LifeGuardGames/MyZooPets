using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Decoration zone.
/// This is the replacement for decoration nodes in V1.3.4 update
/// This is the zone that the inventory will be dragged and dropped on.
/// </summary>
public abstract class DecorationZone : MonoBehaviour {

	// what type of decorations can go on this node?
	public DecorationTypes nodeType;
	public DecorationTypes GetDecoType(){
		return nodeType;
	}

	public Image spriteIcon;
	public Image spriteOutline;
	public Image spriteFill;
	public Color neutralColorOutline;
	public Color neutralColorFill;
	public Color activeColorOutline;
	public Color activeColorFill;
	public Color inactiveColorOutline;
	public Color inactiveColorFill;

	public Animation activeHoverAnimation;

	private bool isAnimationPlaying = false;
	private string nodeID;
	private string placedDecoID = string.Empty;

	protected abstract void _RemoveDecoration();										// removes the decoration
	protected abstract void _SetDecoration(string decoID, bool isPlacedFromDecoMode);	// set the deco to this node

	void Start(){ 
		DecoInventoryUIManager.OnDecoDroppedOnTarget += OnDecorationDroppedInZone;
		DecoInventoryUIManager.OnDecoPickedUp += OnDecorationPickedUp;
		DecoInventoryUIManager.OnDecoDropped += OnDecorationDropped;

		DecoInventoryUIManager.Instance.OnManagerOpen += OnDecoMode;

		// Set the decoration icon
		switch(nodeType){
		case DecorationTypes.Carpet:
			spriteIcon.sprite = SpriteCacheManager.GetSprite("iconDecoCarpet2");
			break;
		case DecorationTypes.Poster:
			spriteIcon.sprite = SpriteCacheManager.GetSprite("iconDecoPoster2");
			break;
		case DecorationTypes.SmallPlant:
			spriteIcon.sprite = SpriteCacheManager.GetSprite("iconDecoSmallPlant2");
			break;
		case DecorationTypes.Wallpaper:
			spriteIcon.sprite = SpriteCacheManager.GetSprite("iconDecoWallpaper2");
			break;
		case DecorationTypes.BigFurniture:
			spriteIcon.sprite = SpriteCacheManager.GetSprite("iconDecoBigFurniture2");
			break;
		}

		// Turn on resizer and resize
		SpriteResizer resizer = spriteIcon.GetComponent<SpriteResizer>();
		resizer.enabled = true;	// Resize automatically

		nodeID = transform.parent.name;

		CheckSaveData();
	}

	void OnDestroy(){
		DecoInventoryUIManager.OnDecoPickedUp -= OnDecorationPickedUp;
		DecoInventoryUIManager.OnDecoDropped -= OnDecorationDropped;
		DecoInventoryUIManager.OnDecoDroppedOnTarget -= OnDecorationDroppedInZone;
		if(DecoInventoryUIManager.Instance){
			DecoInventoryUIManager.Instance.OnManagerOpen -= OnDecoMode;
		}
	}
	
	/// <summary>
	/// Checks save data to see if this node has any decoration saved there.  If it does, place that decoration
	/// </summary>
	private void CheckSaveData(){
		// If the saved data contains this node's id, it means there was a decoration placed here
		if(DataManager.Instance.GameData.Decorations.PlacedDecorations.ContainsKey(nodeID)){
			string savedDeco = DataManager.Instance.GameData.Decorations.PlacedDecorations[nodeID];
			SetDecoration(savedDeco, false);
		}
	}

	/// <summary>
	/// Gets the decoration item from inventory.
	/// </summary>
	/// <returns>The decoration item from inventory.</returns>
	/// <param name="itemID">Item ID</param>
	private DecorationItem GetDecorationItemFromInventory(string itemID){
		InventoryItem item = InventoryManager.Instance.GetDecoInInventory(itemID);
		if(item == null){
			return null;
		}
		else{
			return (DecorationItem)item.ItemData;
		}
	}

	/// <summary>
	/// Raises the dropped in zone event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnDecorationDroppedInZone(object sender, InventoryDragDrop.InvDragDropArgs args){
		// Weed out everything but the target node due to event call
		if(args.TargetCollider.gameObject == this.gameObject){
			// Double check the item
			DecorationItem decoItem = GetDecorationItemFromInventory(args.ItemTransform.gameObject.name);
			if(args.TargetCollider.GetComponent<DecorationZone>().nodeType == decoItem.DecorationType){
				args.IsValidTarget = true;
				SetDecoration(args.ItemTransform.gameObject.name, true);	// TODO refactor this item Name???
			}
		}
	}

	public void SetDecoration(string itemID, bool isPlacedFromDecoMode){
		// Do one last check
		if(!CanPlaceDecoration(itemID)){
			Debug.LogError("Illegal deco placement for " + itemID + " on node " + gameObject);
			return;
		}

		// If there was already a decoration here, remove it
		if(HasDecoration()){
			RemoveDecoration();
		}

		placedDecoID = itemID;

		// update the save data with the new decoration id
		DataManager.Instance.GameData.Decorations.PlacedDecorations[nodeID] = itemID;		
		
		// Notify inventory logic that this item is being used
		InventoryManager.Instance.UsePetItem(itemID);

		_SetDecoration(itemID, isPlacedFromDecoMode);

		// Play a sound
		DecorationItem itemDeco = (DecorationItem)DataLoaderItems.GetItem(itemID);
		if(isPlacedFromDecoMode){
			if(itemDeco.DecorationType == DecorationTypes.Poster || itemDeco.DecorationType == DecorationTypes.Wallpaper){
				AudioManager.Instance.PlayClip("decoPlacePaper");
			}
			else{
				AudioManager.Instance.PlayClip("decoPlaceFurniture");
			}

			// play an FX
			Vector3 particlePos = transform.position;
			string particlePrefabName = Constants.GetConstant<string>("Deco_PlaceParticle");
			ParticleUtils.CreateParticle(particlePrefabName, particlePos);	
		}

		//Check for badge unlock
		int totalNumOfDecorations = DataManager.Instance.GameData.Decorations.PlacedDecorations.Count;
		BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.Decoration, totalNumOfDecorations, true);
	}
	
	/// <summary>
	/// Checks to see if the decoration with strID may be placed on this node.
	/// </summary>
	/// <param name="strID">String ID</param>
	private bool CanPlaceDecoration(string itemID){
		bool isPlaceable = true;	// Start optimistic
		
		// Compare the node type to the decoration type
		DecorationItem itemDeco = (DecorationItem)DataLoaderItems.GetItem(itemID);
		DecorationTypes nodeType = GetDecoType();
		DecorationTypes decoType = itemDeco.DecorationType;
		isPlaceable = (nodeType == decoType);
		return isPlaceable;
	}
	
	/// <summary>
	/// Does this node currently have a decoration on it?
	/// </summary>
	/// <returns><c>true</c> if this node has decoration; otherwise, <c>false</c>.</returns>
	public virtual bool HasDecoration(){
		return placedDecoID != string.Empty;
	}
	
	/// <summary>
	/// Removes the decoration from this node.
	/// Assuming that the user has inventory placed
	/// </summary>
	public void RemoveDecoration(){
		// call child function to actually remove the decoration
		_RemoveDecoration();
		
		// give the user the decoration back in their inventory
		if(placedDecoID != null)
			InventoryManager.Instance.AddItemToInventory(placedDecoID);
		else
			Debug.LogError("Just removed an illegal decoration?");		
		
		// update the save data since this node is now empty
		DataManager.Instance.GameData.Decorations.PlacedDecorations.Remove(placedDecoID);
		
		// reset the deco id on this node
		placedDecoID = string.Empty;
	}

	//////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Raises the decoration picked up event.
	/// </summary>
	private void OnDecorationPickedUp(object sender, EventArgs args){
		DecorationItem decoItem = GetDecorationItemFromInventory(((GameObject)sender).name);
		SetState(decoItem.DecorationType.ToString());
	}

	/// <summary>
	/// Raises the decoration dropped event.
	/// This applies to all nodes, just rever the node back to normal
	/// </summary>
	private void OnDecorationDropped(object sender, EventArgs args){
		SetState(null);
	}

	/// <summary>
	/// Sets the state of the node.
	/// NOTE: Only for object picked up! hovering will be done with raycasting
	/// </summary>
	/// <param name="typeName">Type name.</param>
	private void SetState(string nodeTypeName){
		// Neutral state, play idle state
		if(nodeTypeName == null){
			isAnimationPlaying = false;
			if(!isAnimationPlaying){
				GetComponent<Animation>().Stop();
			}
			
			spriteOutline.color = neutralColorOutline;
			spriteFill.color = neutralColorFill;
			GameObjectUtils.ResetLocalScale(gameObject);
		}
		// Wrong state, play the inactive state
		else if(nodeTypeName != nodeType.ToString()){
			isAnimationPlaying = false;
			if(!isAnimationPlaying){
				GetComponent<Animation>().Stop();
			}

			//spriteOutline.color = inactiveColorOutline;
			//spriteFill.color = inactiveColorFill;
		}
		// Correct state, play the active state
		else{
			GetComponent<Animation>().Play();
			isAnimationPlaying = true;

			spriteOutline.color = activeColorOutline;
			spriteFill.color = activeColorFill;
		}
	}

	// Event listener. listening to when decoration mode is enabled/disabled
	private void OnDecoMode(object sender, UIManagerEventArgs e){
		TweenToggle toggle = GetComponent<ScaleTweenToggle>();
		if(e.Opening){
			toggle.Show();		// edit mode is opening, so turn this node on
		}
		else{
			toggle.Hide();		// edit mode is closing so turn this node off
		}
	}
}
