using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StoreUIManager : MonoBehaviour {
	
	public GameObject ItemPrefab;
	public UIAtlas BackGroundRed;
	public UIAtlas BackGroundGreen;
	public UIAtlas BackGroundOrange;
	public UIAtlas BackGroundPurple;

	//=========================Events====================
	public delegate void CallBack(object sender, EventArgs e);
	public static event CallBack OnStoreClosed; //call when store is closed
	//===================================================
	
	private ItemLogic itemlogic;
	private bool changePage;
	private int page;
	private UISprite uisprite;
	private GameObject grid;
	
	// Use this for initialization
	void Start () {
		itemlogic = GameObject.Find("GameManager/ItemLogic").GetComponent<ItemLogic>();
//		itemlogic = GameObject.Find("Grid").GetComponent<ItemLogic>();
		uisprite = GameObject.Find("BuyingAreaBackground").GetComponent<UISprite>();
		grid = GameObject.Find("Grid");
		CreateItems(null);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StoreClicked(){
		GetComponent<MoveTweenToggle>().Show();
	}

	public void StoreClosed(){
		GetComponent<MoveTweenToggle>().Hide();
		if(OnStoreClosed != null){
			OnStoreClosed(this, EventArgs.Empty);
		}else{
			Debug.LogError("OnStoreClosed listener is null");
		}
	}
	
	public void OnBuyButton(GameObject button){
		
		int cost = int.Parse(button.transform.parent.FindChild("ItemCost").GetComponent<UILabel>().text);
		if(DataManager.Stars >= cost){
			//TODO add item to inventory	
//			inventory.AddItem(categoryList[i], 1);
			DataManager.SubtractStars(cost);
		}
		
	}
	
	private void CreateItems(GameObject page){
		
		//Destory first 
		foreach(Transform child in grid.transform){
			Destroy(child.gameObject);
		}
		
		if(page == null || page.name == "Food"){
			uisprite.atlas = BackGroundRed;
			for(int i = 0;i<itemlogic.foodlist.Count;i++){
				GameObject item = NGUITools.AddChild(grid,ItemPrefab);
				item.transform.localScale = new Vector3(1.2f,1.3f,1);
				item.name = itemlogic.foodlist[i].ToString();
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelBlue";
				item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemlogic.items[itemlogic.foodlist[i]].description;
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = "Cost : " + itemlogic.items[itemlogic.foodlist[i]].cost.ToString();
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.items[itemlogic.foodlist[i]].name;
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.items[itemlogic.foodlist[i]].name;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
			}
		}
		else if(page.name == "Item"){
			uisprite.atlas = BackGroundGreen;
			for(int i = 0;i<itemlogic.itemlist.Count;i++){
				GameObject item = NGUITools.AddChild(grid,ItemPrefab);
				item.transform.localScale = new Vector3(1.2f,1.3f,1);
				item.name = itemlogic.itemlist[i].ToString();
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelRed";
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.items[itemlogic.itemlist[i]].name;
				item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemlogic.items[itemlogic.itemlist[i]].description;
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = "Cost : " + itemlogic.items[itemlogic.itemlist[i]].cost.ToString();
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.items[itemlogic.itemlist[i]].name;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
			}
		}
		else if(page.name == "Decoration"){
			uisprite.atlas = BackGroundPurple;
			for(int i = 0;i<itemlogic.decolist.Count;i++){
				GameObject item = NGUITools.AddChild(grid,ItemPrefab);
				item.transform.localScale = new Vector3(1.2f,1.3f,1);
				item.name = itemlogic.decolist[i].ToString();
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelPurple";
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.items[itemlogic.decolist[i]].name;
				item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemlogic.items[itemlogic.decolist[i]].description;
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = "Cost : " + itemlogic.items[itemlogic.decolist[i]].cost.ToString();
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.items[itemlogic.decolist[i]].name;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
			}
		}
		else if(page.name == "Inhaler"){
			uisprite.atlas = BackGroundOrange;
			for(int i = 0;i<itemlogic.inhalerlist.Count;i++){
				GameObject item = NGUITools.AddChild(grid,ItemPrefab);
				item.transform.localScale = new Vector3(1.2f,1.3f,1);
				item.name = itemlogic.inhalerlist[i].ToString();
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelYellow";
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.items[itemlogic.inhalerlist[i]].name;
				item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemlogic.items[itemlogic.inhalerlist[i]].description;
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = "Cost : " + itemlogic.items[itemlogic.inhalerlist[i]].cost.ToString();
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.items[itemlogic.inhalerlist[i]].name;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
			}
		}
		grid.GetComponent<UIGrid>().Reposition();
		Invoke("Reposition",0.00000001f);
	}
	
	private void Reposition(){
		grid.GetComponent<UIGrid>().Reposition();
		
	}
}
