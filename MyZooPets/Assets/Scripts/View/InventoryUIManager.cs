using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class InventoryUIManager : Singleton<InventoryUIManager> {
	public GameObject inventoryPanel;
    // NGUI revision variables
    public bool isDebug;
    public UISprite itemSprite;
    public UIAtlas itemAtlas;
    public UIFont font;
    public GameObject UIGridObject;
    public GameObject UIButtonToggleObject;
    public GameObject UIButtonSpriteObject;
    public GameObject spritePet;
    public GameObject speechBubblePrefab;

    private bool isGuiShowing = true;   // Aux to keep track, not synced!!
    private float collapsedPos;
    private UIButtonToggle uiButtonToggle;
    private UISprite uiSprite;
    private Dictionary<string, bool> itemTrackHash; // Hashtable to keep track of the types of items present;
    private Inventory inventory;
    private ItemLogic itemLogic;
    
    void Awake(){
        inventory = GameObject.Find("GameManager/InventoryLogic").GetComponent<Inventory>();
        itemLogic = GameObject.Find("GameManager/ItemLogic").GetComponent<ItemLogic>();
        uiButtonToggle = UIButtonToggleObject.GetComponent<UIButtonToggle>();
        uiSprite = UIButtonSpriteObject.GetComponent<UISprite>();
        itemTrackHash = new Dictionary<string, bool>();

    }

    void Start(){
        collapsedPos = inventoryPanel.GetComponent<TweenPosition>().to.x;
        Inventory.OnItemAddedToInventory += OnItemAdded;

        // //Spawn items in the inventory for the first time
        // for(int i=0; i<itemLogic.items.Count; i++) {
        //     if(inventory.InventoryArray[i] > 0){
        //         SpawnInventoryTypeInPanel(itemLogic.items[i].textureName, i);
        //     }
        // }
    }

    void OnDestroy(){
        Inventory.OnItemAddedToInventory -= OnItemAdded;
    }

    //Event listener. listening to when item is dragged out of the inventory on drop
    //on something in the game
    private void OnItemDrop(object sender, InventoryDragDrop.InvDragDropArgs e){
        bool dropOnTarget = false;
        if(isDebug){
            if(e.TargetCollider.name == "Cube") dropOnTarget = true;
        }else{
             if(e.TargetCollider.name == "SpritePet" ||
                e.TargetCollider.name == "Head" ||
                e.TargetCollider.name == "Tummy") dropOnTarget = true;
        }

        if(dropOnTarget){
            ShowPetReceivedFoodAnimation();
            e.IsValidTarget = true;

            int id = int.Parse(e.ItemTransform.name); //get id from listener args
            inventory.UseItem(id); //notify inventory logic that this item is being used

            if(inventory.InventoryArray[id] > 0){ //Redraw count label if item not 0
                e.ParentTransform.Find("label").GetComponent<UILabel>().text = inventory.InventoryArray[id].ToString();
            }else{ //destroy object if it has been used up
                Destroy(e.ParentTransform.gameObject);
                UpdateBarPosition();
            }
        }
    }

    // Spawn a speech bubble where the pet is, and destroy the speech bubble within a certain time limit.
    private void ShowPetReceivedFoodAnimation(){
        GameObject speechBubble = Instantiate(speechBubblePrefab, spritePet.transform.position, Quaternion.identity) as GameObject;
        speechBubble.transform.parent = spritePet.transform;
        speechBubble.transform.localPosition = new Vector3(3.5f, 9, 0);
        speechBubble.transform.localScale = new Vector3(2, 2, 1);

        Destroy(speechBubble, 1.5f);
    }

    //Event listener. listening to when new item is added to the inventory
    private void OnItemAdded(object sender, Inventory.InventoryEventArgs e){
       // if(e.IsItemNew){
       //      SpawnInventoryTypeInPanel(itemLogic.items[e.ItemID].textureName, e.ItemID);
       //  }else{
       //      //this is kind of bad.... need to change the structure of the UI
       //      Transform item = UIGridObject.transform.Find("Item/"+e.ItemID.ToString());
       //      item.parent.Find("label").GetComponent<UILabel>().text = inventory.InventoryArray[e.ItemID].ToString();
       //  }
    }

    private GameObject SpawnInventoryTypeInPanel(string textureName, int id){
        return null;
   //      // If the item type already exists, should not create a new box
   //      if(itemTrackHash.ContainsKey(textureName) && itemTrackHash[textureName] == true){
   //          Debug.LogError("Creating new box for existing item in bar");
   //          return null;
   //      }
   //      else{
   //          // Flag new box created in hash
   //          itemTrackHash.Add(textureName, true);

   //          // Create item structure
   //          GameObject item = NGUITools.AddChild(UIGridObject);
   //          item.name = "Item";

   //          // gray box
   //          UISprite spriteFill = NGUITools.AddSprite(item, itemAtlas, "fill");
   //          spriteFill.transform.localScale = new Vector3(90, 90, 1);   // TODO make const
   //          spriteFill.depth = 2; // two more than the panel behind it
   //          spriteFill.transform.localPosition = new Vector3(0, 0, -15);

   //          // container for sprite
   //          GameObject SpriteGo = NGUITools.AddChild(item);
   //          SpriteGo.gameObject.name = id.ToString();                   // Use ID as name
   //          UISprite sprite = NGUITools.AddSprite(SpriteGo, itemAtlas, textureName);

   //          BoxCollider boxCollider = SpriteGo.gameObject.AddComponent<BoxCollider>();
   //          boxCollider.isTrigger = true;
   //          boxCollider.size = new Vector3(90, 90, 1);                  // TODO make const

   //          InventoryDragDrop invDragDrop = SpriteGo.gameObject.AddComponent("InventoryDragDrop") as InventoryDragDrop;
   //          invDragDrop.OnItemDrop += OnItemDrop;

   //          SpriteGo.gameObject.AddComponent("UIDragPanelContents");

   //          // actual sprite
   //          // sprite.transform.localScale = new Vector3(90, 90, 1);
   //          sprite.depth = 3; // one more than the panel
   //          sprite.transform.localScale = new Vector3(52, 64, 1);       // TODO make const TODO Dynamic size
   //          sprite.transform.localPosition = new Vector3(0, 0, -20);

			// UILabel label = NGUITools.AddWidget<UILabel>(item);
			// label.gameObject.name = "label";
			// label.transform.localPosition = new Vector3(25, -25, -20);   // TODO Different atlas for now, move forward
			// label.transform.localScale = new Vector3(40, 40, 1);
			// label.font = font;
			// label.text = inventory.InventoryArray[id].ToString();

   //          UpdateBarPosition();

   //          return item;
   //      }
    }

    public void UpdateBarPosition(){
        UIGridObject.GetComponent<UIGrid>().Reposition();
        if(inventoryPanel.GetComponent<TweenPosition>().from.x > -1064){  // Limit Move after x items     // TODO make const
            inventoryPanel.GetComponent<TweenPosition>().from.x = collapsedPos - inventory.InventoryCount * 90;
            if(uiButtonToggle.isActive){    // Animate the move if inventory is open
                Hashtable optional = new Hashtable();
                optional.Add("ease", LeanTweenType.easeOutBounce);
                LeanTween.moveLocalX(inventoryPanel, collapsedPos - inventory.InventoryCount * 90, 0.4f, optional);
            }
        }
    }

    // Image button clicked receiver
    public void ExpandToggled(){
        // Local aux to keep track of toggles
        isGuiShowing = !isGuiShowing;
        uiSprite.spriteName = isGuiShowing ? "InventoryContract" : "InventoryExpand";
    }

	public void ShowPanel(){
		inventoryPanel.GetComponent<MoveTweenToggle>().Show();
	}

	public void HidePanel(){
		inventoryPanel.GetComponent<MoveTweenToggle>().Hide();
	}
}
