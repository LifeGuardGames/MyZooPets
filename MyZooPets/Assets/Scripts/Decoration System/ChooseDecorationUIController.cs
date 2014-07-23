using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//---------------------------------------------------
// ChooseDecorationUI
// This script is attached to the UI that appears
// when a decoration node is clicked.  It is a UI
// that lets the user set and remove decorations.
//---------------------------------------------------

public class ChooseDecorationUIController : MonoBehaviour{
	//=======================Events========================
	public EventHandler<EventArgs> OnDecoPlaced;   // when a decoration is placed	

	public GameObject chooseDecoEntryPrefab; // prefab that items in this UI are instantiated from
	public string soundPlace; // sound that gets played when a decoration is placed
	public GameObject removeButtonGO; // the remove button game object
	public UIPanel gridPanel; // Panel of the grid
	public GameObject grid; // the grid this UI places its items in
	public UILabel statusLabel; // Show the status of available items, else tell user to go to store
	public AnimationControl shopAnimControl; // Animation for shop button if there is no deco

	private DecorationNode decoNodeCurrent; // the decoration node that this UI is currently representing
	private GameObject goTutorialEntry; // save the 0th item in the deco menu for using in tutorials

	public DecorationNode GetNode(){
		return decoNodeCurrent;	
	}

	public GameObject GetTutorialEntry(){
		return goTutorialEntry;	
	}

	void Start(){
		// Reposition all the things nicely to stretch to the end of the screen
		Vector4 oldRange = gridPanel.clipRange;
		
		// The 52 comes from some wierd scaling issue.. not sure what it is but compensate now
		gridPanel.transform.localPosition = new Vector3(0f, gridPanel.transform.localPosition.y, 0f);
		gridPanel.clipRange = new Vector4(0f, oldRange.y, (float)(CameraManager.Instance.GetNativeWidth()), oldRange.w);
		
		// Position the grid origin to the left of the screen
		Vector3 gridPosition = grid.transform.localPosition;
		grid.transform.localPosition = new Vector3(
			(-1f * (CameraManager.Instance.GetNativeWidth() / 2)) - gridPanel.transform.localPosition.x + 10,
			gridPosition.y, gridPosition.z);	
	}
	
	void Update(){
		// TODO-s THIS IS A MISBEHAVING VARIABLE PUT A DEBUG TO PRINT WHEN REFACTORING TO CATCH THIS!!
		if(gridPanel.clipRange.y != 0){
			//Debug.Log("CAUGHT YOU!");
			gridPanel.clipRange = new Vector4(gridPanel.clipRange.x, 0, gridPanel.clipRange.z, gridPanel.clipRange.w);
		}
	}

	/// <summary>
	/// Updates the items.
	/// Updates the choose decoration menu with the appropriate decorations for the 
	/// incoming node, decoNode.
	/// </summary>
	/// <param name="decoNode">Deco node.</param>
	public void UpdateItems(DecorationNode decoNode){
		// set our current deco node
		decoNodeCurrent = decoNode;
		
		// Destroy all child within grid
		// NOTE: cant enumerate (skipping), destroying backwards!
		int childs = grid.transform.childCount;
		for(int i = childs - 1; i > 0; i--){
			Destroy(grid.transform.GetChild(i).gameObject);
		}
		
		// create the decoration entries in the UI
		CreateEntries(grid);
		
		// show or hide the remove button as appropriate
		bool bShowRemove = decoNodeCurrent.HasRemoveOption();
		NGUITools.SetActive(removeButtonGO, bShowRemove);
		
		grid.GetComponent<UIGrid>().Reposition();
		Invoke("Reposition", 0.00000001f);
		
		ResetUIPanelClipRange();
	}

