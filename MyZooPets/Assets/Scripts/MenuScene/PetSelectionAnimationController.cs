using UnityEngine;
using System.Collections;

public class PetSelectionAnimationController : LgCharacterAnimator {
    // key that tells where the animation is used -- i.e runner, bedroom, trigger ninja
    public string strKeyAnimType;
    public string GetAnimTypeKey(){
        return strKeyAnimType;
    } // key of the pet's "species" -- i.e. what kind of pet it is

    new void Start(){
        // string strSpecies = "Basic";
        // string strColor = DataManager.Instance.GameData.PetInfo.PetColor;
        //DataManager.Instance.GetPetSpeciesColor("");
        // animName = strSpecies + strColor;
        folderPath = "LWF/" + strKeyAnimType + "/" + animName + "/";

        base.Start();
    }
}
