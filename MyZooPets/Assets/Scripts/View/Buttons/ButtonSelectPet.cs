using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonEgg
// Button for the egg that appears when users first
// hatch their pet.
//---------------------------------------------------
public class ButtonSelectPet : LgButton{
    
    //---------------------------------------------------
    // ProcessClick()
    //---------------------------------------------------   
    protected override void ProcessClick(){
		if(CustomizationUIManager.Instance.canClickHatchPet){	// Third customization ui
			CustomizationUIManager.Instance.ThirdUIEggTapped();
		}
		else{
			SelectionUIManager.Instance.PetSelected(this.gameObject);	// First call into customization
		}
    }
}