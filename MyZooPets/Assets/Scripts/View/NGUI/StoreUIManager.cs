using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StoreUIManager : MonoBehaviour {
	
	public GameObject ItemPrefab;
	public GameObject ItemSpritePrefab;
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
		//debug option. use only in Store_NGUI scene
//		itemlogic = GameObject.Find("GameManager/ItemLogic").GetComponent<ItemLogic>();
		itemlogic = GameObject.Find("Grid").GetComponent<ItemLogic>();
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
	
	//This function is called when buying an item
	//It creates a icon for the item and move it to Inventory
	//TODO Scales are a little mess up
	public void OnBuyAnimation(GameObject sprite){
		Vector3 origin = sprite.transform.position;
		//adjust moving speed here
		float speed = 50f;
		
		//Change the 3 V3 to where icon should move
		Vector3[] path = new Vector3[4];
		path[0] = origin;
		path[1] = new Vector3(3f,0f,-2f);
		path[2] = new Vector3(5f,-5f,-2f);
		path[3] = new Vector3(7f,-7f,-2f);
		
		Hashtable optional = new Hashtable();
		optional.Add("ease",LeanTweenType.easeOutQuad);
		GameObject animationSprite = NGUITools.AddChild(GameObject.Find("StoreGUI")/*sprite.transform.parent.gameObject*/,ItemSpritePrefab);
		animationSprite.transform.position = origin;
		animationSprite.transform.localScale = new Vector3(156,154,0);
		animationSprite.GetComponent<UISprite>().spriteName = sprite.GetComponent<UISprite>().spriteName;
		LeanTween.move(animationSprite,path,speed,optional);
		Destroy(animationSprite);
	}
	
	
	//Called when "Buy" is clicked
	public void OnBuyButton(GameObject button){
		int cost = int.Parse(button.transform.parent.FindChild("ItemCost").GetComponent<UILabel>().text);
		if(DataManager.Stars >= cost){
			//TODO add item to inventory	
//			inventory.AddItem(categoryList[i], 1);
			DataManager.SubtractStars(cost);
		}
		OnBuyAnimation(button.transform.parent.FindChild("ItemTexture").gameObject);
	}
	
	//Drawing function. 
	//draw according to itemlogic 
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
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemlogic.items[itemlogic.foodlist[i]].cost.ToString();
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
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemlogic.items[itemlogic.itemlist[i]].cost.ToString();
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
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemlogic.items[itemlogic.decolist[i]].cost.ToString();
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
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemlogic.items[itemlogic.inhalerlist[i]].cost.ToString();
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.items[itemlogic.inhalerlist[i]].name;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
			}
		}
		grid.GetComponent<UIGrid>().Reposition();
		Invoke("Reposition",0.00000001f);
	}
	//Delay calling reposition due to async problem Destroying/Repositionoing.
	//TODO Maybe change later when we have more items 
	private void Reposition(){
		grid.GetComponent<UIGrid>().Reposition();
		
	}
}
