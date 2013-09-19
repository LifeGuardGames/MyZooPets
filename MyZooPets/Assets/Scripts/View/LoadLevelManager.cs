using UnityEngine;
using System.Collections;

public class LoadLevelManager : MonoBehaviour {
    // private GameObject calendar;
    private GameObject hudPanel;
    private GameObject navigationPanel;
    private GameObject inventoryPanel;
	public GameObject NGUICamera;
	
    /*
        Use IsPaused instead of Time.timeScale to pause all critical parts of the game,
        so that nothing important happens while notifications are being displayed.

        Why not just change Time.timeScale to zero? Because any particle animations
        that come along with the notifications will be paused as well.
    */
    public static bool IsPaused = false;
	
    private const string ANCHOR_TOP = "UI Root (2D)/Camera/Anchor-Top/";
    private const string ANCHOR_CENTER = "UI Root (2D)/Camera/Anchor-Center/";
    private const string ANCHOR_BOTTOMLEFT = "UI Root (2D)/Camera/Anchor-BottomLeft/";
    private const string ANCHOR_BOTTOMRIGHT = "UI Root (2D)/Camera/Anchor-BottomRight/";

    void Awake(){
        switch(Application.loadedLevelName){
            case "NewBedRoom":
                hudPanel = GameObject.Find(ANCHOR_TOP + "HUDPanel");
                navigationPanel = GameObject.Find(ANCHOR_BOTTOMLEFT + "NavigationPanel");
                inventoryPanel = GameObject.Find(ANCHOR_BOTTOMRIGHT + "InventoryPanel");

            break;
            case "Yard":
                hudPanel = GameObject.Find(ANCHOR_TOP + "HUDPanel");
                navigationPanel = GameObject.Find(ANCHOR_BOTTOMLEFT + "NavigationPanel");
                inventoryPanel = GameObject.Find(ANCHOR_BOTTOMRIGHT + "InventoryPanel");

            break;
            case "InhalerGamePet":
                // hud = GameObject.Find(ANCHOR_TOP + "HUD");
            break;
            case "InhalerGameTeddy":
                // hud = GameObject.Find(ANCHOR_TOP + "HUD");
            break;
            case "SlotMachineGame":
                // hud = GameObject.Find(ANCHOR_TOP + "HUD");
            break;
            case "DiagnosePet":
                // hud = GameObject.Find(ANCHOR_TOP + "HUD");
            break;
        }
    }

    void Start(){
        Invoke("InitializeDataForUI", 0.5f);

		// Load bg music here
		AudioManager.Instance.PlayBackground();
    }

    //Data is ready for use so initialize all UI data
    private void InitializeDataForUI(){
        switch(Application.loadedLevelName){
            case "NewBedRoom":
                hudPanel.GetComponent<TweenToggleDemux>().Show();
                navigationPanel.GetComponent<TweenToggleDemux>().Show();
                inventoryPanel.GetComponent<TweenToggle>().Show();
            break;
            case "Yard":
                hudPanel.GetComponent<TweenToggleDemux>().Show();
                navigationPanel.GetComponent<TweenToggleDemux>().Show();
                inventoryPanel.GetComponent<TweenToggle>().Show();
            break;
            case "InhalerGamePet":
                // hud.GetComponent<MoveTweenToggleDemultiplexer>().Show();
            break;
            case "InhalerGameTeddy":
                // hud.GetComponent<MoveTweenToggleDemultiplexer>().Show();
            break;
            case "SlotMachineGame":
                // hud.GetComponent<MoveTweenToggleDemultiplexer>().Show();
            break;
            case "DiagnosePet":
                // hud.GetComponent<MoveTweenToggleDemultiplexer>().Show();
            break;
        }
    }
}
