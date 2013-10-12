using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;

public class ParentPortalLogic : Singleton<ParentPortalLogic> {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    /*
        Check if email is valid and if confirmPassword matches with password
        Returns a hashtable that contains
        Valid : T or F
        EmailErrorMsg:
        PinErrorMsg:
        ConfirmPinErrorMsg:
    */
    public Hashtable VerifyAndSaveSetupInput(string email, string password, string confirmPassword){
        Hashtable result = new Hashtable();

        VerifyEmail(email, ref result);
        VerifyPassword(password, confirmPassword, ref result);

        if(result.Count == 0){ //No errors, so save the data
            result.Add("Saved", true);

            DataManager.Instance.ParentEmail = email;
            DataManager.Instance.ParentPortalPin = password;
        }else
            result.Add("Saved", false);

        return result;
    }

    //Check if the input email has the right email format
    private void VerifyEmail(string email, ref Hashtable result){
        bool isValidEmail = Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"); 

        if(String.IsNullOrEmpty(email))
            result.Add("EmailErrorMsg", Localization.Localize("ERROR_EMAIL_EMPTY"));
        else
            if(!isValidEmail)
                result.Add("EmailErrorMsg", Localization.Localize("ERROR_EMAIL_INVALID"));
    }

    private void VerifyPassword(string password, string confirmPassword, ref Hashtable result){
        if(String.IsNullOrEmpty(password))
            result.Add("PinErrorMsg", Localization.Localize("ERROR_PIN_EMPTY"));
            if(String.IsNullOrEmpty(confirmPassword))
                result.Add("ConfirmPinErrorMsg", Localization.Localize("ERROR_PIN_CONFIRM_EMPTY"));
        else
            if(password.Length != 4)
                result.Add("PinErrorMsg", Localization.Localize("ERROR_PIN_INVALID"));
            else
                if(!password.Equals(confirmPassword))
                    result.Add("ConfirmPinErrorMsg", Localization.Localize("ERROR_PIN_NO_MATCH"));
    }
}
