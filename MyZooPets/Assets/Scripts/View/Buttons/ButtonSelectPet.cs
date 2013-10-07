using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonEgg
// Button for the egg that appears when users first
// hatch their pet.
//---------------------------------------------------

public class ButtonSelectPet : LgButton {
    
    //---------------------------------------------------
    // ProcessClick()
    //---------------------------------------------------   
    protected override void ProcessClick() {
		CustomizationUIManager.Instance.HideTitle();
        SelectionUIManager.Instance.PetSelected(this.gameObject);
    }

}