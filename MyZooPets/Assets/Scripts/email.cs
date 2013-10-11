using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class email : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void SendMail(){
         MailMessage mail = new MailMessage();
 
            mail.From = new MailAddress("jason.sonnygrease@gmail.com");
            mail.To.Add("yannru.cheng@gmail.com");
            mail.Subject = "Test Mail";
            mail.Body = "This is for testing SMTP mail from GMAIL";
 
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential("account", "password") as ICredentialsByHost;
            smtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback = 
                delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
                    { return true; };
            smtpServer.Send(mail);
         Debug.Log("success");
    }

    void OnGUI(){
     if(GUI.Button(new Rect(10f, 10f, 100f, 100f), "Send Mail")){
        SendMail(); 
     }
    }
}
