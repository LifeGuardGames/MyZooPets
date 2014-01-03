using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerAnimationController : LgCharacterAnimator {
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
    }

    new void Start(){
        // set the LWFAnimator loading data based on the pet's attributes
        string strSpecies = DataManager.Instance.GameData.PetInfo.PetSpecies; 

        // string strColor = GetColorKey();
        string strColor = DataManager.Instance.GameData.PetInfo.PetColor;
        
        animName = strSpecies + strColor;
        folderPath = "LWF/" + strKeyAnimType + "/" + animName + "/";
        
        // only call this AFTER we have set our loading data
        base.Start();
        Flip(true);

        RunnerGameManager.OnStateChanged += GameStateChanged;
    }

    new void OnDestroy(){
        RunnerGameManager.OnStateChanged -= GameStateChanged;

        base.OnDestroy();
    }

    private void GameStateChanged(object sender, GameStateArgs args){
        MinigameStates gameState = args.GetGameState();
        if(gameState == MinigameStates.Paused){
            if(resumeCount == 1)
                Pause();
        }else if(gameState == MinigameStates.Playing){
            if(resumeCount == 0)
                Resume();
        }
    }

    void OnGrounded(){
        Run();
    }

    void OnFalling(){
        Fall();
    }

    void OnJumping(){
        Jump();
    }

    private void Jump(){
        PlayClip("jump");
    }

    private void Run(){
        PlayClip("run");
    }

    private void Fall(){
        PlayClip("fall");
    }

    // void OnGUI(){
    //     if(GUI.Button(new Rect(0, 0, 100, 100), "pause anim")){
    //         Pause();
    //     }
    // }

}
