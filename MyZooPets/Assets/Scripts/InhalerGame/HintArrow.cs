using UnityEngine;
using System.Collections;

/*
    This generic class controls the hint arrows for inhaler parts
*/
public class HintArrow : MonoBehaviour {

    // To limit this hint arrow to show only for a specific type of inhaler,
    // set specificInhalerType to either "advair" or "rescue".
    // All other values will be ignored.
    public string specificInhalerTypeString = "none";

    // Set this to the step that the hint arrow should be shown on.
    public int showOnStep = 0;

    private bool hasSpecificType = true;
    private InhalerType specificInhalerType;


	public GameObject optionalTextPrefab;
	private GameObject optionalTextReference;

    void Start(){
        renderer.enabled = false;

        if(specificInhalerTypeString == "rescue")
            specificInhalerType = InhalerType.Rescue;
        else if(specificInhalerTypeString == "advair"){
            specificInhalerType = InhalerType.Advair;
        }
        else{
            hasSpecificType = false;
        }
    }

    void Update(){
        if(hasSpecificType && InhalerLogic.Instance.CurrentInhalerType != specificInhalerType){
            return;
        }
        if(InhalerLogic.Instance.CurrentStep == showOnStep && InhalerGameManager.Instance.ShowHint){
			if(optionalTextPrefab != null && optionalTextReference == null){
				optionalTextReference = Instantiate(optionalTextPrefab) as GameObject;
			}
			renderer.enabled = true;
        }
        else{
			if(optionalTextReference != null){
				Destroy(optionalTextReference);
			}
            renderer.enabled = false;
        }
    }
}
