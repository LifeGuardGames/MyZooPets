using UnityEngine;
using System.Collections;

/// <summary>
/// First time GUI.
/// Stuffed everything that needs to be done in the first run here.
/// If its not the first time the game is run, this will delete itself.
/// </summary>

public class FirstTimeNGUI : MonoBehaviour {

    public GameObject eggObject;
    private Vector3 eggSpritePosition = new Vector3(0f, 2.8f, 22.44f);
    private tk2dSprite eggSpriteScript;
    public GameObject nestObject;
    public string petName;
    public string petColor;

    public UILabel nameField;
    public GameObject firstTimeChoosePanel;
    public GameObject popupTitle;

    // Camera moving
    public GameObject mCamera;
    private float smooth = 1.0f;
    private bool isZoomed = false;
    private Vector3 initPosition = new Vector3(-2.47f, 11.47f, 2.83f);
    private Vector3 initFaceDirection = new Vector3(11.3f, 0, 0);
    private Vector3 finalPosition = new Vector3(4.7f, 7.08f, 12.23f);
    private Vector3 finalFaceDirection = new Vector3(11.3f, 0, 0);

    private Color currentRenderColor;
    private bool eggClicked = false;

    // public delegate void FinishHatchCallBack();
    // public static FinishHatchCallBack finishHatchCallBack; //call when hatching is done

    // public delegate void FinishCheckingForFirstTime();
    // public static FinishCheckingForFirstTime finishCheckingForFirstTime; //call when pet has been instantiated

    void Start(){
        // if(DataManager.FirstTime){ //first time playing game
            eggSpriteScript = eggObject.GetComponent<tk2dSprite>();
            currentRenderColor = RenderSettings.ambientLight;
            RenderSettings.ambientLight = Color.black;

            ShowDropInAnimation();

        // }
        // else{ //not first time. spawn pet as usual
        //     // TEMPORARY spawn the pet in location
        //     GameObject goPet = Instantiate(petObject, new Vector3(0f, 0.35f, 23f), Quaternion.identity) as GameObject;
        //     goPet.name = "SpritePet";

        //     //continue normal gui stuff
        //     if(finishCheckingForFirstTime != null) finishCheckingForFirstTime();

        //     // Kill itself + related objects if not first time
        //     Destroy(eggObject);
        //     Destroy(nestObject);
        //     Destroy(gameObject);
        //     Destroy(firstTimeChoosePanel);
        // }
    }

    void ShowDropInAnimation(){
        // Splash finished, Drop down the title and the egg sprite, only called once
        popupTitle.GetComponent<MoveTweenToggle>().Show();

        Hashtable optional = new Hashtable();
        optional.Add("ease", LeanTweenType.easeOutBounce);
        LeanTween.move(eggObject, eggSpritePosition, 1.5f, optional);
    }

    void Update(){

        //TODO-s Optimize this for touch? / ABSTRACT TO CAMERAMOVE?? perhaps not for coherency
        if(Input.GetMouseButtonUp(0)){
            Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(myRay,out hit))
            {
                if(hit.collider.name == "SpriteEgg" && eggClicked == false)
                {
                    eggClicked = true;
                    CameraTransform(finalPosition,finalFaceDirection);
                    isZoomed = true;
                    HideTitle();
                    ShowChooseGUI();
                }
            }
        }
    }

    void ShowChooseGUI(){
        firstTimeChoosePanel.GetComponent<MoveTweenToggle>().Show(smooth);
    }

    void HideChooseGUI(){
        firstTimeChoosePanel.GetComponent<MoveTweenToggle>().Hide(smooth);
        RenderSettings.ambientLight = currentRenderColor;   // lerp this
        HelperFinishEditPet();
    }

    // Callback for closing edit panel
    private void HelperFinishEditPet(){
        DataManager.PetName = petName;
        DataManager.PetColor = petColor;
        DataManager.TurnFirstTimeOff();

        // // Spawn pet object
        // // GameObject goPet = Instantiate(petObject, new Vector3(0f, -2.87f, -10f), Quaternion.identity) as GameObject;
        // GameObject goPet = Instantiate(petObject, new Vector3(0f, 0.35f, 23f), Quaternion.identity) as GameObject;
        // goPet.name = "SpritePet";

        // Start normal GUI stuff
        // if(finishHatchCallBack != null) finishHatchCallBack();

        // Commit seppuku
        // Destroy(eggObject);
        // Destroy(nestObject);
        // Destroy(gameObject);
        // Destroy(firstTimeChoosePanel, 3.0f);
        // Application.LoadLevel("NewBedRoom");
    }

    void ButtonClicked_Blue(){
        eggSpriteScript.SetSprite("eggBlueChoose");
        petColor = "whiteBlue";
    }
    void ButtonClicked_Green(){
        eggSpriteScript.SetSprite("eggGreenChoose");
        petColor = "whiteGreen";
    }
    void ButtonClicked_Yellow(){
        eggSpriteScript.SetSprite("eggYellowChoose");
        petColor = "whiteYellow";
    }
    void ButtonClicked_Red(){
        eggSpriteScript.SetSprite("eggRedChoose");
        petColor = "whiteRed";
    }
    void ButtonClicked_Purple(){
        eggSpriteScript.SetSprite("eggPurpleChoose");
        petColor = "whitePurple";
    }
    void ButtonClicked_Finish(){
        petName = nameField.text;
        if(isZoomed){
            ZoomOutMove();
            isZoomed = false;
            HideChooseGUI();
        }
    }

    void HideTitle(){
        popupTitle.GetComponent<MoveTweenToggle>().Hide();
        Destroy(popupTitle, 3.0f);
    }

    void CameraTransform (Vector3 newPosition, Vector3 newDirection){
        Hashtable optional = new Hashtable();
        optional.Add("ease", LeanTweenType.easeInOutQuad);
        LeanTween.move(mCamera, newPosition, smooth, optional);
        LeanTween.rotate(mCamera, newDirection, smooth, optional);
    }

    void ZoomOutMove(){
        CameraTransform(initPosition,initFaceDirection);
        Invoke("LoadScene", 1);
    }

    private void LoadScene(){
        Application.LoadLevel("NewBedRoom");
    }
}
