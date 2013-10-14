using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Controls the parent portal template
public class ParentPortalUIManager : Singleton<ParentPortalUIManager> {
    public static Action onOkButtonClicked;  
    public static Action onCancelButtonClicked; 
    public List<GameObject> parentPortalContents = new List<GameObject>();
    public UILabel generalMessage; //Use this label to display positive feedback (ex. your account has been created)

    public void OkButtonClicked(){
        if(onOkButtonClicked != null)
            onOkButtonClicked();
    }

    public void CancelButtonClicked(){
        if(onCancelButtonClicked != null)
            onCancelButtonClicked();
    }

    public void DisplayGeneralMessage(string message){
        generalMessage.text = Localization.Localize(message);
        Invoke("HideGeneralMessage", 3f);
    }

    private void HideGeneralMessage(){
        generalMessage.text = "";
    }

    public void ShowPortalSetup(GameObject currentContent){
        ShowContent("PortalSetup", currentContent);
    }

    public void ShowPortalLogin(GameObject currentContent){
        ShowContent("PortalLogin", currentContent);
    }

    public void ShowPortalReset(GameObject currentContent){
        ShowContent("PortalReset", currentContent);
    }

    public void ShowPortalRecovery(GameObject currentContent){
        ShowContent("PortalRecovery", currentContent);
    }

    //Display new content in parent portal template and hide current content
    private void ShowContent(string newContentName, GameObject currentContent){
        GameObject newContent = parentPortalContents.Find(content => content.name == newContentName);

        if(currentContent != null)
            currentContent.SetActive(false);
        newContent.SetActive(true);
    }
}
