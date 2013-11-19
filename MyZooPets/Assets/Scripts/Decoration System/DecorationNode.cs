using UnityEngine;
using System.Collections;

//---------------------------------------------------
// DecorationNode
// Script that resides on node objects that define
// the type of decoration node.
//---------------------------------------------------

public abstract class DecorationNode : LgButton {
	// ------ Pure Abstract --------------------------
	protected abstract void _SetDefaultDeco( string strDecoID );		// sets the default deco properly
	protected abstract void _RemoveDecoration();						// removes the decoration
	public abstract bool HasRemoveOption();								// can this deco be removed?
	//------------------------------------------------
	
	// what type of decorations can go on this node?
	public DecorationTypes eType;
	public DecorationTypes GetDecoType() {
		return eType;
	}
	
//	const string BUTTON_INACTIVE = "buildIcon";
//	const string BUTTON_ACTIVE = "buildIconActive";
//	public UISprite buttonImage;
	
	// the ID of this decoration node -- changing this will corrupt the save data
	// should be unique.  This will be set to the game object's name.
	private string strNodeID;
	
	// default decoration that is on this node (optional); note that this is a "soft" setting...we must make sure that this public
	// variable actually matches the decoration ID of whatever place in the scene
	public string strDefaultDecoID;
	
	// the decoration currently being displayed on this node
	private string strDecoID = string.Empty;

	protected override void _Start() {
		bCheckClickManager = false;

		// set the node ID to the game object name
		strNodeID = gameObject.name;
		
		// check save data to see if something was on this node
		CheckSaveData();
		
		// use event handler to listen for when the player goes into edit deco mode
		EditDecosUIManager.Instance.OnManagerOpen += OnDecoMode;
		
		// by default, decoration nodes are not visible/interactable
		ToggleNode( false );
		
		// set the node's label
		SetNodeLabel();
	}
	
	//---------------------------------------------------
	// SetNodeLabel()
	// Sets the node's label, based on what kind of node
	// it is.
	//---------------------------------------------------		
	private void SetNodeLabel() {
		DecorationTypes eType = GetDecoType();
		string strType = eType.ToString();
		string strKey = "NODE_" + strType.ToUpper();
		string strText = Localization.Localize( strKey );
		
		Transform transLabel = transform.Find( "Label" );
		UILabel label = transLabel.gameObject.GetComponent<UILabel>();
		label.text = strText;
	}

	//---------------------------------------------------
	// CheckSaveData()
	// Checks save data to see if this node has any
	// decoration saved there.  If it does, place that
	// decoration.
	//---------------------------------------------------	
	private void CheckSaveData() {
		// if the saved data contains this node's id, it means there was a decoration placed here
		if ( DataManager.Instance.GameData.Decorations.PlacedDecorations.ContainsKey( strNodeID ) ) {
			string strSavedDeco = DataManager.Instance.GameData.Decorations.PlacedDecorations[ strNodeID ];
			SetDecoration( strSavedDeco );
		}
		else {
			// there was no decoration placed here -- however, there could be a default decoration, in which case, we just want
			// to set it so that when it is replaced, the user gets that default decoration in their inventory
			if ( !string.IsNullOrEmpty( strDefaultDecoID ) )
				SetDefaultDeco( strDefaultDecoID );
		}
	}
	
	//---------------------------------------------------
	// SetDefaultDeco()
	// Setting the default decoration of a node is a little
	// complicated because there are different types of
	// nodes.
	//---------------------------------------------------	
	private void SetDefaultDeco( string strDecoID ) {
		// set the string ID of the decoration -- this will place the item in the user's inventory when it is replaced
		this.strDecoID = strDecoID;	
		
		// now call children function so that certain node can do additional stuff
		_SetDefaultDeco( strDecoID );
	}
	
