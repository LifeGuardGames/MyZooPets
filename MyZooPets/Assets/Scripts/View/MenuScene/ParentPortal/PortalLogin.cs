using UnityEngine;
using System.Collections;

public class PortalLogin : PortalContent {
    public GameObject pinInput;
    public GameObject pinError;

    protected override void OkButtonClicked(){
        string inputPin = pinInput.GetComponent<UIInput>().text;

        Hashtable result = ParentPortalLogic.Instance.VerifyLoginPin(inputPin);
        
        if(result != null){
            pinError.SetActive(false);

            if(result.Contains("Completed")){
                bool validPin = (bool)result["Completed"];

                if(!validPin){ //There are errors. Display them
                    if(result.Contains("InputPinErrorMsg")){
                        string errorMsg = (string)result["InputPinErrorMsg"];
                        pinError.SetActive(true);
                        pinError.GetComponent<UILabel>().text = errorMsg;
                    }
                }else{ //Login successful. Redirect to parent portal scene
                    //Load scene here

                }
            }
        }   
    }

    protected override void CancelButtonClicked(){

    }

    public void ResetButtonClicked(){
        ParentPortalUIManager.Instance.ShowPortalReset(this.gameObject);
    }

    public void RecoveryButtonClicked(){
        ParentPortalUIManager.Instance.ShowPortalRecovery(this.gameObject);
    }
}
