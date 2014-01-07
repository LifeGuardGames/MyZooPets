// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class TwitterUIManager : MonoBehaviour {
//     public GameObject twitter;

//     private const string CONSUMER_KEY = "nBADj5RBHQocsgs8i5FFOA";
//     private const string CONSUMER_SECRET = "ZP8rGTTWsL5f3IaeVw9lwQOr1uNXgNw91YLfCuG6p8";
//     private string lastRequestType; //prime31's twitter event call back is too generic, so
//                                     //we can't tell what request was called inside the listener
//                                     //using this variable here to keep track

// 	// Use this for initialization
// 	void Start () {
//         TwitterManager.loginSucceededEvent += LoginSucceeded;
//         TwitterManager.loginFailedEvent += LoginFailed;
//         TwitterManager.requestDidFinishEvent += RequestDidFinishEvent;
//         TwitterManager.requestDidFailEvent += RequestDidFailEvent;

//         TwitterBinding.init(CONSUMER_KEY, CONSUMER_SECRET);

//         /*
//             If twitter session cannot be opened then user needs to login first.
//             Credentials need to be verified even if the user is logged in because
//             the cached token may be invalid
//         */
//         if(!TwitterBinding.isLoggedIn())
//             RefreshTwitterButtons();
//         else
//             VerifyUserCredentials();
// 	}
	
//     void OnDestroy(){
//         TwitterManager.loginSucceededEvent -= LoginSucceeded;
//         TwitterManager.loginFailedEvent -= LoginFailed;
//         TwitterManager.requestDidFinishEvent -= RequestDidFinishEvent;
//         TwitterManager.requestDidFailEvent -= RequestDidFailEvent;
//     }

//     //Toggle UI Buttons depending user's access token
//     private void RefreshTwitterButtons(){
//         GameObject twitterButton = twitter.transform.Find("Button").gameObject;
//         GameObject twitterLogoutButton = twitter.transform.Find("Logout").gameObject;

//         if(TwitterBinding.isLoggedIn()){
//             twitterButton.transform.Find("Label").GetComponent<UILabel>().text = "Tweet!!";
//             twitterButton.GetComponent<UIButtonMessage>().target = this.gameObject;
//             twitterButton.GetComponent<UIButtonMessage>().functionName = "TweetToTimeline";

//             twitterLogoutButton.SetActive(true);
//         }else{
//             twitterButton.transform.Find("Label").GetComponent<UILabel>().text = "Login";
//             twitterButton.GetComponent<UIButtonMessage>().target = this.gameObject;
//             twitterButton.GetComponent<UIButtonMessage>().functionName = "TwitterLogin";

//             twitterLogoutButton.SetActive(false);
//         }
//     }

//     public void TweetToTimeline(){
//         lastRequestType = "Tweet";
//         TwitterBinding.postStatusUpdate( "im posting this from Unity: " + Time.deltaTime );
//         // var pathToImage = Application.persistentDataPath + "/" + FacebookGUIManager.screenshotFilename;
//         // TwitterBinding.postStatusUpdate( "I'm posting this from Unity with a fancy image: " + Time.deltaTime, pathToImage );
//     }

//     public void TwitterLogin(){
//         TwitterBinding.showLoginDialog();
//     }

//     public void TwitterLogout(){
//         TwitterBinding.logout();
//         RefreshTwitterButtons();
//         Debug.Log("Twitter logout");
//     }

//     //Use this to verify cached access token
//     private void VerifyUserCredentials(){
//         lastRequestType = "VerifyCredential";
//         string apiCall = "1.1/account/verify_credentials.json";

//         var dict = new Dictionary<string, string>();
//         dict.Add("include_entities", "false");
//         dict.Add("skip_status", "true");

//         TwitterBinding.performRequest("GET", apiCall, dict);
//     }

//     private void LoginSucceeded( string screenname ){
//         RefreshTwitterButtons();
//     }
    
//     private void LoginFailed( string error ){
//         Debug.Log("Login failed " + error);
//     }

//     //Whenever a request is successful
//     //TO DO: Prime31 says the plugin will support customizable event handler
//     //once unity WWW has been fixed
//     private void RequestDidFinishEvent(object result){
//         if(result != null){
//             switch(lastRequestType){
//                 case "Tweet":
//                 break;
//                 case "VerifyCredential":
//                 break;
//             }
//             lastRequestType = "";
//             Prime31.Utils.logObject(result);
//         }
//     }

//     //Whenever a request fail
//     private void RequestDidFailEvent(string error){
//         Debug.Log("request fail " + error);
//         switch(lastRequestType){
//             case "Tweet":
//             break;
//             case "VerifyCredential":
//                 //Cannot verify user credential, so logout user to force a login
//                 TwitterLogout();
//             break;
//         }
//         lastRequestType = "";
//     }
// }
