using UnityEngine;
using System.Collections;

//Parent portal content parent class
public class PortalContent : MonoBehaviour {

    protected virtual void OnEnable(){
        ParentPortalUIManager.onOkButtonClicked = OkButtonClicked;
        ParentPortalUIManager.onCancelButtonClicked = CancelButtonClicked;
    }

    protected virtual void OnDisable(){
        ParentPortalUIManager.onOkButtonClicked = null; 
        ParentPortalUIManager.onCancelButtonClicked = null;
    }

    protected virtual void OkButtonClicked(){}
    protected virtual void CancelButtonClicked(){}
}