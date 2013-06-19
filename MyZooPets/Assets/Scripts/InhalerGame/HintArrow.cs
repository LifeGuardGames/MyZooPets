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


    // Use this for initialization
    void Start () {
        renderer.enabled = false;

        if (specificInhalerTypeString == "rescue")
            specificInhalerType = InhalerType.Rescue;
        else if (specificInhalerTypeString == "advair"){
            specificInhalerType = InhalerType.Advair;
        }
        else {
            hasSpecificType = false;
        }
    }

    // Update is called once per frame
    void Update () {
        if (hasSpecificType && InhalerLogic.CurrentInhalerType != specificInhalerType){
            return;
        }

        if (InhalerLogic.CurrentStep == showOnStep){
            renderer.enabled = true;
        }
        else
        {
            renderer.enabled = false;
        }
    }
}
