using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InhalerAnimationController : LgCharacterAnimator {
    public static EventHandler<EventArgs> OnAnimDone;

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

    private Queue<string> animQueue; //Queue up the animation that needs to be played
    private string lastPlayedAnimation; //Animation clip name that was last played

    new void Start(){
       // set the LWFAnimator loading data based on the pet's attributes
        string strSpecies = DataManager.Instance.GameData.PetInfo.PetSpecies; 

        // string strColor = GetColorKey();
        string strColor = DataManager.Instance.GameData.PetInfo.PetColor;
        
        animName = strSpecies + strColor;
        folderPath = "LWF/" + strKeyAnimType + "/" + animName + "/";
        
        // only call this AFTER we have set our loading data
        base.Start();

        animQueue = new Queue<string>();
        Idle();
    }

    new void Update(){
        //Call base update so that LWF can do its thing
        base.Update(); 

        //player the animation in the queue if not animation is currently playing
        if(!IsAnimating() && animQueue.Count > 0){
            string clipName = animQueue.Dequeue();
            PlayClip(clipName);
        }
    }

    //queue up exhale animation
    public void Exhale(){
        lastPlayedAnimation = "exhale";
        animQueue.Enqueue("exhale");
    }

    //queue up inhaler animation
    public void Inhale(){
        lastPlayedAnimation = "inhale";
        animQueue.Enqueue("inhale");
    }

    //queue up idle animation
    private void Idle(){
        //If queue is not empty, don't add another idle animation
        if(animQueue.Count > 0)
            return;

        lastPlayedAnimation = "idle";
        animQueue.Enqueue("idle");
    }

    //Implement callback for when a clip is finish playing
    protected override void _ClipFinished(){
        if(lastPlayedAnimation != "idle"){
            // Idle();

            if(OnAnimDone != null)
                OnAnimDone(this, EventArgs.Empty);
        }
    }
}
