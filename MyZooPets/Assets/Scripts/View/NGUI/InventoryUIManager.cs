using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour {

	private Inventory inventory;
    private ItemLogic itemLogic;
    
    // NGUI revision variables
    public bool isDebug;
    public UISprite itemSprite;
    public UIAtlas itemAtlas;
    public UIFont font;
    public GameObject UIGrid;
    public GameObject UIButtonToggleObject;
    
    private bool isGuiShowing = true;   // Aux to keep track, not synced!!
    private float collapsedPos;
    private UIButtonToggle uiButtonToggle;
    private Dictionary<string, bool> itemTrackHash; // Hashtable to keep track of the types of items present;
    
    void Awake(){
        inventory = GameObject.Find("GameManager/InventoryLogic").GetComponent<Inventory>();
        itemLogic = GameObject.Find("GameManager/ItemLogic").GetComponent<ItemLogic>();
        collapsedPos = gameObject.GetComponent<TweenPosition>().to.x;
        uiButtonToggle = UIButtonToggleObject.GetComponent<UIButtonToggle>();
        
        itemTrackHash = new Dictionary<string, bool>();
        Inventory.OnItemAddedToInventory += OnItemAdded;
    }
    
    void Start(){
        print("start");
        for(int i=0; i<itemLogic.items.Count; i++) {
            if(inventory.InventoryArray[i] > 0){
                print("drawing");
                SpawnInventoryTypeInPanel(itemLogic.items[i].name, i);
            }
        }
    }

    //Event listener. listening to when item is dragged out of the inventory on drop
    //on something in the game
    private void OnItemDrop(object sender, InventoryDragDrop.InvDragDropArgs e){
        bool dropOnTarget = false;
        if(isDebug){
            if(e.TargetCollider.name == "Cube") dropOnTarget = true;
        }else{
             if(e.TargetCollider.name == "SpritePet" ||
                e.TargetCollider.name == "PetHead" ||
                e.TargetCollider.name == "PetTummy") dropOnTarget = true;
        }

        if(dropOnTarget){
            e.IsValidTarget = true;

            int id = int.Parse(e.ItemTransform.name); //get id from listener args
            inventory.UseItem(id); //notify inventory logic that this item is being used

            if(inventory.InventoryArray[id] > 0){ //Redraw count label if item not 0
                e.ParentTransform.Find("label").GetComponent<UILabel>().text = 
                    inventory.InventoryArray[id].ToString();
            }else{ //destroy object if it has been used up
                Destroy(e.ParentTransform.gameObject);
                UpdateBarPosition();
            }    
        }
    }

    //Event listener. listening to when new item is added to the inventory
    private void OnItemAdded(object sender, Inventory.InventoryEventArgs e){
       if(e.IsItemNew){
            SpawnInventoryTypeInPanel(itemLogic.items[e.ItemID].name, e.ItemID);
        }else{
            //this is kind of bad.... need to change the structure of the UI
            Transform item = UIGrid.transform.Find("Item/"+e.ItemID.ToString());
            item.parent.Find("label").GetComponent<UILabel>().text = inventory.InventoryArray[e.ItemID].ToString();
        }
    }
    
    private GameObject SpawnInventoryTypeInPanel(string name, int id){
        // If the item type already exists, should not create a new box
        if(itemTrackHash.ContainsKey(name) && itemTrackHash[name] == true){
            Debug.LogError("Creating new box for existing item in bar");
            return null;
        }
        else{
            // Flag new box created in hash
            itemTrackHash.Add(name, true);
            
            // Create item structure
            GameObject item = NGUITools.AddChild(UIGrid);
            item.name = "Item";
            
            UISprite spriteFill = NGUITools.AddSprite(item, itemAtlas, "fill");
            spriteFill.transform.localScale = new Vector3(90, 90, 1);   // TODO make const
            spriteFill.depth = NGUITools.CalculateNextDepth(UIGrid);
            
            GameObject SpriteGo = NGUITools.AddChild(item);
            SpriteGo.gameObject.name = id.ToString();                   // Use ID as name
            UISprite sprite = NGUITools.AddSprite(SpriteGo, itemAtlas, name);
            
            BoxCollider boxCollider = SpriteGo.gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(90, 90, 1);                  // TODO make const

            InventoryDragDrop invDragDrop = SpriteGo.gameObject.AddComponent("InventoryDragDrop") as InventoryDragDrop;
            invDragDrop.OnItemDrop += OnItemDrop;

            SpriteGo.gameObject.AddComponent("UIDragPanelContents");
            
            UILabel label = NGUITools.AddWidget<UILabel>(item);
            label.gameObject.name = "label";
            label.transform.localPosition = new Vector3(25, -25, -1);   // TODO Different atlas for now, move forward
            label.transform.localScale = new Vector3(40, 40, 1);
            label.font = font;
            label.depth = NGUITools.CalculateNextDepth(UIGrid);
            label.text = inventory.InventoryArray[id].ToString();
            
            sprite.transform.localScale = new Vector3(90, 90, 1);
            sprite.transform.localScale = new Vector3(52, 64, 1);       // TODO make const TODO Dynamic size
            sprite.depth = NGUITools.CalculateNextDepth(UIGrid);
            
            UpdateBarPosition();
            
            return item;
        }
    }
    
    public void UpdateBarPosition(){
        UIGrid.GetComponent<UIGrid>().Reposition();
        print(inventory.InventoryCount);
        if(gameObject.GetComponent<TweenPosition>().from.x > -1064){  // Limit Move after x items     // TODO make const
            gameObject.GetComponent<TweenPosition>().from.x = collapsedPos - inventory.InventoryCount * 90;
            if(uiButtonToggle.isActive){    // Animate the move if inventory is open
                Hashtable optional = new Hashtable();
                optional.Add("ease", LeanTweenType.easeOutBounce);
                LeanTween.moveLocalX(gameObject, collapsedPos - inventory.InventoryCount * 90, 0.4f, optional);              
            }
        }
    }
    
    // Image button clicked receiver
    public void ExpandToggled(){
        // Local aux to keep track of toggles
        isGuiShowing = !isGuiShowing;
        
        // Change the sprite on the button
        UIButtonToggleObject.GetComponent<UIImageButton>().normalSprite = isGuiShowing ? "InventoryContract" : "InventoryExpand";
        UIButtonToggleObject.GetComponent<UIImageButton>().disabledSprite = isGuiShowing ? "InventoryContract" : "InventoryExpand";
        UIButtonToggleObject.GetComponent<UIImageButton>().hoverSprite = isGuiShowing ? "InventoryContract" : "InventoryExpand";
        UIButtonToggleObject.GetComponent<UIImageButton>().pressedSprite = isGuiShowing ? "InventoryContract" : "InventoryExpand";
    }

}
