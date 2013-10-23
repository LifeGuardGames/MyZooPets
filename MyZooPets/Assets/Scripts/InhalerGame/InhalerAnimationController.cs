using UnityEngine;
using System.Collections;

public class InhalerAnimationController : LgCharacterAnimator {
     // key of the pet's "species" -- i.e. what kind of pet it is
    // this will eventually be set in save data probably
    public string strKeySpecies;
    public string GetSpeciesKey() {
        return strKeySpecies;   
    }
    
    // key of the pet's color
    // this will eventually be set in save data probably
    public string strKeyColor;
    public string GetColorKey() {
        return strKeyColor; 
    }

    // key that tells where the animation is used -- i.e runner, bedroom, trigger ninja
    public string strKeyAnimType;
    public string GetAnimTypeKey(){
        return strKeyAnimType;
    } // key of the pet's "species" -- i.e. what kind of pet it is

    new void Start(){
        // set the LWFAnimator loading data based on the pet's attributes
        string strSpecies = GetSpeciesKey();
        string strColor = GetColorKey();
        animName = strSpecies + strColor;
        folderPath = "LWF/" + strKeyAnimType + "/" + animName + "/";
        
        // only call this AFTER we have set our loading data
        base.Start();
        // Flip(true);
        PlayClip("exhale");
    }

    private void Exhale(){
        PlayClip("exhale");
    }

    private void Inhale(){
        PlayClip("inhale");
    }
}
