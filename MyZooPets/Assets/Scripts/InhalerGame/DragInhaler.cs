using UnityEngine;
using System.Collections;

#pragma warning disable 0618

/*
    AdvairDrag.cs and RescueDrag.cs inherit from this class
    Inherit from this class to be able to drag a large inhaler, which changes to a small
    inhaler icon while dragging, to the pet
*/
public class DragInhaler : MonoBehaviour {
    public GameObject smallInhalerPrefab; //prefab
    protected string spriteName; //Sprite name in the sprite collection
    protected int advairStepID; //step id correlating to Inhaler logic
    private GameObject smallInhaler; //Reference to the gameobject instantiated
    private GameObject petSprite;
    private bool firstTouchOnObject = false; //User needs to touch the object first before
                                            //touch events can be handled
    protected virtual void Awake(){
        petSprite = GameObject.Find("PetSprite");
    }

    void Start(){
       GetComponent<Collider>().enabled = false;
    }

    void Update(){
        if(InhalerGameManager.Instance.CurrentStep != advairStepID) return;
        if(!GetComponent<Collider>().enabled) GetComponent<Collider>().enabled = true;
        if(Input.touchCount > 0){
            Touch touch = Input.touches[0];
            switch(touch.phase){
                case TouchPhase.Began:
                    //Condition that terminate touch
                    if(!InhalerUtility.IsTouchingObject(touch, gameObject)) return;

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
                    if(InhalerUtility.IsTouchingObject(touch, petSprite)){ //Check if advair drop on pet
                        if(InhalerGameManager.Instance.IsCurrentStepCorrect(advairStepID)){
                            InhalerGameManager.Instance.NextStep();
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

    //Show the inhaler plus all its children.
    private void ShowLargeInhaler(){
        // gameObject.SetActive(true);
        foreach(Transform tran in transform){
            tran.gameObject.SetActive(true);
        }
    }

    // Hide the inhaler plus all its children.
    private void HideLargeInhaler(){
        // gameObject.SetActive(false);
        foreach(Transform tran in transform){
            tran.gameObject.SetActive(false);
        }
    }

    /*
        Show small inhaler. If small inhaler is not present in the hierarchy
        a new one is instantiated else just activate the game object
    */
    private void ShowSmallInhaler(Vector3 pos){
        if(smallInhaler == null){
            float posZ = smallInhalerPrefab.transform.position.z;
            smallInhaler = (GameObject) Instantiate(smallInhalerPrefab);
            smallInhaler.transform.position = new Vector3(0, 0, posZ);
        }else{
            smallInhaler.active = true;            
        }
        MoveSmallInhalerToTouchPos(pos);
    }

    //Hide by deactivating the game object
    private void HideSmallInhaler(){
        smallInhaler.active = false;
    }

    //Move inhaler sprite to where user is touching the screen
    private void MoveSmallInhalerToTouchPos(Vector3 pos){
        float posZ = smallInhaler.transform.position.z;
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(
            new Vector3(pos.x, pos.y, posZ));
        smallInhaler.transform.position = new Vector3(touchPos.x, touchPos.y, posZ);
    }
}
