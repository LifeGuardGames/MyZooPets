using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ChooseDecorationUI
// This script is attached to the UI that appears
// when a decoration node is clicked.  It is a UI
// that lets the user set and remove decorations.
//---------------------------------------------------

public class ChooseDecorationUI : MonoBehaviour {
	
	// prefab that items in this UI are instantiated from
	public GameObject prefabChooseDecoEntry;
	
	// prefab of the area that items are populated to -- this exists because if we don't instantiate it, the list "remembers" where it was last scrolled to
	public GameObject prefabChooseArea;
	private GameObject goChooseArea;
	
	// the grid this UI places its items in
	private GameObject goGrid;	
	
	// the decoration node that this UI is currently representing
	private DecorationNode decoNodeCurrent;
	
	//---------------------------------------------------
	// UpdateItems()
	// This function updates the choose decoration menu
	// with the appropriate decorations for the incoming
	// node, decoNode.
	//---------------------------------------------------	
	public void UpdateItems( DecorationNode decoNode ) {
		// set our current deco node
		decoNodeCurrent = decoNode;
		
		// get the type of decorations to create the list for
		DecorationTypes eType = decoNode.GetDecoType();
		
		// instantiate the item area
		if ( goChooseArea )
			Destroy( goChooseArea );	// destroy the section of the UI with all the entries if it existed already
		goChooseArea = NGUITools.AddChildWithPosition( gameObject, prefabChooseArea );
		goGrid = goChooseArea.transform.Find("Grid").gameObject;
		
		// Destory all items in the list first (these may exist from the prefab)
		foreach(Transform child in goGrid.transform){
			Destroy(child.gameObject);
		}		
		
		// we also have to reset the position of the ItemArea or else the list will "remember" where it was scrolled to
		//Debug.Log("Resetting pos from " + goItemArea.transform.position + " to " + vItemArea);
		//goItemArea.transform.position = vItemArea;
		
		// get the ordered list of decorations to be displayed
		DecorationTypes[] arrayDecos = GetDecorationList( eType );
		
		for ( int i = 0; i < 7; i++ ) {
			bool bDecoOK = arrayDecos[i] == eType;
			string strDesc = bDecoOK ? "Deco item " + i + "(" + arrayDecos[i] + ")" : "Cannot place!";
			
			GameObject item = NGUITools.AddChild(goGrid, prefabChooseDecoEntry);
			item.name = "item_" + i;
			item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = strDesc;
			item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = "100";
			item.transform.FindChild("ItemName").GetComponent<UILabel>().text = "Deco #" + i;
			item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = "apple";
			
			// depending on if the deco can be placed or not, set certain attributes on the entry
			UIButtonMessage button = item.transform.FindChild("PlaceButton").GetComponent<UIButtonMessage>();
			if ( bDecoOK ) {
				
				// set up place button callbacks
				button.target = gameObject;
				button.functionName = "OnPlaceButton";
			}
			else {
				// destroy the place button
				Destroy(button.gameObject);
				
				// color the box bg appropriately
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().color = new Color32(201,201,201,255);
			}
		}	
		
		// the last item in the list (so it shows up first) is the removal option (if there is a decoration at this node)
		if ( decoNodeCurrent.HasDecoration() ) {
			GameObject itemRemove = NGUITools.AddChild(goGrid, prefabChooseDecoEntry);
			itemRemove.name = "item_remove";
			itemRemove.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = "Remove this decoration.";
			itemRemove.transform.FindChild("ItemCost").GetComponent<UILabel>().text = "100";
			itemRemove.transform.FindChild("ItemName").GetComponent<UILabel>().text = "Remove";
			itemRemove.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = "apple";
			itemRemove.transform.FindChild("PlaceButton").GetComponent<UIButtonMessage>().target = gameObject;
			itemRemove.transform.FindChild("PlaceButton").GetComponent<UIButtonMessage>().functionName = "OnRemoveButton";	
		}
		
		goGrid.GetComponent<UIGrid>().Reposition();
		Invoke("Reposition",0.00000001f);		
	}
	
	//---------------------------------------------------
	// GetDecorationList()
	// Returns the ordered decoration list to be displayed
	// for eType.  Note that the list is in reverse order;
	// the elements at the end will be displayed first.
	//---------------------------------------------------	
	private DecorationTypes[] GetDecorationList( DecorationTypes eType ) {
		DecorationTypes[] listDecos = new DecorationTypes[7];
		if ( eType == DecorationTypes.Floor ) {
			listDecos[0] = DecorationTypes.Wall;
			listDecos[1] = DecorationTypes.Wall;
			listDecos[2] = DecorationTypes.Wall;
			listDecos[3] = DecorationTypes.Wall;
			listDecos[4] = DecorationTypes.Floor;
			listDecos[5] = DecorationTypes.Floor;
			listDecos[6] = DecorationTypes.Floor;			
		}
		else {
			listDecos[0] = DecorationTypes.Floor;
			listDecos[1] = DecorationTypes.Floor;
			listDecos[2] = DecorationTypes.Floor;
			listDecos[3] = DecorationTypes.Wall;
			listDecos[4] = DecorationTypes.Wall;
			listDecos[5] = DecorationTypes.Wall;
			listDecos[6] = DecorationTypes.Wall;			
		}
		
		return listDecos;
	}
	
	//Delay calling reposition due to async problem Destroying/Repositioning
	//TODO Maybe change later when we have moreItems 
	private void Reposition(){
		goGrid.GetComponent<UIGrid>().Reposition();

	}	
	
	//---------------------------------------------------
	// OnPlaceButton()
	// Callback function for when the user clicks the
	// place button for putting a decoration into the
	// scene.
	//---------------------------------------------------
	public void OnPlaceButton( GameObject button ) {
		Debug.Log("Time to place this decoration!!!");
		
		// place the deco
		DecorationTypes eType = decoNodeCurrent.GetDecoType();
		string strResource = eType == DecorationTypes.Floor ? "GO_Deco_Couch" : "GO_Deco_Painting";
		GameObject goTest = Resources.Load(strResource) as GameObject;
		Vector3 vPos = decoNodeCurrent.transform.position;
		GameObject goDeco = Instantiate(goTest, vPos, goTest.transform.rotation) as GameObject;
		
		// set the deco on the node
		decoNodeCurrent.SetDecoration( goDeco );
		
		// close this menu
		EditDecosUIManager.Instance.CloseChooseMenu();
		/*
		int cost = int.Parse(button.transform.parent.FindChild("ItemCost").GetComponent<UILabel>().text);
		int itemId = int.Parse(button.transform.parent.name);
		if(DataManager.Instance.Stats.Stars >= cost){
			inventory.AddItem(itemId, 1);
			StatsController.Instance.ChangeStats(0, Vector3.zero, cost * -1, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);	// Convert to negative
			OnBuyAnimation(button.transform.parent.FindChild("ItemTexture").gameObject);
		}
		*/
	}
	
	//---------------------------------------------------
	// OnRemoveButton()
	// Callback function for when the user presses the
	// remove button to remove a decoration from the
	// scene.
	//---------------------------------------------------
	public void OnRemoveButton( GameObject button ) {
		Debug.Log("Time to remove this decoration!");
		
		// remove the deco from the node
		decoNodeCurrent.RemoveDecoration();
		
		// close this menu
		EditDecosUIManager.Instance.CloseChooseMenu();		
	}
}
