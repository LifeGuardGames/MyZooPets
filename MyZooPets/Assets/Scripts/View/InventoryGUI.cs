using UnityEngine;
using System.Collections;

public class InventoryGUI : MonoBehaviour{
    private LTRect inventoryRect;
    private Inventory inventory;
    private Texture2D textureSwap;
    private bool pickedUp;
    private bool isMenuExpanded;
    private int pickUpId;
    private ItemLogic itemLogic;
    private Vector2 displayPosition;
    private Vector2 hidePosition;

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

    void Start(){
        inventory = GameObject.Find("GameManager").GetComponent<Inventory>();
        itemLogic = GameObject.Find("GameManager").GetComponent<ItemLogic>();
    }

    //Initialize InventoryGUI
    public void Init(){
        inventoryRect = new LTRect(0, NATIVE_HEIGHT - 100, 1000, 105);
        displayPosition = new Vector2(0, 700);
        hidePosition = new Vector2(0, 850);
        pickedUp = false;
        isMenuExpanded = true;
        pickUpId = -1;
    }

    //Display InventoryGUI in game
    public void Display(){
        LeanTween.move(inventoryRect, displayPosition, 0.5f);
    }

    //Hide InventoryGUI in game
    public void Hide(){
        LeanTween.move(inventoryRect, hidePosition, 0.5f);
    }

    void Update(){
        if(!LoadDataLogic.IsDataLoaded) return;
        if(pickedUp){
            if(Input.touchCount > 0){
                if(Input.GetTouch(0).phase == TouchPhase.Ended){
                    Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if(Physics.Raycast(myRay,out hit)){
                        if(hit.collider.name == "SpritePet" ||
                            hit.collider.name == "PetHead" ||
                            hit.collider.name == "PetTummy"){
                            inventory.useItem(pickUpId);
                        }
                    }
                    pickedUp = false;
                    pickUpId = -1;
                }
            }
        }
    }

    void OnGUI(){
        if(!LoadDataLogic.IsDataLoaded) return;
        GUI.skin = defaultSkin;

        // Proportional scaling
        if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
        }

        //TO DO: move this to logic
        int counter = 0;
        for(int i = 0;i< inventory.InventoryArray.Length;i++){
            if(inventory.InventoryArray[i]!=0) counter++;
        }

        //Calculate the position of the inventory bar depending on how many items
        //are in the inventory
        Rect inventoryTextureRect = new Rect(inventoryRect.rect.x  - 900 + 80f * counter, 
             inventoryRect.rect.y - 10, inventoryRect.rect.width, inventoryRect.rect.height);
        GUI.DrawTexture(inventoryTextureRect, itemBarTexture);
        GUILayout.BeginArea(inventoryRect.rect);
        GUILayout.BeginHorizontal();

        counter =0;

        //implementing itemLogic
        for(int i = 0 ;i < itemLogic.items.Count; i++){
            if(i == pickUpId){
                textureSwap = null;
            }
            else{
                textureSwap = itemLogic.items[i].Texture;
            }
            if(inventory.InventoryArray[i]!=0){
                if(GUILayout.RepeatButton(textureSwap, GUILayout.Height(ITEM_BOX_HEIGHT), GUILayout.Width(ITEM_BOX_WIDTH))){
                    pickedUp = true;
                    pickUpId = i;
                }
                counter++;
                // GUI.Label(new Rect(-10+counter*80-80,35,100,80),"x " + inventory.InventoryArray[i].ToString(),itemCountTextStyle);
            }
        }

        // move in/out of item bar
        if(isMenuExpanded){
            if(GUILayout.Button(minusTexture, GUILayout.Height(ITEM_BOX_HEIGHT), GUILayout.Width(ITEM_BOX_WIDTH))){
                isMenuExpanded = false;
                Hashtable optional = new Hashtable();
                optional.Add("ease", LeanTweenType.easeInOutQuad);
                LeanTween.move(inventoryRect, new Vector2(-80f * counter, NATIVE_HEIGHT - 100), 0.3f, optional);
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
        GUILayout.EndHorizontal();
        GUILayout.EndArea(); 

        //Texture swap
        if(pickedUp){
            Event e = Event.current;
            GUI.DrawTexture(new Rect(e.mousePosition.x - ITEM_BOX_WIDTH / 2, 
                e.mousePosition.y - ITEM_BOX_HEIGHT / 2, ITEM_BOX_WIDTH,ITEM_BOX_HEIGHT),itemLogic.items[pickUpId].Texture);

        }
    }
}