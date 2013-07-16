using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class InventoryGUI : MonoBehaviour{
    
    private Inventory inventory;
    private Texture2D textureSwap; //texture used when dragging
    private bool pickedUp; //True: item has been picke dup
    private bool isMenuExpanded; //True: inventory expanded
    private int pickUpId; //The id of the item picked up
    private ItemLogic itemLogic;

    //LeanTween position
    private LTRect inventoryRect; 
    private Vector2 displayPosition; 
    private Vector2 hidePosition;
    private Rect inventoryTextureRect; //inventory background rect

    private const int ITEM_BOX_HEIGHT = 75;
    private const int ITEM_BOX_WIDTH = 75;

    // native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    public Texture2D minusTexture;
    public Texture2D plusTexture;
    public Texture2D itemBarTexture;
    public GUIStyle itemCountTextStyle;
    public GUISkin defaultSkin;
	
	// NGUI revision variables
	public UISprite itemSprite;
	public UIAtlas itemAtlas;
	public UIFont font;
	public GameObject UIGrid;
	public GameObject parentWindow;
	public GameObject UIButtonToggleObject;
	
	private bool isGuiShowing = true; 	// Aux to keep track, not synced!!
	private int itemCount = 0;			// Local GUI item count	
	private float collapsedPos;
	private UIButtonToggle uiButtonToggle;
	private Dictionary<string, bool> itemTrackHash;	// Hashtable to keep track of the types of items present;
	
    void Awake(){
        inventory = GameObject.Find("GameManager/InventoryLogic").GetComponent<Inventory>();
        itemLogic = GameObject.Find("GameManager/ItemLogic").GetComponent<ItemLogic>();

        inventoryRect = new LTRect(0, NATIVE_HEIGHT - 100, 1000, 105);
        displayPosition = new Vector2(0, 700);
        hidePosition = new Vector2(0, 850);
        pickedUp = false;
        isMenuExpanded = true;
        pickUpId = -1;

        // Set listener to resize Inventory texture when inventory size change
        // Inventory.OnInventoryResize += ResizeInventory;
    }
	
	void Start(){
		collapsedPos = parentWindow.GetComponent<TweenPosition>().to.x;
		uiButtonToggle = UIButtonToggleObject.GetComponent<UIButtonToggle>();
		
		itemTrackHash = new Dictionary<string, bool>();
		
		// Populate initial items
		for(int i = 0; i < itemLogic.items.Count; i++){
			if(inventory.InventoryArray[i] != null && inventory.InventoryArray[i] > 0)
        		SpawnInventoryTypeInPanel(itemLogic.items[i].name, i);
		}
	}
	
//    void Update(){
//        if(!LoadDataLogic.IsDataLoaded) return;
//        if(pickedUp){
//            // if dragging an item, don't treat this as a swipe
//            SwipeDetection.CancelSwipe();
//            if(Input.touchCount > 0){
//                if(Input.GetTouch(0).phase == TouchPhase.Ended){
//                    Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
//                    RaycastHit hit;
//
//                    if(Physics.Raycast(myRay,out hit)){
//	                    if(itemLogic.items[pickUpId].itemreceiver == ItemReceiver.Pet){
//	                        if(hit.collider.name == "SpritePet" ||
//	                            hit.collider.name == "PetHead" ||
//	                            hit.collider.name == "PetTummy"){
//	                            inventory.UseItem(pickUpId);
//	                        }
//	                    }
//						else if(itemLogic.items[pickUpId].itemreceiver == ItemReceiver.Floor){
//							print ("floorItem");
//							if(hit.collider == GameObject.Find("Floor Rectangular").collider ||
//								hit.collider == GameObject.Find("planeCenter").collider) inventory.UseItem(pickUpId);
//						}
//						else if(itemLogic.items[pickUpId].itemreceiver == ItemReceiver.Wall){
//							if(hit.collider == GameObject.Find("Walls").collider) inventory.UseItem(pickUpId);
//						}
//					}
//                    pickedUp = false;
//                    pickUpId = -1;
//                }
//            }
//        }
//	}
	
	public bool NotifyDroppedItem(int pickUpId){
		Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
	    RaycastHit hit;
	
	    if(Physics.Raycast(myRay,out hit)){
	        if(itemLogic.items[pickUpId].itemreceiver == ItemReceiver.Pet){
	            if(hit.collider.name == "SpritePet" ||
		                hit.collider.name == "PetHead" ||
		                hit.collider.name == "PetTummy"){
	                inventory.UseItem(pickUpId);
					return true;
	            }
				else
					return false;
	        }
			else if(itemLogic.items[pickUpId].itemreceiver == ItemReceiver.Floor){
				print ("floorItem");
				if(hit.collider == GameObject.Find("Floor Rectangular").collider ||
						hit.collider == GameObject.Find("planeCenter").collider){
					inventory.UseItem(pickUpId);
					return true;
				}
				else
					return false;
			}
			else if(itemLogic.items[pickUpId].itemreceiver == ItemReceiver.Wall){
				if(hit.collider == GameObject.Find("Walls").collider){
					inventory.UseItem(pickUpId);
					return true;
				}
				else
					return false;
			}
			else
				return false;
		}
		else
			return false;
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
			spriteFill.transform.localScale = new Vector3(90, 90, 1); 	// TODO make const
			spriteFill.depth = NGUITools.CalculateNextDepth(UIGrid);
			
			GameObject SpriteGo = NGUITools.AddChild(item);
			SpriteGo.gameObject.name = id.ToString();					// Use ID as name
			UISprite sprite = NGUITools.AddSprite(SpriteGo, itemAtlas, name);
			
			BoxCollider boxCollider = SpriteGo.gameObject.AddComponent<BoxCollider>();
			boxCollider.isTrigger = true;
			boxCollider.size = new Vector3(90, 90, 1); 					// TODO make const
			SpriteGo.gameObject.AddComponent("InventoryDragDrop");
			SpriteGo.gameObject.AddComponent("UIDragPanelContents");
			
			UILabel label = NGUITools.AddWidget<UILabel>(item);
			label.gameObject.name = "label";
			label.transform.localPosition = new Vector3(25, -25, -1); 	// TODO Different atlas for now, move forward
			label.transform.localScale = new Vector3(40, 40, 1);
			label.font = font;
			label.depth = NGUITools.CalculateNextDepth(UIGrid);
			label.text = inventory.InventoryArray[id].ToString();
			
			sprite.transform.localScale = new Vector3(52, 64, 1);		// TODO make const TODO Dynamic size
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
		
		if(parentWindow.GetComponent<TweenPosition>().from.x > -1064){ 	// Limit Move after x items		// TODO make const
			parentWindow.GetComponent<TweenPosition>().from.x = collapsedPos - itemCount * 90;
			if(uiButtonToggle.isActive){	// Animate the move if inventory is open
				Hashtable optional = new Hashtable();
				optional.Add("ease", LeanTweenType.easeOutBounce);
				LeanTween.moveLocalX(parentWindow, collapsedPos - itemCount * 90, 0.4f, optional);				
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

    void OnGUI(){
		
		// Test, deprecated...
//		if(GUI.Button(new Rect(100, 100, 100, 100), "Spawn")){
//			SpawnInventoryTypeInPanel("teddy", itemCount);
//		}
		
		/*
        if(!LoadDataLogic.IsDataLoaded) return;
        GUI.skin = defaultSkin;

        // Proportional scaling
        if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
        }
       
        GUILayout.BeginArea(inventoryRect.rect);
            GUI.DrawTexture(inventoryTextureRect, itemBarTexture);
            GUILayout.BeginHorizontal();

                int itemCounter =0;
                //implementing itemLogic
                for(int i = 0 ;i < itemLogic.items.Count; i++){
                    if(i == pickUpId){
                        textureSwap = null;
                    }
                    else{
                        textureSwap = itemLogic.items[i].Texture;
                    }
                    if(inventory.InventoryArray[i]!=0){

                        GUILayout.BeginVertical(GUILayout.Width(ITEM_BOX_WIDTH));
                            GUILayout.FlexibleSpace();
                            if(GUILayout.RepeatButton(textureSwap, GUILayout.Height(ITEM_BOX_HEIGHT), 
                                GUILayout.Width(ITEM_BOX_WIDTH))){
                                pickedUp = true;
                                pickUpId = i;
                            }
                            GUI.Label(new Rect(itemCounter*ITEM_BOX_WIDTH, 0, ITEM_BOX_WIDTH, ITEM_BOX_HEIGHT),
                                "x " + inventory.InventoryArray[i].ToString(),itemCountTextStyle);
                            itemCounter++;
                            GUILayout.FlexibleSpace();
                        GUILayout.EndVertical();
                    }
                }

                // move in/out of item bar
                GUILayout.BeginVertical(GUILayout.Width(ITEM_BOX_WIDTH));
                GUILayout.FlexibleSpace();
                if(isMenuExpanded){
                    if(GUILayout.Button(minusTexture, GUILayout.Height(ITEM_BOX_HEIGHT), GUILayout.Width(ITEM_BOX_WIDTH))){
                        isMenuExpanded = false;
                        Hashtable optional = new Hashtable();
                        optional.Add("ease", LeanTweenType.easeInOutQuad);
                        LeanTween.move(inventoryRect, new Vector2(-80f * itemCounter, NATIVE_HEIGHT - 100), 0.3f, optional);
                    }
                }
                else{
                    if(GUILayout.Button(plusTexture, GUILayout.Height(ITEM_BOX_HEIGHT), GUILayout.Width(ITEM_BOX_WIDTH))){
                        isMenuExpanded = true;
                        Hashtable optional = new Hashtable();
                        optional.Add("ease", LeanTweenType.easeInOutQuad);
                        LeanTween.move(inventoryRect, new Vector2(0, NATIVE_HEIGHT - 100), 0.3f, optional);
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        GUILayout.EndArea(); 

        //Texture swap
        if(pickedUp){
            Event e = Event.current;
            GUI.DrawTexture(new Rect(e.mousePosition.x - ITEM_BOX_WIDTH / 2, 
                e.mousePosition.y - ITEM_BOX_HEIGHT / 2, ITEM_BOX_WIDTH,ITEM_BOX_HEIGHT),itemLogic.items[pickUpId].Texture);

        }
        
        */
    }
	
	// TODO Move these out, use toggleTween script on gameobject!!!	
    //Display InventoryGUI in game
    public void Display(){
        Hashtable optional = new Hashtable();
        optional.Add("ease", LeanTweenType.easeOutElastic);
        LeanTween.move(inventoryRect, displayPosition, 1f, optional);
    }

    //Hide InventoryGUI in game
    public void Hide(){
        LeanTween.move(inventoryRect, hidePosition, 0.5f);
    }

    private void ResizeInventory(object sender, EventArgs e){
        inventoryTextureRect = new Rect(-900 + 80f * inventory.InventoryCount, 0, 
            inventoryRect.rect.width, inventoryRect.rect.height);
    }
}