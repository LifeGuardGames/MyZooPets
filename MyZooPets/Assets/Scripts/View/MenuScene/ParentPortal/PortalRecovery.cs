using UnityEngine;
using System.Collections;

public class PortalRecovery : PortalContent {
    public GameObject emailInput;
    public GameObject emailError;

    protected override void OnEnable(){
        base.OnEnable();

        emailInput.GetComponent<UIInput>().text = ParentPortalLogic.Instance.ParentEmail;;
    }

    protected override void OkButtonClicked(){
        string email = emailInput.GetComponent<UIInput>().text;
        
        Hashtable result = ParentPortalLogic.Instance.VerifyAndSendPinToEmail(email);

        if(result != null){
            emailError.SetActive(false);

            if(result.Contains("Completed")){
                bool isEmailSent = (bool)result["Completed"];

                if(!isEmailSent){ //Display the errors
                    if(result.Contains("EmailErrorMsg")){
                        string emailErrorMsg = (string)result["EmailErrorMsg"];
                        emailError.SetActive(true);
                        emailError.GetComponent<UILabel>().text = emailErrorMsg;
                    }
                    if(result.Contains("SendMailErrorMsg")){
                        string sendEmailErrorMsg = (string)result["SendMailErrorMsg"];
                        emailError.SetActive(true);
                        emailError.GetComponent<UILabel>().text = sendEmailErrorMsg;
                    }
                }else{

                    ParentPortalUIManager.Instance.ShowPortalLogin(this.gameObject);
                }
            }
        }
    }

    protected override void CancelButtonClicked(){}

}
