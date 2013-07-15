using UnityEngine;
using System.Collections;
using System;

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

	
	
    void Awake(){
        inventory = GameObject.Find("GameManager/InventoryLogic").GetComponent<Inventory>();
        itemLogic = GameObject.Find("GameManager/ItemLogic").GetComponent<ItemLogic>();

        inventoryRect = new LTRect(0, NATIVE_HEIGHT - 100, 1000, 105);
        displayPosition = new Vector2(0, 700);
        hidePosition = new Vector2(0, 850);
        pickedUp = false;
        isMenuExpanded = true;
        pickUpId = -1;

        //set listener to resize Inventory texture when inventory size change
        Inventory.OnInventoryResize += ResizeInventory;
    }

    void Update(){
        if(!LoadDataLogic.IsDataLoaded) return;
        if(pickedUp){
            // if dragging an item, don't treat this as a swipe
            SwipeDetection.CancelSwipe();
            if(Input.touchCount > 0){
                if(Input.GetTouch(0).phase == TouchPhase.Ended){
                    Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if(Physics.Raycast(myRay,out hit)){
	                    if(itemLogic.items[pickUpId].itemreceiver == ItemReceiver.Pet){
	                        if(hit.collider.name == "SpritePet" ||
	                            hit.collider.name == "PetHead" ||
	                            hit.collider.name == "PetTummy"){
	                            inventory.UseItem(pickUpId);
	                        }
	                    }
						else if(itemLogic.items[pickUpId].itemreceiver == ItemReceiver.Floor){
							print ("floorItem");
							if(hit.collider == GameObject.Find("Floor Rectangular").collider ||
								hit.collider == GameObject.Find("planeCenter").collider) inventory.UseItem(pickUpId);
						}
						else if(itemLogic.items[pickUpId].itemreceiver == ItemReceiver.Wall){
							if(hit.collider == GameObject.Find("Walls").collider) inventory.UseItem(pickUpId);
						}
					}
                    pickedUp = false;
                    pickUpId = -1;
                }
            }
        }
    }
	

	

	
	private GameObject SpawnInventoryInPanel(){
		GameObject item = NGUITools.AddChild(UIGrid);
		item.name = "Item";
//		BoxCollider boxCollider = item.AddComponent<BoxCollider>();
//		boxCollider.isTrigger = true;
//		boxCollider.size = new Vector3(90, 90, 1); 					// TODO make const
		item.AddComponent("UIDragPanelContents");
		//item.AddComponent("DragDropItem");
		//item.AddComponent("InventoryDragDrop");
		
		UISprite spriteFill = NGUITools.AddSprite(item, itemAtlas, "fill");
		spriteFill.transform.localScale = new Vector3(90, 90, 1); 	// TODO make const
		spriteFill.depth = NGUITools.CalculateNextDepth(UIGrid);
		
		GameObject spriteGO = NGUITools.AddChild(item);
		
		BoxCollider boxCollider = spriteGO.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		boxCollider.size = new Vector3(90, 90, 1); 					// TODO make const
		
		spriteGO.name = "SpriteGO";
		UISprite sprite = NGUITools.AddSprite(spriteGO, itemAtlas, "teddy");
		sprite.transform.localScale = new Vector3(52, 64, 1);		// TODO make const
		sprite.depth = NGUITools.CalculateNextDepth(UIGrid);
		spriteGO.AddComponent("InventoryDragDrop");
		
		itemCount++;
		
		return item;
	}
	
//	void Printo(){
//		Debug.Log("KABOOYA");	// TODO windowsRoot callback lcok - UIButtonToggle to keep track of the state, the toggle button doesnt persist state?? check
//	}

	public UISprite itemSprite;
	public UIAtlas itemAtlas;
	public GameObject UIGrid;
	public GameObject parentWindow;
	public GameObject UIButtonToggleObject;
	private UIButtonToggle uiButtonToggle;
	
	private int itemCount = 0;
	private float collapsedPos;
	
	void Start(){
		collapsedPos = 1115f;
		uiButtonToggle = UIButtonToggleObject.GetComponent<UIButtonToggle>();
	}
	
    void OnGUI(){
		if(GUI.Button(new Rect(100, 100, 100, 100), "spawn")){
			SpawnInventoryInPanel();
			UIGrid.GetComponent<UIGrid>().Reposition();	// Reposition the grid after item added
			
			if(parentWindow.GetComponent<TweenPosition>().from.x > 215){ 	// Limit Move after x items
				parentWindow.GetComponent<TweenPosition>().from.x = collapsedPos - itemCount * 90;

				if(uiButtonToggle.isActive){	// Animate the move if inventory is open
					Hashtable optional = new Hashtable();
					optional.Add("ease", LeanTweenType.easeOutBounce);
					LeanTween.moveLocalX(parentWindow, collapsedPos - itemCount * 90, 0.4f, optional);				
				}
			}
		}
		
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