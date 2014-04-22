using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class ParentPortalLogic : Singleton<ParentPortalLogic> {

    // public string ParentEmail{
    //     get{return DataManager.Instance.ParentEmail;}
    // }

    // public string ParentPortalPin{
    //     get{return DataManager.Instance.ParentPortalPin;}
    // }

    /*
        Check if email is valid and if confirmPassword matches with password
        RETURN:
            Completed : T or F
            EmailErrorMsg:
            PinErrorMsg:
            ConfirmPinErrorMsg:
    */
    public Hashtable VerifyAndSaveSetupInput(string email, string password, string confirmPassword){
        Hashtable result = new Hashtable();

        VerifyEmail(email, ref result);
        VerifyPassword(password, confirmPassword, ref result);

        if(result.Count == 0){ //No errors, so save the data
            result.Add("Completed", true);

            // DataManager.Instance.ParentEmail = email;
            // DataManager.Instance.ParentPortalPin = password;
        }else
            result.Add("Completed", false);

        return result;
    }

    /*
        Check if email is valid and send the pin to that email
        RETURN:
            Completed: T or F
            EmailErrorMsg:
            SendMailErrorMsg:
    */
    public Hashtable VerifyAndSendPinToEmail(string email){
        Hashtable result = new Hashtable();

        VerifyEmail(email, ref result);

        if(!result.Contains("EmailErrorMsg")){
            // string pin = DataManager.Instance.ParentPortalPin;
            // SendPinToEmail(email, pin, ref result);
        }

        if(result.Count == 0)
            result.Add("Completed", true);
        else
            result.Add("Completed", false);

        return result;
    }

    /*
        Verify user input pin with saved pin
        RETURN:
            Completed: T or F
            InputPinErrorMsg:
    */
    public Hashtable VerifyLoginPin(string inputPin){
        Hashtable result = new Hashtable();

        VerifyInputPinWithSavedPin(inputPin, ref result);

        if(result.Count == 0)
            result.Add("Completed", true);
        else
            result.Add("Completed", false);

        return result;
    }

    /*
        RETURN:
            Completed: T or F
            InputPinErrorMsg:
            PinErrorMsg:
            ConfirmPinErrorMsg:
    */
    public Hashtable VerifyResetInput(string oldPin, string newPin, string newPinConfirm){
        Hashtable result = new Hashtable();    

        VerifyInputPinWithSavedPin(oldPin, ref result);
        VerifyPassword(newPin, newPinConfirm, ref result);

        if(result.Count == 0){ //No errors. Reset pin
            result.Add("Completed", true);

            // DataManager.Instance.ParentPortalPin = newPin;
        }else{
            result.Add("Completed", false);
        }

        return result;
    }

    /*
        Errors:
            InputErrorMsg
    */
    private void VerifyInputPinWithSavedPin(string inputPin, ref Hashtable result){
        // string savedPin = DataManager.Instance.ParentPortalPin;

            //not first time anymore after this property is called
            PlayerPrefs.SetInt("IsFirstTime", 0);
        // //Pin can't be empty
        // if(String.IsNullOrEmpty(inputPin))
        //     result.Add("InputPinErrorMsg", Localization.Localize("ERROR_PIN_EMPTY"));
        // else
        //     //Check if pin is correct
        //     if(savedPin != inputPin)
        //         result.Add("InputPinErrorMsg", Localization.Localize("ERROR_WRONG_PIN"));
    }

    /*
        Check if the input email has the right email format
        Errors:
            EmailErrorMsg
    */
    private void VerifyEmail(string email, ref Hashtable result){
        bool isValidEmail = IsValidEmail(email);

        //Email cannot be empty
        if(String.IsNullOrEmpty(email))
            result.Add("EmailErrorMsg", Localization.Localize("ERROR_EMAIL_EMPTY"));
        else
            //Email not in the right format
            if(!isValidEmail)
                result.Add("EmailErrorMsg", Localization.Localize("ERROR_EMAIL_INVALID"));
    }

    private bool IsValidEmail(string email){
        return Regex.IsMatch(email, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + 
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$");
    }

    /*
        Errors:
            PinErrorMsg
            ConfirmPinErrorMsg
    */
    private void VerifyPassword(string password, string confirmPassword, ref Hashtable result){
        //Pin cannot be empty
        if(String.IsNullOrEmpty(password))
            result.Add("PinErrorMsg", Localization.Localize("ERROR_PIN_EMPTY"));
            //Pin confirm cannot be empty
            if(String.IsNullOrEmpty(confirmPassword))
                result.Add("ConfirmPinErrorMsg", Localization.Localize("ERROR_PIN_CONFIRM_EMPTY"));
        else
            //Pin has to be 4 characters
            if(password.Length != 4)
                result.Add("PinErrorMsg", Localization.Localize("ERROR_PIN_INVALID"));
            else
                //Pin confirm needs to be the same as pin
                if(!password.Equals(confirmPassword))
                    result.Add("ConfirmPinErrorMsg", Localization.Localize("ERROR_PIN_NO_MATCH"));
    }

    /*
        Using SmtpClient to send email
        Errors:
            SendMailErrorMsg
    */
    private void SendPinToEmail(string email, string pin, ref Hashtable result){
        string to = email;
        string from = "info@lifeguardgames.com";

        string account = "";
        string password = "";
        MailMessage message = new MailMessage(from, to);
 
        message.Subject = "Wellapets parent portal";
        message.Body = "This is for testing SMTP mail from LifeGuardGames";

        //TO DO: consider changing this to NameCheap server instead
        SmtpClient client = new SmtpClient("smtp.gmail.com");
        client.Timeout = 10;
        client.Port = 587;
        client.Credentials = new System.Net.NetworkCredential(account, password) as ICredentialsByHost;
        client.EnableSsl = true;

        //TO DO: Need to figure out how to check server certificate. Is it even important for us because
        //we are only using the server to send an email 
        ServicePointManager.ServerCertificateValidationCallback = 
        delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors){
            return true;
        };

        try{
            //Send message synchronously 
            client.Send(message);

        }catch(SmtpException e){
            Debug.Log(e.Message + " : " + e.StatusCode);
            result.Add("SendMailErrorMsg", Localization.Localize("ERROR_SMTP_BAD_CONNECTION"));
        }
    }
}
