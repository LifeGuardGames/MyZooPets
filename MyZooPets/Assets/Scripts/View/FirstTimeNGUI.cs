using UnityEngine;
using System.Collections;

/// <summary>
/// First time GUI.
/// Stuffed everything that needs to be done in the first run here.
/// If its not the first time the game is run, this will delete itself.
/// </summary>

public class FirstTimeNGUI : SingletonUI<FirstTimeNGUI> {

    public GameObject eggObject;
    public GameObject nestObject;
    public GameObject firstTimeChoosePanel;
    public GameObject popupTitle;
    public GameObject mCamera;
    public GameObject loadingScreen;
    
    public UILabel nameField;
    
    // Camera moving
    private float smooth = 1.0f;
    private bool isZoomed = false;
    private Vector3 initPosition = new Vector3(0.1938391f, 11.47f, 2.83f);
    private Vector3 initFaceDirection = new Vector3(11.3f, 0, 0);
    private Vector3 finalPosition = new Vector3(4.7f, 7.08f, 12.23f);
    private Vector3 finalFaceDirection = new Vector3(11.3f, 0, 0);

    private Color currentRenderColor;
    private bool eggClicked = false;
    private bool finishClicked = false;
    private Vector3 eggSpritePosition = new Vector3(0f, 2.8f, 22.44f);
    private tk2dSprite eggSpriteScript;
    private string petName;
    private string petColor;

    void Start(){
            eggSpriteScript = eggObject.GetComponent<tk2dSprite>();
            currentRenderColor = RenderSettings.ambientLight;
            RenderSettings.ambientLight = Color.black;

            ShowDropInAnimation();
    }
	
	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------	
	protected override void _OpenUI(){
       	eggClicked = true;
        CameraTransform(finalPosition,finalFaceDirection);
        isZoomed = true;
        HideTitle();
        ShowChooseGUI();	
	}	
	
	/*
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
    */

    private void ShowDropInAnimation(){
        // Splash finished, Drop down the title and the egg sprite, only called once
        popupTitle.GetComponent<MoveTweenToggle>().Show();

        Hashtable optional = new Hashtable();
        optional.Add("ease", LeanTweenType.easeOutBounce);
        LeanTween.move(eggObject, eggSpritePosition, 1.5f, optional);
    }
    
    private void ShowChooseGUI(){
        firstTimeChoosePanel.GetComponent<MoveTweenToggle>().Show(smooth);
    }

    private void HideChooseGUI(){
        firstTimeChoosePanel.GetComponent<MoveTweenToggle>().Hide(smooth);
        RenderSettings.ambientLight = currentRenderColor;   // lerp this
        HelperFinishEditPet();
    }

    // Callback for closing edit panel
    public void HelperFinishEditPet(){
        DataManager.Instance.PetName = petName;
        DataManager.Instance.PetColor = petColor;
        DataManager.Instance.TurnFirstTimeOff();
    }

    public void ButtonClicked_Blue(){
        if (!finishClicked){
            eggSpriteScript.SetSprite("eggBlueChoose");
            petColor = "whiteBlue";
        }
    }
    public void ButtonClicked_Green(){
        if (!finishClicked){
            eggSpriteScript.SetSprite("eggGreenChoose");
            petColor = "whiteGreen";
        }
    }
    public void ButtonClicked_Yellow(){
        if (!finishClicked){
            eggSpriteScript.SetSprite("eggYellowChoose");
            petColor = "whiteYellow";
        }
    }
    public void ButtonClicked_Red(){
        if (!finishClicked){
            eggSpriteScript.SetSprite("eggRedChoose");
            petColor = "whiteRed";
        }
    }
    public void ButtonClicked_Purple(){
        if (!finishClicked){
            eggSpriteScript.SetSprite("eggPurpleChoose");
            petColor = "whitePurple";
        }
    }
    public void ButtonClicked_Finish(){
        if (!finishClicked){
            finishClicked = true;
            petName = nameField.text;
            if(isZoomed){
                ZoomOutMove();
                isZoomed = false;
                HideChooseGUI();
            }
        }
    }

    private void HideTitle(){
        popupTitle.GetComponent<MoveTweenToggle>().Hide();
        Destroy(popupTitle, 3.0f);
    }

    private void CameraTransform (Vector3 newPosition, Vector3 newDirection){
        Hashtable optional = new Hashtable();
        optional.Add("ease", LeanTweenType.easeInOutQuad);
        LeanTween.move(mCamera, newPosition, smooth, optional);
        LeanTween.rotate(mCamera, newDirection, smooth, optional);
    }

    private void ZoomOutMove(){
        CameraTransform(initPosition,initFaceDirection);
        Invoke("LoadScene", 1);
    }

    private void LoadScene(){
        Application.LoadLevel("NewBedRoom");
    }
}
