using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Decoration zone.
/// This is the replacement for decoration nodes in V1.3.4 update
/// This is the zone that the inventory will be dragged and dropped on.
/// </summary>

public enum DecorationZoneState{
	Neutral, Inactive, Active, Hover
}

public abstract class DecorationZone : MonoBehaviour {

	// what type of decorations can go on this node?
	public DecorationTypes nodeType;
	public DecorationTypes GetDecoType(){
		return nodeType;
	}

	public UISprite spriteIcon;
	public UISprite spriteOutline;
	public UISprite spriteFill;
	public Color neutralColorOutline;
	public Color neutralColorFill;
	public Color activeColorOutline;
	public Color activeColorFill;
	public Color inactiveColorOutline;
	public Color inactiveColorFill;

	public Animation activeHoverAnimation;

	private bool isHovered = false;
	private bool isHoverPlaying = false;
	private DecorationZoneState state;
	private string nodeID;
	private string placedDecoID = null;

	protected abstract void _RemoveDecoration();						// removes the decoration
	protected abstract void _SetDecoration(string strID);				// set the deco to this node

	void Start(){
		DecorationUIManager.Instance.OnDecoPickedUp += OnDecorationPickedUp;
		DecorationUIManager.Instance.OnDecoDropped += OnDecorationDropped;

		// Set the decoration icon
		switch(nodeType){
		case DecorationTypes.Carpet:
			spriteIcon.spriteName = "iconDecoCarpet";
			break;
		case DecorationTypes.Poster:
			spriteIcon.spriteName = "iconDecoPoster";
			break;
		case DecorationTypes.SmallPlant:
			spriteIcon.spriteName = "iconDecoSmallPlant";
			break;
		case DecorationTypes.Wallpaper:
			spriteIcon.spriteName = "iconDecoWallpaper";
			break;
		case DecorationTypes.BigFurniture:
			spriteIcon.spriteName = "iconDecoBigFurniture";
			break;
		}

		nodeID = transform.parent.name;

		CheckSaveData();
	}

	void OnDestroy(){
		DecorationUIManager.Instance.OnDecoPickedUp -= OnDecorationPickedUp;
		DecorationUIManager.Instance.OnDecoDropped -= OnDecorationDropped;
	}
	
	/// <summary>
	/// Checks save data to see if this node has any decoration saved there.  If it does, place that decoration
	/// </summary>
	private void CheckSaveData(){
		// If the saved data contains this node's id, it means there was a decoration placed here
		if(DataManager.Instance.GameData.Decorations.PlacedDecorations.ContainsKey(nodeID)){
			string savedDeco = DataManager.Instance.GameData.Decorations.PlacedDecorations[nodeID];
			SetDecoration(savedDeco);
		}
	}

	void SetDecoration(string itemID){
		// do one last check
		if(!CanPlaceDecoration(itemID)){
			Debug.LogError("Illegal deco placement for " + itemID + " on node " + gameObject);
			return;
		}

		// if there was already a decoration here, remove it
		if(HasDecoration())
			RemoveDecoration();	

		placedDecoID = itemID;

		// update the save data with the new decoration id
		DataManager.Instance.GameData.Decorations.PlacedDecorations[nodeID] = itemID;		
		int totalNumOfDecorations = DataManager.Instance.GameData.Decorations.PlacedDecorations.Count;
		
		//Check for badge unlock
		BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Decoration, totalNumOfDecorations, true);

		_SetDecoration(itemID);
	}
	
	/// <summary>
	/// Checks to see if the decoration with strID may be placed on this node.
	/// </summary>
	/// <param name="strID">String ID</param>
	private bool CanPlaceDecoration(string itemID){
		bool isPlaceable = true;	// Start optimistic
		
		// Compare the node type to the decoration type
		DecorationItem itemDeco = (DecorationItem)ItemLogic.Instance.GetItem(itemID);
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
	/// </summary>
	public void RemoveDecoration(){		
		// call child function to actually remove the decoration
		_RemoveDecoration();
		
		// give the user the decoration back in their inventory
		if(placedDecoID != null)
			InventoryLogic.Instance.AddItem(placedDecoID, 1);
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
		SetState(DecorationUIManager.Instance.currentDeco.DecorationType.ToString());
	}

	/// <summary>
	/// Raises the decoration dropped event.
	/// </summary>
	private void OnDecorationDropped(object sender, EventArgs args){
		SetState(null);
	}

	/// <summary>
	/// Raycasting call from drag object
	/// </summary>
	public void CheckHover(GameObject sender){
		SetState(DecorationUIManager.Instance.currentDeco.DecorationType.ToString(), isHovered:true);
	}

	/// <summary>
	/// Sets the state of the node.
	/// NOTE: Only for object picked up! hovering will be done with raycasting
	/// </summary>
	/// <param name="typeName">Type name.</param>
	private void SetState(string nodeTypeName, bool isHovered = false){
		// Neutral state, play idle state
		if(nodeTypeName == null){
			state = DecorationZoneState.Neutral;

			isHoverPlaying = false;
			if(!isHoverPlaying){
				animation.Stop();
			}

			spriteOutline.color = neutralColorOutline;
			spriteFill.color = neutralColorFill;
		}
		// Wrong state, play the inactive state
		else if(nodeTypeName != nodeType.ToString()){
			state = DecorationZoneState.Inactive;

			isHoverPlaying = false;
			if(!isHoverPlaying){
				animation.Stop();
			}

			spriteOutline.color = inactiveColorOutline;
			spriteFill.color = inactiveColorFill;
		}
		// Correct state, is hovered with right object
		else if(state == DecorationZoneState.Active && isHovered){
			state = DecorationZoneState.Hover;

			if(!isHoverPlaying){
				isHoverPlaying = true;
				activeHoverAnimation.Play();
			}
		}
		// Correct state, play the active state
		else{
			state = DecorationZoneState.Active;

			isHoverPlaying = false;
			if(!isHoverPlaying){
				animation.Stop();
			}

			spriteOutline.color = activeColorOutline;
			spriteFill.color = activeColorFill;
		}
	}
}