    //Event listener. listening to when decoration mode is enabled/disabled
    private void OnDecoMode(object sender, UIManagerEventArgs e){
       if(e.Opening){
            ToggleNode( true );		// edit mode is opening, so turn this node on
        }else{
            ToggleNode( false );	// edit mode is closing so turn this node off
        }
    }
	
	//---------------------------------------------------
	// ToggleNode()
	// Makes the node visible or invisible. Checks for TweenToggleScale
	//---------------------------------------------------	
	private void ToggleNode( bool bOn ) {
		GetComponent<BoxCollider>().enabled = bOn;
		
		TweenToggle toggle = GetComponent<ScaleTweenToggle>();
		if(toggle != null){
			if(bOn){
				toggle.Show();
			}
			else{
				toggle.Hide();
			}
		}
	}
	
//	//---------------------------------------------------
//	// SetNodeImageActive()
//	// Called from editDecosUiManager to set color
//	//---------------------------------------------------
//	public void SetNodeImageActive(bool isActive){
//		buttonImage.spriteName = isActive ? BUTTON_ACTIVE : BUTTON_INACTIVE;
//	}
	
	//---------------------------------------------------
	// ProcessClick()
	// Called when this node is clicked.
	//---------------------------------------------------	
	protected override void ProcessClick() {
		// have the deco UI manager update itself based on this node being selected
		EditDecosUIManager.Instance.UpdateChooseMenu( this );
	}
	
	//---------------------------------------------------
	// GetDecorationID()
	// Returns this nodes decoration id.  May be null if
	// no decoration is set.
	//---------------------------------------------------	
	public string GetDecorationID() {
		return strDecoID;
	}
	
	//---------------------------------------------------
	// HasDecoration()
	// Does this node currently have a decoration on it?
	//---------------------------------------------------	
	public virtual bool HasDecoration() {
		return strDecoID != string.Empty;
	}	
	
	//---------------------------------------------------
	// SetDecoration()
	// Sets this node's decoration to the incoming
	// decoration.
	//---------------------------------------------------	
	public void SetDecoration( string strID ) {
		// do one last check
		if ( !CanPlaceDecoration( strID ) ) {
			Debug.Log("Illegal deco placement for " + strID + " on node " + gameObject);
			return;
		}
		
		// if there was already a decoration here, remove it
		if ( HasDecoration() )
			RemoveDecoration();		
		
		// cache the id
		strDecoID = strID;
		
		// update the save data with the new decoration id
		DataManager.Instance.GameData.Decorations.PlacedDecorations[strNodeID] = strID;		
		
		// actually create/set the decoration
		_SetDecoration( strDecoID );
	}
	
	protected virtual void _SetDecoration( string strID ) {
		// children implement this
	}
	
	//---------------------------------------------------
	// CanPlaceDecoration()
	// Checks to see if the decoration with strID may
	// be placed on this node.
	//---------------------------------------------------	
	private bool CanPlaceDecoration( string strID ) {
		bool bOK = true;	// start optimistic
		
		// compare the node type to the decoration type
		DecorationItem itemDeco = (DecorationItem) DataItems.GetItem( strID );
		DecorationTypes eNodeType = GetDecoType();
		DecorationTypes eDecoType = itemDeco.DecorationType;
		bOK = eNodeType == eDecoType;
		
		return bOK;
	}
	
	//---------------------------------------------------
	// RemoveDecoration()
	// Removes the decoration from this node.
	//---------------------------------------------------	
	public void RemoveDecoration() {		
		// call child function to actually remove the decoration
		_RemoveDecoration();
		
		// give the user the decoration back in their inventory
		if ( strDecoID != null )
			InventoryLogic.Instance.AddItem(strDecoID, 1);
		else
			Debug.Log("Just removed an illegal decoration?");		
		
		// update the save data since this node is now empty
		DataManager.Instance.GameData.Decorations.PlacedDecorations.Remove( strNodeID );
		
		// reset the deco id on this node
		strDecoID = string.Empty;
	}
}
