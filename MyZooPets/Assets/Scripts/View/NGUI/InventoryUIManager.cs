using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour {

	private Inventory inventory;
    private bool pickedUp; //True: item has been picke dup
    private int pickUpId; //The id of the item picked up
    private ItemLogic itemLogic;

    
    // NGUI revision variables
    public UISprite itemSprite;
    public UIAtlas itemAtlas;
    public UIFont font;
    public GameObject UIGrid;
    public GameObject UIButtonToggleObject;
    
    private bool isGuiShowing = true;   // Aux to keep track, not synced!!
    private int itemCount = 0;          // Local GUI item count 
    private float collapsedPos;
    private UIButtonToggle uiButtonToggle;
    private Dictionary<string, bool> itemTrackHash; // Hashtable to keep track of the types of items present;
    
    void Awake(){
        inventory = GameObject.Find("GameManager/InventoryLogic").GetComponent<Inventory>();
        itemLogic = GameObject.Find("GameManager/ItemLogic").GetComponent<ItemLogic>();

        pickedUp = false;
        pickUpId = -1;
    }
    
    void Start(){
        collapsedPos = gameObject.GetComponent<TweenPosition>().to.x;
        uiButtonToggle = UIButtonToggleObject.GetComponent<UIButtonToggle>();
        
        itemTrackHash = new Dictionary<string, bool>();
        
        // Populate initial items
        //Note: make sure InventoryLogic populates item before this loop. Start()
        //may not be called in the same order everytime, so beware
        for(int i = 0; i < itemLogic.items.Count; i++){
            if(inventory.InventoryArray[i] > 0)
                SpawnInventoryTypeInPanel(itemLogic.items[i].name, i);
        }
    }
    
    public bool NotifyDroppedItem(int pickUpId){
        Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool retVal = false; 
        if(Physics.Raycast(myRay,out hit)){
            if(itemLogic.items[pickUpId].itemreceiver == ItemReceiver.Pet){
                if(hit.collider.name == "SpritePet" ||
                        hit.collider.name == "PetHead" ||
                        hit.collider.name == "PetTummy"){
                    inventory.UseItem(pickUpId);
                    retVal = true;
                }
            }
            else if(itemLogic.items[pickUpId].itemreceiver == ItemReceiver.Floor){
                print ("floorItem");
                if(hit.collider == GameObject.Find("Floor Rectangular").collider ||
                        hit.collider == GameObject.Find("planeCenter").collider){
                    inventory.UseItem(pickUpId);
                    retVal = true;
                }
            }
            else if(itemLogic.items[pickUpId].itemreceiver == ItemReceiver.Wall){
                if(hit.collider == GameObject.Find("Walls").collider){
                    inventory.UseItem(pickUpId);
                    retVal = true;
                }
            }
        }
        return retVal;
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
            InventoryListener listener = item.AddComponent("InventoryListener") as InventoryListener;
            listener.Count = inventory.InventoryArray[id];
            
            UISprite spriteFill = NGUITools.AddSprite(item, itemAtlas, "fill");
            spriteFill.transform.localScale = new Vector3(90, 90, 1);   // TODO make const
            spriteFill.depth = NGUITools.CalculateNextDepth(UIGrid);
            
            GameObject SpriteGo = NGUITools.AddChild(item);
            SpriteGo.gameObject.name = id.ToString();                   // Use ID as name
            UISprite sprite = NGUITools.AddSprite(SpriteGo, itemAtlas, name);
            
            BoxCollider boxCollider = SpriteGo.gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(90, 90, 1);                  // TODO make const
            SpriteGo.gameObject.AddComponent("InventoryDragDrop");
            SpriteGo.gameObject.AddComponent("UIDragPanelContents");
            
            UILabel label = NGUITools.AddWidget<UILabel>(item);
            label.gameObject.name = "label";
            label.transform.localPosition = new Vector3(25, -25, -1);   // TODO Different atlas for now, move forward
            label.transform.localScale = new Vector3(40, 40, 1);
            label.font = font;
            label.depth = NGUITools.CalculateNextDepth(UIGrid);
            label.text = inventory.InventoryArray[id].ToString();
            
            sprite.transform.localScale = new Vector3(52, 64, 1);       // TODO make const TODO Dynamic size
            sprite.depth = NGUITools.CalculateNextDepth(UIGrid);
            
            itemCount++;
            UpdateBarPosition();
            
            return item;
        }
    }
    
    // From InventoryListener 
    public void DecreaseItemTypeCount(){
        itemCount--;
        UpdateBarPosition();
    }
    
    public void UpdateBarPosition(){
        UIGrid.GetComponent<UIGrid>().Reposition();
        
        if(gameObject.GetComponent<TweenPosition>().from.x > -1064){  // Limit Move after x items     // TODO make const
            gameObject.GetComponent<TweenPosition>().from.x = collapsedPos - itemCount * 90;
            if(uiButtonToggle.isActive){    // Animate the move if inventory is open
                Hashtable optional = new Hashtable();
                optional.Add("ease", LeanTweenType.easeOutBounce);
                LeanTween.moveLocalX(gameObject, collapsedPos - itemCount * 90, 0.4f, optional);              
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
