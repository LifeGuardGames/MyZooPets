using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Controls the parent portal template
public class ParentPortalUIManager : Singleton<ParentPortalUIManager> {
    public static Action onOkButtonClicked;
    public static Action onCancelButtonClicked;
    public List<GameObject> parentPortalContents = new List<GameObject>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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
        GameObject portalSetup =  parentPortalContents.Find(content => content.name == "PortalSetup");

        if(currentContent != null)
            currentContent.SetActive(false);
        portalSetup.SetActive(true);
    }

    //Turn off the current content and display portal login
    public void ShowPortalLogin(GameObject currentContent){
        GameObject portalLogin = parentPortalContents.Find(content => content.name == "PortalLogin");

        if(currentContent != null)
            currentContent.SetActive(false);
        portalLogin.SetActive(true);
    }
}
