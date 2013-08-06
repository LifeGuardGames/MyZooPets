using UnityEngine;
using System.Collections;

public class HintArrow : MonoBehaviour {

    // To limit this hint arrow to show only for a specific type of inhaler,
    // set specificInhalerType to either "advair" or "rescue".
    // All other values will be ignored.
    public string specificInhalerTypeString = "none";

    // Set this to the step that the hint arrow should be shown on.
    public int showOnStep = 0;

    private bool hasSpecificType = true;
    private InhalerType specificInhalerType;

    private InhalerGameManager inhalerGameManager;

	public GameObject optionalTextPrefab;
	private GameObject optionalTextReference;

    void Start(){
        renderer.enabled = false;
        inhalerGameManager = GameObject.Find("InhalerGameManager").GetComponent<InhalerGameManager>();

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
        if(hasSpecificType && InhalerLogic.CurrentInhalerType != specificInhalerType){
            return;
        }
        if(InhalerLogic.CurrentStep == showOnStep && inhalerGameManager.ShowHint){
			if(optionalTextPrefab != null && optionalTextReference == null){
				optionalTextReference = Instantiate(optionalTextPrefab) as GameObject;
			}
			renderer.enabled = true;
        }
        else{
			if(optionalTextReference != null){
				Debug.Log("bye");
				Destroy(optionalTextReference);
			}
            renderer.enabled = false;
        }
    }
}
