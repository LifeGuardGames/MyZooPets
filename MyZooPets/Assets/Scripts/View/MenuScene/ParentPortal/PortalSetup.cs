using UnityEngine;
using System.Collections;

public class PortalSetup : PortalContent {
    public GameObject emailInput;
    public GameObject pinInput;
    public GameObject pinConfirmInput; 
    public GameObject emailError;
    public GameObject pinError;
    public GameObject pinConfirmError;

    protected override void OkButtonClicked(){
        string email = emailInput.GetComponent<UIInput>().text;
        string pin = pinInput.GetComponent<UIInput>().text;
        string pinConfirm = pinConfirmInput.GetComponent<UIInput>().text;

        Hashtable result = ParentPortalLogic.Instance.VerifyAndSaveSetupInput(email, pin, pinConfirm);

        if(result!= null){
            //Reset or disable error messages
            emailError.SetActive(false);
            pinError.SetActive(false);
            pinConfirmError.SetActive(false);

            if(result.Contains("Completed")){
                bool valid = (bool)result["Completed"];

                if(!valid){ //There are errors. Display them
                    if(result.Contains("EmailErrorMsg")){
                        string emailErrorMsg = (string)result["EmailErrorMsg"];
                        emailError.SetActive(true);
                        emailError.GetComponent<UILabel>().text = emailErrorMsg;
                    }
                    if(result.Contains("PinErrorMsg")){
                        string pinErrorMsg = (string)result["PinErrorMsg"];
                        pinError.SetActive(true);
                        pinError.GetComponent<UILabel>().text = pinErrorMsg;
                    }
                    if(result.Contains("ConfirmPinErrorMsg")){
                        string confirmPinErrorMsg = (string)result["ConfirmPinErrorMsg"];
                        pinConfirmError.SetActive(true);
                        pinConfirmError.GetComponent<UILabel>().text = confirmPinErrorMsg;
                    }
                }else{ //Setup is successful. redirect to login page

                    ParentPortalUIManager.Instance.ShowPortalLogin(this.gameObject);
                }
            }
        }
    }

    protected override void CancelButtonClicked(){

    }
}
