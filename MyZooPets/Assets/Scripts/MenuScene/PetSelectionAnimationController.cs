using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PetSelectionAnimationController : LWFAnimator {
    // // key that tells where the animation is used -- i.e runner, bedroom, trigger ninja
    public string animType;

    protected override void Start(){
        	MutableDataPetMenuInfo petMenuInfo = DataManager.Instance.MenuSceneData;
//        string petID = this.transform.parent.parent.parent.name;
//        if(petMenuInfoDict.ContainsKey(petID)){
            string speciesColor = petMenuInfo.PetSpecies + petMenuInfo.PetColor;
            // showCaseAnimator.GetComponent<LWFAnimator>().animName = speciesColor; 
            animName = speciesColor;
//        }

        folderPath = "LWF/" + animType + "/" + animName + "/";
        base.Start();
    }
}
