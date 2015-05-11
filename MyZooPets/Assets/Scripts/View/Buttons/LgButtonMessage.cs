using UnityEngine;
using System.Collections;

//---------------------------------------------------
// Similar to NGUI's UIButtonMessage, but this class
// checks the clickmanager before processing the button
// click. It also has a more robust error handling
//---------------------------------------------------

public class LgButtonMessage : LgButton {
    public GameObject target;
    public string functionName;
    
    //---------------------------------------------------
    // ProcessClick()
    //---------------------------------------------------   
    protected override void ProcessClick(){
        Send();
    }

    private void Send(){
        if(string.IsNullOrEmpty(functionName)){
            Debug.LogError("LgButtonMessage functionName in parent (" + gameObject + ") cannot be null");
            return;
        }

        if(target == null){
            Debug.LogError("LgButtonMessage target in parent (" + gameObject + ") cannot be null", this);
            return;
        }

        target.SendMessage(functionName, gameObject, SendMessageOptions.RequireReceiver);
    }
}