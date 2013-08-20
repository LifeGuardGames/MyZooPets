using UnityEngine;
using System.Collections;

/*
    Advair Draggable Body (Advair Step 4).

    This listens for the user's touch on the body, and the inhaler "becomes" a smaller advair inhaler
    when dragged to the pet.

    How this happens is, when the user's touch starts on the inhaler, the original body is hidden.
    At the same time, a GUI texture is drawn under the user's touch position.

    If the inhaler is dragged to and released over the pet, it will disappear, and the
    step is completed.
    If it is released anywhere else, the original advair inhaler will reappear.
*/
public class AdvairDrag : MonoBehaviour{
    public GameObject advairSmallPrefab; //prefab
    private GameObject advairSmall; //Reference to the gameobject instantiated
    private GameObject petSprite;
    private const string ADVAIR_SMALL = "advair_small";
    private int advairStepID = 3; //step id correlating to Inhaler logic
    private bool firstTouchOnObject = false; //User needs to touch the object first before
                                            //other touch logic can work
    void Awake(){
        petSprite = GameObject.Find("PetSprite");
        advairSmall = GameObject.Find("AdvairSmall");
    }

    void Start(){
       collider.enabled = false;
    }

    void Update(){
        if (InhalerLogic.Instance.CurrentStep != advairStepID) return;
        if(!collider.enabled) collider.enabled = true;
        if(Input.touchCount > 0){
            Touch touch = Input.touches[0];
            switch(touch.phase){
                case TouchPhase.Began:
                    //Condition that terminate touch
                    if(!IsTouchingObject(touch, gameObject)) return;

                    firstTouchOnObject = true;
                    HideLargeInhaler();    
                    ShowSmallInhaler(touch.position);
                break;
                case TouchPhase.Moved:
                    //Condition that terminate touch
                    if(!firstTouchOnObject) return;

                    //Move the small advair icon to the finger position
                    MoveSmallInhalerToTouchPos(touch.position);
                break;
                case TouchPhase.Ended:
                    //Condition that terminate touch
                    if(!firstTouchOnObject) return;

                    firstTouchOnObject = false;
                    if(IsTouchingObject(touch, petSprite)){ //Check if advair drop on pet
                        if(InhalerLogic.Instance.IsCurrentStepCorrect(advairStepID)){
                            InhalerLogic.Instance.NextStep();
                            HideSmallInhaler();
                        }
                    }else{
                        ShowLargeInhaler(); 
                        HideSmallInhaler();
                    }
                break;
            }
        }
    }

    //Cast a ray to test if the touch position is ontop of the object
    private bool IsTouchingObject(Touch touch, GameObject target){
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit ;
        bool retVal = false;
        if (Physics.Raycast (ray, out hit)) {
            if(hit.collider.gameObject == target){
                retVal = true;;
            }
        }
        return retVal;
    }

    //Show the inhaler plus all its children.
    private void ShowLargeInhaler(){
        Component[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers) {
            r.enabled = true;
        }
    }

    // Hide the inhaler plus all its children.
    private void HideLargeInhaler(){
        Component[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers) {
            r.enabled = false;
        }
    }

    private void ShowSmallInhaler(Vector3 pos){
        if(advairSmall == null){
            float posZ = advairSmallPrefab.transform.position.z;
            advairSmall = (GameObject) Instantiate(advairSmallPrefab);
            advairSmall.transform.position = new Vector3(0, 0, posZ);
        }else{
            advairSmall.active = true;            

        }
        MoveSmallInhalerToTouchPos(pos);
    }

    private void HideSmallInhaler(){
        advairSmall.active = false;
    }

    //Move inhaler sprite to where user is touching the screen
    private void MoveSmallInhalerToTouchPos(Vector3 pos){
        float posZ = advairSmall.transform.position.z;
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(
            new Vector3(pos.x, pos.y, posZ));
        advairSmall.transform.position = new Vector3(touchPos.x, touchPos.y, posZ);
    }
}