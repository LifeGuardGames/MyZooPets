using UnityEngine;
using UnityEngine.UI;
using System;

// This is the zone that the inventory will be dragged and dropped on.
public abstract class DecorationZone : MonoBehaviour, IDropInventoryTarget {
	// what type of decorations can go on this node?
	public DecorationTypes nodeType;
	public DecorationTypes GetDecoType() {
		return nodeType;
	}

	public Image spriteIcon;
	public Image spriteFill;
	public Color neutralColorFill;
	public Color activeColorFill;
	public Color inactiveColorFill;

	public Animation activeHoverAnimation;

	private bool isAnimationPlaying = false;
	private string nodeID;
	protected string placedDecoID = string.Empty;

	protected abstract void _RemoveDecoration();                                        // removes the decoration
	protected abstract void _SetDecoration(string decoID, bool isPlacedFromDecoMode);   // set the deco to this node

	void Start() {
		DecoModeUIManager.Instance.OnManagerOpen += ShowDecoZones;
		InventoryTokenDragElement.OnDecoItemPickedUp += OnDecorationPickedUp;
		InventoryTokenDragElement.OnDecoItemDropped += OnDecorationDropped;

        spriteIcon.sprite = SpriteCacheManager.GetDecoIconSprite(nodeType);
		nodeID = transform.parent.name;
		CheckSaveData();
	}

	void OnDestroy() {
		if(DecoModeUIManager.Instance) {
			DecoModeUIManager.Instance.OnManagerOpen -= ShowDecoZones;
		}
		InventoryTokenDragElement.OnDecoItemPickedUp -= OnDecorationPickedUp;
		InventoryTokenDragElement.OnDecoItemDropped -= OnDecorationDropped;
	}

	/// <summary>
	/// Checks save data to see if this node has any decoration saved there.  If it does, place that decoration
	/// </summary>
	private void CheckSaveData() {
		// If the saved data contains this node's id, it means there was a decoration placed here
		if(DataManager.Instance.GameData.Decorations.PlacedDecorations.ContainsKey(nodeID)) {
			string savedDeco = DataManager.Instance.GameData.Decorations.PlacedDecorations[nodeID];
			SetDecoration(savedDeco, false);
		}
	}

	/// <summary>
	/// Gets the decoration item from inventory.
	/// </summary>
	/// <returns>The decoration item from inventory.</returns>
	/// <param name="itemID">Item ID</param>
	private DecorationItem GetDecorationItemFromInventory(string itemID) {
		InventoryItem item = InventoryManager.Instance.GetDecoInInventory(itemID);
		if(item == null) {
			return null;
		}
		else {
			return (DecorationItem)item.ItemData;
		}
	}

	// Implementation for IDropInventoryTarget
	public void OnItemDropped(InventoryItem item) {
		if(CanPlaceDecoration(item.ItemID)) {
			SetDecoration(item.ItemID, true);
		}
	}

	public void SetDecoration(string itemID, bool isPlacedFromDecoMode) {
		// Do one last check
		if(!CanPlaceDecoration(itemID)) {
			Debug.LogError("Illegal deco placement for " + itemID + " on node " + gameObject);
			return;
		}
		
		if(nodeType == DecorationTypes.Wallpaper) {	// Wallpapers always needs to be removed
			RemoveDecoration();
		}
		else if(HasDecoration()) {  // If there was already a decoration here, remove it
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
		if(isPlacedFromDecoMode) {
			if(itemDeco.DecorationType == DecorationTypes.Poster || itemDeco.DecorationType == DecorationTypes.Wallpaper) {
				AudioManager.Instance.PlayClip("decoPlacePaper");
			}
			else {
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

	private bool CanPlaceDecoration(string itemID) {
		bool isPlaceable = true;    // Start optimistic

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
	public virtual bool HasDecoration() {
		return placedDecoID != string.Empty;
	}

	/// <summary>
	/// Removes the decoration from this node.
	/// Assuming that the user has inventory placed
	/// </summary>
	public void RemoveDecoration() {
		// call child function to actually remove the decoration
		_RemoveDecoration();

		if(nodeType != DecorationTypes.Wallpaper) {
			// give the user the decoration back in their inventory
			if(placedDecoID != null) {
				InventoryManager.Instance.AddItemToInventory(placedDecoID);
			}
			else {
				Debug.LogError("Just removed an illegal decoration?");
			}

			// update the save data since this node is now empty
			DataManager.Instance.GameData.Decorations.PlacedDecorations.Remove(placedDecoID);
		}

		// reset the deco id on this node
		placedDecoID = string.Empty;
	}

	//////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Raises the decoration picked up event.
	/// </summary>
	private void OnDecorationPickedUp(object sender, EventArgs args) {
		DecorationItem decoItem = (DecorationItem)InventoryTokenDragElement.itemBeingDragged.GetComponent<InventoryTokenDragElement>().InventoryData.ItemData;
		SetState(decoItem.DecorationType.ToString());
	}

	/// <summary>
	/// Raises the decoration dropped event.
	/// This applies to all nodes, just revert the node back to normal
	/// </summary>
	private void OnDecorationDropped(object sender, EventArgs args) {
		SetState(null);
	}

	/// <summary>
	/// Sets the state of the node.
	/// NOTE: Only for object picked up! hovering will be done with raycasting
	/// </summary>
	/// <param name="typeName">Type name.</param>
	private void SetState(string nodeTypeName) {
		if(nodeTypeName == null) {							// Neutral state, play idle state
			isAnimationPlaying = false;
			if(!isAnimationPlaying) {
				GetComponent<Animation>().Stop();
			}
			spriteFill.color = neutralColorFill;
			GameObjectUtils.ResetLocalScale(gameObject);
		}
		else if(nodeTypeName != nodeType.ToString()) {      // Wrong state, play the inactive state
			isAnimationPlaying = false;
			if(!isAnimationPlaying) {
				GetComponent<Animation>().Stop();
			}
			spriteFill.color = inactiveColorFill;
		}
		else {                                              // Correct state, play the active state
			GetComponent<Animation>().Play();
			isAnimationPlaying = true;
			spriteFill.color = activeColorFill;
		}
	}

	// Event listener. listening to when decoration mode is enabled/disabled
	private void ShowDecoZones(object sender, UIManagerEventArgs args) {
		TweenToggle toggle = GetComponent<ScaleTweenToggle>();
		if(args.Opening) {
			toggle.Show();      // edit mode is opening, so turn this node on
		}
		else {
			toggle.Hide();      // edit mode is closing so turn this node off
		}
	}
}
