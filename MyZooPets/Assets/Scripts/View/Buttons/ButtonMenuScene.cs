using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonStore
// Button that opens the store UI.
//---------------------------------------------------

public class ButtonMenuScene : LgButton {
    
    //---------------------------------------------------
    // ProcessClick()
    //---------------------------------------------------   
    protected override void ProcessClick() {
        GetComponent<SceneTransition>().StartTransition();
    }
}
