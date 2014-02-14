using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PetSelectionAnimationController : LWFAnimator {
    // // key that tells where the animation is used -- i.e runner, bedroom, trigger ninja
    public string animType;

    protected override void Start(){
        Dictionary<string, MutableData_PetMenuInfo> petMenuInfoDict = DataManager.Instance.MenuSceneData;
        string petID = this.transform.parent.parent.parent.name;
        if(petMenuInfoDict.ContainsKey(petID)){
            string speciesColor = petMenuInfoDict[petID].PetSpecies + petMenuInfoDict[petID].PetColor;
            // showCaseAnimator.GetComponent<LWFAnimator>().animName = speciesColor; 
            animName = speciesColor;
        }

        folderPath = "LWF/" + animType + "/" + animName + "/";
        base.Start();
    }
}