	/// <summary>
	/// Creates each individual UI entry for all decorations that are in this UI.
	/// </summary>
	/// <param name="goGrid">Go grid.</param>
	private void CreateEntries(GameObject grid){
		// Keep track if there is something we can play, changes status text
		bool isDecoItemsAvailable = false;
		
		// Destory all items in the list first (these may exist from the prefab)
		foreach(Transform child in grid.transform)
			Destroy(child.gameObject);
		
		// get the type of decorations to create the list for
		DecorationTypes eType = decoNodeCurrent.GetDecoType();
		
		// get the ordered list of decorations to be displayed
		List<InventoryItem> listDecos = InventoryLogic.Instance.GetDecorationInventoryItemsOrderyByType(eType);
		
		// loop through the list and create an entry for each decoration
		for(int i = 0; i < listDecos.Count; i++){
			InventoryItem inventoryDecoItem = listDecos[i];
			DecorationItem itemDeco = (DecorationItem)inventoryDecoItem.ItemData;
			
			bool isDecoTypeCorrect = itemDeco.DecorationType == eType;

			//create the deco item gameobject
			GameObject item = NGUITools.AddChild(grid, chooseDecoEntryPrefab);
			ChooseDecorationUIEntry decorationUIEntry = item.GetComponent<ChooseDecorationUIEntry>();
			decorationUIEntry.SetDecoID(itemDeco.ID);
			
			// sorting in a grid is a pain in the ass...trying to use _ for decos that are OK and an X for those that are not to sort it
			string strPrefix = isDecoTypeCorrect ? "_" : "X_";
			
			item.name = strPrefix + (listDecos.Count - i - 1) + "-" + itemDeco.ID;	// DO NOT CHANGE...this is what sorts it
			decorationUIEntry.itemName.text = itemDeco.Name;
			decorationUIEntry.itemTexture.spriteName = itemDeco.TextureName;
			decorationUIEntry.itemAmount.text = "x" + inventoryDecoItem.Amount.ToString();
		
			// depending on if the deco can be placed or not, set certain attributes on the entry
			LgButtonMessage button = decorationUIEntry.placeButtonGO.GetComponent<LgButtonMessage>();
			if(isDecoTypeCorrect){
				// set up place button callbacks
				button.target = gameObject;
				button.functionName = "OnPlaceButton";
				
				// Tell status bar there is something available
				isDecoItemsAvailable = true;

				decorationUIEntry.xMark.enabled = false;
			}
			else{
				// destroy the place button
				Destroy(button.gameObject);
				
				// color the box bg appropriately
				decorationUIEntry.itemBackground.color = new Color32(201, 201, 201, 255);

				// Show "X" over it
				decorationUIEntry.xMark.enabled = true;
			}
			
			// save the tutorial entry (a bit hacky)
			if(goTutorialEntry == null)
				goTutorialEntry = decorationUIEntry.placeButtonGO;
		}
		
		// Update the status label with formatted type name
		string formattedTypeKey = "DECO_TYPE_" + eType.ToString().ToUpper();
		// Choose which preformatted text to use
		statusLabel.text = isDecoItemsAvailable ?
			String.Format(Localization.Localize("DECO_CHOOSE_ITEM"), Localization.Localize(formattedTypeKey)) :
			String.Format(Localization.Localize("DECO_CHOOSE_NO_ITEM"), Localization.Localize(formattedTypeKey));

		// Bounch the shop button is there is nothing in your inventory
		if(isDecoItemsAvailable){
			shopAnimControl.Stop();
		}
		else{
			shopAnimControl.Play();
		}
	}
	
	//Delay calling reposition due to async problem Destroying/Repositioning
	//TODO Maybe change later when we have moreItems 
	private void Reposition(){
		grid.GetComponent<UIGrid>().Reposition();
	}
	
	//------------------------------------------
	// ResetUIPanelClipRange()
	// reset the clip range for the item area so that scrolling starts from the beginning
	//------------------------------------------
	private void ResetUIPanelClipRange(){
		Vector4 clipRange = gridPanel.GetComponent<UIPanel>().clipRange;
		
		// Stop the springing action when switching
		SpringPanel spring = gridPanel.GetComponent<SpringPanel>();
		if(spring != null){
			spring.enabled = false;	
		}
		
		// Reset the localposition and clipping position
		gridPanel.transform.localPosition = new Vector3(0f, gridPanel.transform.localPosition.y, gridPanel.transform.localPosition.z);
		gridPanel.gameObject.GetComponent<UIPanel>().clipRange = new Vector4(0f, clipRange.y, clipRange.z, clipRange.w);
	}

	/// <summary>
	/// Callback function for when the user clicks the
	/// place button for putting a decoration into the
	/// scene.
	/// </summary>
	/// <param name="button">Button.</param>
	public void OnPlaceButton(GameObject button){	
		// get the ID from the UI entry that was clicked
		ChooseDecorationUIEntry scriptEntry = button.transform.parent.gameObject.GetComponent<ChooseDecorationUIEntry>();
		string decoID = scriptEntry.GetDecoID();

		// set the deco on the node -- it does the instantiation of the 3d game object
		decoNodeCurrent.SetDecoration(decoID);
		
		// play a sound
		AudioManager.Instance.PlayClip(soundPlace);
		
		//notify inventory logic that this item is being used
		InventoryLogic.Instance.UsePetItem(decoID);		
		
		// close this menu
		EditDecosUIManager.Instance.CloseChooseMenu();
		
		// play an FX
		Vector3 vPosFX = decoNodeCurrent.gameObject.transform.position;
		string strFX = Constants.GetConstant<string>("Deco_PlaceFX");
		ParticleUtils.CreateParticle(strFX, vPosFX);		
		
		// send a callback
		if(OnDecoPlaced != null)
			OnDecoPlaced(this, EventArgs.Empty);		
		
		// send out a task completion event (probably just going to be used for tutorial...)
		WellapadMissionController.Instance.TaskCompleted("Decorate");
	}

	/// <summary>
	/// RCallback function for when the user presses the
	/// remove button to remove a decoration from the
	/// scene.
	/// </summary>
	/// <param name="button">Button.</param>
	public void OnRemoveButton(GameObject button){
		// remove the deco from the node
		decoNodeCurrent.RemoveDecoration();
		
		// give the user back their item
		// NOTE: This is now done in DecorationNode.RemoveDecoration()
		
		// close this menu
		EditDecosUIManager.Instance.CloseChooseMenu();		
	}
}
