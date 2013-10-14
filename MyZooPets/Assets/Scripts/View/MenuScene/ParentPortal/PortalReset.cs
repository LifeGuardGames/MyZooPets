using UnityEngine;
using System.Collections;

public class PortalReset : PortalContent {
    public GameObject oldPinInput;
    public GameObject newPinInput;
    public GameObject newPinConfirmInput;
    public GameObject oldPinError;
    public GameObject newPinError;
    public GameObject newPinConfirmError;

    protected override void OkButtonClicked(){
        string oldPin = oldPinInput.GetComponent<UIInput>().text;
        string newPin = newPinInput.GetComponent<UIInput>().text;
        string newPinConfirm = newPinConfirmInput.GetComponent<UIInput>().text;

        Hashtable result = ParentPortalLogic.Instance.VerifyResetInput(oldPin, newPin, newPinConfirm);

        if(result != null){
            oldPinError.SetActive(false);
            newPinError.SetActive(false);
            newPinConfirmError.SetActive(false);

            if(result.Contains("Completed")){
                bool resetValid = (bool)result["Completed"];

                if(!resetValid){
                    if(result.Contains("InputPinErrorMsg")){
                        string inputPinErrorMsg = (string)result["InputPinErrorMsg"];
                        oldPinError.SetActive(true);
                        oldPinError.GetComponent<UILabel>().text = inputPinErrorMsg;
                    }
                    if(result.Contains("PinErrorMsg")){
                        string pinErrorMsg = (string)result["PinErrorMsg"];
                        newPinError.SetActive(true);
                        newPinError.GetComponent<UILabel>().text = pinErrorMsg;
                    }
                    if(result.Contains("ConfirmPinErrorMsg")){
                        string confirmPinErrorMsg = (string)result["ConfirmPinErrorMsg"];
                        newPinConfirmError.SetActive(true);
                        newPinConfirmError.GetComponent<UILabel>().text = confirmPinErrorMsg;
                    }
                }else{ //Reset is successful. redirect to login page

                    ParentPortalUIManager.Instance.ShowPortalLogin(this.gameObject);
                }
            }
        }

    }

    protected override void CancelButtonClicked(){

    }
}
