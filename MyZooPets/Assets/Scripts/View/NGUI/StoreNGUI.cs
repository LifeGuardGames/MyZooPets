using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StoreNGUI : MonoBehaviour {
	
	public GameObject ItemPrefab;
	public UIAtlas BackGroundRed;
	public UIAtlas BackGroundGreen;
	public UIAtlas BackGroundOrange;
	public UIAtlas BackGroundPurple;
	
	private ItemLogic itemlogic;
	private bool changePage;
	private int page;
	private UISprite uisprite;
	
	// Use this for initialization
	void Start () {
		//todo Change to GameManager later
		itemlogic = GameObject.Find("Grid").GetComponent<ItemLogic>();
		uisprite = GameObject.Find("BuyingAreaBackground").GetComponent<UISprite>();
		createItems(null);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnBuyButton(GameObject button){
		itemlogic.OnCall(Convert.ToInt32(button.transform.parent.name));
	}
	
	void createItems(GameObject page){
		
		//When clicked, first destory every item
		GameObject grid = GameObject.Find("Grid");
		foreach(Transform child in grid.transform){
			Destroy(child.gameObject);
		}
		
		if(page == null || page.name == "Food"){
			uisprite.atlas = BackGroundRed;
			for(int i = 0;i<itemlogic.foodlist.Count;i++){
				GameObject item = NGUITools.AddChild(GameObject.Find("Grid"),ItemPrefab);
				item.transform.localScale = new Vector3(1.2f,1.3f,1);
				item.name = itemlogic.foodlist[i].ToString();
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelBlue";
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.items[itemlogic.foodlist[i]].name;
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.items[itemlogic.foodlist[i]].name;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = GameObject.Find("StoreGUI");
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
				//Todo add Background and Description
			}
		}
		else if(page.name == "Item"){
			uisprite.atlas = BackGroundGreen;
			for(int i = 0;i<itemlogic.itemlist.Count;i++){
				GameObject item = NGUITools.AddChild(GameObject.Find("Grid"),ItemPrefab);
				item.transform.localScale = new Vector3(1.2f,1.3f,1);
				item.name = itemlogic.itemlist[i].ToString();
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelRed";
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.items[itemlogic.itemlist[i]].name;
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.items[itemlogic.itemlist[i]].name;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = GameObject.Find("StoreGUI");
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
				//Todo add Background and Description
			}
		}
		else if(page.name == "Decoration"){
			uisprite.atlas = BackGroundPurple;
			for(int i = 0;i<itemlogic.decolist.Count;i++){
				GameObject item = NGUITools.AddChild(GameObject.Find("Grid"),ItemPrefab);
				item.transform.localScale = new Vector3(1.2f,1.3f,1);
				item.name = itemlogic.decolist[i].ToString();
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelPurple";
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.items[itemlogic.decolist[i]].name;
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.items[itemlogic.decolist[i]].name;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = GameObject.Find("StoreGUI");
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
				//Todo add Background and Description
			}
		}
		else if(page.name == "Inhaler"){
			uisprite.atlas = BackGroundGreen;
			for(int i = 0;i<itemlogic.inhalerlist.Count;i++){
				GameObject item = NGUITools.AddChild(GameObject.Find("Grid"),ItemPrefab);
				item.transform.localScale = new Vector3(1.2f,1.3f,1);
				item.name = itemlogic.inhalerlist[i].ToString();
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelYellow";
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.items[itemlogic.inhalerlist[i]].name;
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.items[itemlogic.inhalerlist[i]].name;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = GameObject.Find("StoreGUI");
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
				//Todo add Background and Description
			}
		}
		grid.GetComponent<UIGrid>().Reposition();
		Invoke("reposition",0.00000001f);
	}
	
	void reposition(){
		GameObject.Find("Grid").GetComponent<UIGrid>().Reposition();
		
	}
}
