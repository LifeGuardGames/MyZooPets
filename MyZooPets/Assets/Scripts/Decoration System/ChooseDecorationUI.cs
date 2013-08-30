using UnityEngine;
using System.Collections;

public class ChooseDecorationUI : MonoBehaviour {
	
	// the grid this UI places its items in
	public GameObject goGrid;
	
	// prefab that items in this UI are instantiated from
	public GameObject prefabChooseDecoEntry;

	public void UpdateItems( DecorationTypes eType ) {
		//Destory all items in the list first
		foreach(Transform child in goGrid.transform){
			Destroy(child.gameObject);
		}		
		
		for ( int i = 0; i < 7; i++ ) {
			GameObject item = NGUITools.AddChild(goGrid, prefabChooseDecoEntry);
			item.name = "item_" + i;
			item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = "This is deco item " + i;
			item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = "100";
			item.transform.FindChild("ItemName").GetComponent<UILabel>().text = "Deco #" + i;
			item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = "apple";
			item.transform.FindChild("PlaceButton").GetComponent<UIButtonMessage>().target = gameObject;
			item.transform.FindChild("PlaceButton").GetComponent<UIButtonMessage>().functionName = "OnPlaceButton";
		}	
		
		goGrid.GetComponent<UIGrid>().Reposition();
		Invoke("Reposition",0.00000001f);		
	}
	
	//Delay calling reposition due to async problem Destroying/Repositionoing.
	//TODO Maybe change later when we have moreItems 
	private void Reposition(){
		goGrid.GetComponent<UIGrid>().Reposition();

	}	
	
	//Called when "Place" is clicked
	public void OnPlaceButton(GameObject button){
		Debug.Log("Time to place this decoration!!!");
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
}
