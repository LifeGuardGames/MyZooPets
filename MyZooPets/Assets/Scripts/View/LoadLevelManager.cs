using UnityEngine;
using System.Collections;

public class LoadLevelManager : MonoBehaviour {
    // private GameObject calendar;
    private GameObject hud;
    private GameObject navigation;
    private GameObject inventory;

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
                hud = GameObject.Find(ANCHOR_TOP + "HUD");
                navigation = GameObject.Find(ANCHOR_BOTTOMLEFT + "Navigation");
                inventory = GameObject.Find(ANCHOR_BOTTOMRIGHT + "Inventory");

            break;
            case "Yard":
                hud = GameObject.Find(ANCHOR_TOP + "HUD");
                navigation = GameObject.Find(ANCHOR_BOTTOMLEFT + "Navigation");
                inventory = GameObject.Find(ANCHOR_BOTTOMRIGHT + "Inventory");

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
		AudioManager.Instance.PlayBackground("background1");
    }

    //Data is ready for use so initialize all UI data
    private void InitializeDataForUI(){
        switch(Application.loadedLevelName){
            case "NewBedRoom":
                hud.GetComponent<MoveTweenToggleDemultiplexer>().Show();
                navigation.GetComponent<MoveTweenToggleDemultiplexer>().Show();
                inventory.GetComponent<MoveTweenToggle>().Show();
            break;
            case "Yard":
                hud.GetComponent<MoveTweenToggleDemultiplexer>().Show();
                navigation.GetComponent<MoveTweenToggleDemultiplexer>().Show();
                inventory.GetComponent<MoveTweenToggle>().Show();
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
