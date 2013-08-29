using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StoreUIManager : SingletonUI<StoreUIManager> {
	public GameObject ItemPrefab;
	public GameObject ItemSpritePrefab;
	public GameObject storePanel;
	public UIAtlas BackGroundRed;
	public UIAtlas BackGroundGreen;
	public UIAtlas BackGroundOrange;
	public UIAtlas BackGroundPurple;

	private bool changePage;
	private int page;
	private UISprite uisprite;
	private GameObject grid;
	private List<GameObject> animSpriteRemoveList = new List<GameObject>(); //sprites used for buy animation

	void Awake(){
		uisprite = GameObject.Find("BuyingAreaBackground").GetComponent<UISprite>();
		grid = GameObject.Find("Grid");
	}

	// Use this for initialization
	void Start () {
		CreateItems(null);
	}

	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		
		storePanel.GetComponent<MoveTweenToggle>().Show();
	}

	protected override void _CloseUI(){
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();		
		
		storePanel.GetComponent<MoveTweenToggle>().Hide();
	}

	//This function is called when buying an item
	//It creates a icon for the item and move it to Inventory
	//TODO Scales are a little mess up
	public void OnBuyAnimation(GameObject sprite){
		Vector3 origin = new Vector3(sprite.transform.position.x, sprite.transform.position.y, -0.1f);
		string itemID = sprite.transform.parent.name;
		Vector3 itemPosition = origin;

		//-0.22
		itemPosition = InventoryUIManager.Instance.GetPositionOfInvItem(itemID);

		//adjust moving speed here
		float speed = 1f;

		//Change the 3 V3 to where icon should move
		Vector3[] path = new Vector3[4];
		path[0] = origin;
		path[1] = origin + new Vector3(0f, 1.5f, -0.1f);
		path[2] = origin;
		path[3] = itemPosition;

		Hashtable optional = new Hashtable();
		GameObject animationSprite = NGUITools.AddChild(storePanel, ItemSpritePrefab);

		optional.Add("ease", LeanTweenType.easeOutQuad);
		optional.Add ("onComplete", "DestroyAllSprites");
		optional.Add("onCompleteTarget", gameObject);
		animationSprite.transform.position = origin;
		animationSprite.transform.localScale = new Vector3(90, 90, 1);
		animationSprite.GetComponent<UISprite>().spriteName = sprite.GetComponent<UISprite>().spriteName;
		LeanTween.move(animationSprite, path, speed, optional);
		animSpriteRemoveList.Add(animationSprite);
	}

	//helper for Buying Animation
	public void DestroyAllSprites(){
		foreach (GameObject go in animSpriteRemoveList){
			Destroy(go);
		}
		animSpriteRemoveList.Clear();
	}

	//Called when "Buy" is clicked
	public void OnBuyButton(GameObject button){
		string itemID = button.transform.parent.name;
		Item itemData = ItemLogic.Instance.GetItem(itemID);
		if(DataManager.Instance.Stats.Stars >= itemData.Cost){
			InventoryLogic.Instance.AddItem(itemID, 1);
			StatsController.Instance.ChangeStats(0, Vector3.zero, itemData.Cost * -1, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);	// Convert to negative
			OnBuyAnimation(button.transform.parent.FindChild("ItemTexture").gameObject);
		}
	}

	//Drawing function.
	//draw according to ItemLogic.Instance 
	private void CreateItems(GameObject page){

		//Destory first
		foreach(Transform child in grid.transform){
			Destroy(child.gameObject);
		}

		if(page == null || page.name == "Food"){
			uisprite.atlas = BackGroundRed;
			List<Item> foodList = ItemLogic.Instance.FoodList;

			foreach(Item itemData in foodList){
				GameObject itemUIObject = NGUITools.AddChild(grid, ItemPrefab);
				SetUpItemObject(itemUIObject, itemData);
			}
		}else if(page.name == "Usable"){
			uisprite.atlas = BackGroundGreen;
			List<Item> usableList = ItemLogic.Instance.UsableList;

			foreach(Item itemData in usableList){
				GameObject itemUIObject = NGUITools.AddChild(grid, ItemPrefab);
				SetUpItemObject(itemUIObject, itemData);
			}
		}else if(page.name == "Decoration"){

		}
		
		grid.GetComponent<UIGrid>().Reposition();
		Invoke("Reposition",0.00000001f);
	}

	private void SetUpItemObject(GameObject itemUIObject, Item itemData){
		itemUIObject.name = itemData.ID;
		itemUIObject.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemData.Description;
		itemUIObject.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemData.Cost.ToString();
		itemUIObject.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemData.Name;
		itemUIObject.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemData.TextureName;
		itemUIObject.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
		itemUIObject.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
	}

	//Delay calling reposition due to async problem Destroying/Repositionoing.
	//TODO Maybe change later when we have moreItems 
	private void Reposition(){
		grid.GetComponent<UIGrid>().Reposition();

	}
}
