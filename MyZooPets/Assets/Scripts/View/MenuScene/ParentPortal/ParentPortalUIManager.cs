using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Controls the parent portal template
public class ParentPortalUIManager : Singleton<ParentPortalUIManager> {
    public static Action onOkButtonClicked;
    public static Action onCancelButtonClicked;
    public List<GameObject> parentPortalContents = new List<GameObject>();

    public void OkButtonClicked(){
        if(onOkButtonClicked != null)
            onOkButtonClicked();
    }

    public void CancelButtonClicked(){
        if(onCancelButtonClicked != null)
            onCancelButtonClicked();
    }

    //Turn off the current content and display portal setup 
    public void ShowPortalSetup(GameObject currentContent){
        ShowContent("PortalSetup", currentContent);
    }

    //Turn off the current content and display portal login
    public void ShowPortalLogin(GameObject currentContent){
        ShowContent("PortalLogin", currentContent);
    }

    public void ShowPortalReset(GameObject currentContent){
        ShowContent("PortalReset", currentContent);
    }

    public void ShowPortalRecovery(GameObject currentContent){
        ShowContent("PortalRecovery", currentContent);
    }

    private void ShowContent(string newContentName, GameObject currentContent){
        GameObject newContent = parentPortalContents.Find(content => content.name == newContentName);

        if(currentContent != null)
            currentContent.SetActive(false);
        newContent.SetActive(true);
    }
}
