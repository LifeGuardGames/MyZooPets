using UnityEngine;
using System.Collections;
using System;

public class DojoUIManager : MonoBehaviour {
    //======================Event=============================
    public static event EventHandler<EventArgs> OnDojoDoorClosed;
    //=======================================================
    public GUISkin defaultSkin;
    public Texture2D backButton;
    public GUIStyle blankButtonStyle;

    private bool isActive = false;

    void OnGUI(){
        GUI.skin = defaultSkin;
        if(isActive && !ClickManager.isClickLocked){ // checking isClickLocked because trophy shelf back button should not be clickable if there is a notification
            if(GUI.Button(new Rect(10, 10, backButton.width, backButton.height), backButton, blankButtonStyle)){
                if(OnDojoDoorClosed != null){
                    OnDojoDoorClosed (this, EventArgs.Empty);
                }else{
                    Debug.LogError("OnDojoDoorClosed is null");
                }
                isActive = false;
            }
        }
    }

    public void DojoDoorClicked(){
        if(!isActive) isActive = true;
    }
}
