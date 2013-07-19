using UnityEngine;
using System.Collections;

public class LoadLevelManager : MonoBehaviour {
    //UI
    private DegradationUIManager degradationUIManager; //reference to UI
    private GameObject calendar;
    private GameObject hud;
    private GameObject navigation;
    private GameObject inventory;

    private const string ANCHOR_TOP = "UI Root (2D)/Camera/Panel/Anchor-Top/";
    private const string ANCHOR_CENTER = "UI Root (2D)/Camera/Panel/Anchor-Center/";
    private const string ANCHOR_BOTTOMLEFT = "UI Root (2D)/Camera/Panel/Anchor-BottomLeft/";
    private const string ANCHOR_BOTTOMRIGHT = "UI Root (2D)/Camera/Panel/Anchor-BottomRight/";

    void Awake(){
        switch(Application.loadedLevelName){
            case "NewBedRoom":
                hud = GameObject.Find(ANCHOR_TOP + "HUD");
                calendar = GameObject.Find(ANCHOR_CENTER + "Calendar");
                navigation = GameObject.Find(ANCHOR_BOTTOMLEFT + "Navigation");
                inventory = GameObject.Find(ANCHOR_BOTTOMRIGHT + "Inventory");

            break;
            case "Yard":
                hud = GameObject.Find(ANCHOR_TOP + "HUD");
                navigation = GameObject.Find(ANCHOR_BOTTOMLEFT + "Navigation");
                inventory = GameObject.Find(ANCHOR_BOTTOMRIGHT + "Inventory");

            break;
            // case "InhalerGamePet":
            //     animator = GameObject.Find("UIManager/HUD").GetComponent<HUDAnimator>();
            //     InitializeDataForUI();
            // break;
            // case "InhalerGameTeddy":
            //     animator = GameObject.Find("UIManager/HUD").GetComponent<HUDAnimator>();
            //     InitializeDataForUI();
            // break;
            // case "SlotMachineGame":
            //     animator = GameObject.Find("UIManager/HUD").GetComponent<HUDAnimator>();
            //     InitializeDataForUI();
            // break;
        }
    }

    void Start(){
        Invoke("InitializeDataForUI", 0.5f);
    }

    //Data is ready for use so initialize all UI data
    private void InitializeDataForUI(){
        switch(Application.loadedLevelName){
            case "NewBedRoom":
                hud.GetComponent<MoveTweenToggle>().Show();
                navigation.GetComponent<MoveTweenToggleDemultiplexer>().Show();
                inventory.GetComponent<MoveTweenToggle>().Show();
            break;
            case "Yard":
                hud.GetComponent<MoveTweenToggle>().Show();
                navigation.GetComponent<MoveTweenToggle>().Show();
                inventory.GetComponent<MoveTweenToggle>().Show();
            break;
            // case "InhalerGamePet":
            //     animator.Init();
            // break;
            // case "InhalerGameTeddy":
            //     animator.Init();
            // break;
            // case "SlotMachineGame":
            // break;
        }
    }
}
