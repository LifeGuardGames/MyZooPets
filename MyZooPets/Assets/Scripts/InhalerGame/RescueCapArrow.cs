using UnityEngine;
using System.Collections;

public class RescueCapArrow : MonoBehaviour {
    // Use this for initialization
    void Start () {
        renderer.enabled = false;
    }

    // Update is called once per frame
    void Update () {
        if (InhalerLogic.CurrentInhalerType != InhalerType.Rescue){
            return;
        }

        if (InhalerLogic.CurrentStep == 1){
            renderer.enabled = true;
        }
        else
        {
            renderer.enabled = false;
        }
    }
}
