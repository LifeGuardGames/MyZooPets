using UnityEngine;
using System.Collections;
using Prime31;

public class FacebookUIManager : MonoBehaviour {
    public GameObject facebook;

    private bool fbHasPublishPermission = false;

	// Use this for initialization
	void Start () {
        FacebookManager.sessionOpenedEvent += SessionOpenedEvent;   
        FacebookManager.reauthorizationSucceededEvent += ReauthorizationSucceededEvent;
        FacebookManager.loginFailedEvent += LoginFailedEvent;
        FacebookBinding.init();

        /*
            If a fb session cannot be opened that means there is no cached long-term token,
            so user needs to login first.
            If a fb session is opened, we still need to do a simple check because cached long-term
            token can be invalid if the app has been removed by the user from the app dashboard 
        */
        if(!FacebookBinding.isSessionValid())
            RefreshFacebookButtons();
        else
            CheckAccessTokenWithGraphAPI();
	}

    void OnDestroy(){
        FacebookManager.sessionOpenedEvent -= SessionOpenedEvent;   
        FacebookManager.reauthorizationSucceededEvent -= ReauthorizationSucceededEvent;
        FacebookManager.loginFailedEvent -= LoginFailedEvent;
    }

    //Toggle UI Buttons depending user's access token
    private void RefreshFacebookButtons(){
        GameObject fbButton = facebook.transform.Find("Button").gameObject;
        GameObject fbLogoutButton = facebook.transform.Find("Logout").gameObject;

        //If facebook session is valid allow user to pose to wall; otherwise, user needs to 
        //log in first
        if(FacebookBinding.isSessionValid()){
            fbButton.transform.Find("Label").GetComponent<UILabel>().text = "Post to Wall";
            fbButton.GetComponent<UIButtonMessage>().target = this.gameObject;
            fbButton.GetComponent<UIButtonMessage>().functionName = "FacebookPostToWall";

            fbLogoutButton.SetActive(true);
        }else{
            fbButton.transform.Find("Label").GetComponent<UILabel>().text = "Login";
            fbButton.GetComponent<UIButtonMessage>().target = this.gameObject;
            fbButton.GetComponent<UIButtonMessage>().functionName = "FacebookLogin";

            fbLogoutButton.SetActive(false);
        }
    }

    //Post to user's wall. If user posting for the first time need to get permission to post to
    //wall from facebook
    public void FacebookPostToWall(){
        if(fbHasPublishPermission){
            Facebook.instance.postMessageWithLinkAndLinkToImage( "link post from Unity: " + Time.deltaTime, 
                "http://prime31.com", "Prime31 Studios", 
                "http://prime31.com/assets/images/prime31logo.png", 
                "Prime31 Logo", PostWallCompletionHandler);
       }else{
            var permissions = new string[] {"publish_stream"};
            FacebookBinding.reauthorizeWithPublishPermissions(permissions,
                FacebookSessionDefaultAudience.Friends);
       }
    }

    //Login to facebook and request basic permission from user
    public void FacebookLogin(){
        var permissions = new string[] {"email"};
        FacebookBinding.loginWithReadPermissions(permissions);
        Debug.Log("login");
    }

    //Logout of facebook. close session and invalidate token
    //!!NOTE: _logout() method from FacebookManager.mm was modified to remove cached session
    public void FacebookLogout(){
        FacebookBinding.logout();
        FacebookBinding.renewCredentialsForAllFacebookAccounts();
        RefreshFacebookButtons();
        Debug.Log("logout");
    }

    //Do a simple graph API request to get information of the user
    //TO DO: Maybe there's a better way to check the cached long-term token, but right now
    //this is the simpliest/easiest way I can think of
    private void CheckAccessTokenWithGraphAPI(){
        Facebook.instance.graphRequest("me", HTTPVerb.GET, CheckAccessTokenEvent);
    }

    //--------------------------FB Event Listeners-----------------------------
    //Event listener for when a session is opened. From Facebook initialization and Facebook login
    private void SessionOpenedEvent(){
        fbHasPublishPermission = FacebookBinding.getSessionPermissions().Contains( "publish_stream" );
        RefreshFacebookButtons();
    }

    //After reauthorization to get more permission from user
    private void ReauthorizationSucceededEvent(){
        fbHasPublishPermission = FacebookBinding.getSessionPermissions().Contains( "publish_stream" );

        if(fbHasPublishPermission)
            FacebookPostToWall();
    }

    private void LoginFailedEvent(P31Error error){
        Debug.Log("login in fail: " + error);
    }

    private void CheckAccessTokenEvent(string error, object result){
        //If we can't get basic info from graph api then the access token is probably
        //not valid anymore despite the long-term access token cached by facebook SDK
        //Logout user and force user to login again
        if(error != null)
            FacebookLogout();
    }

    /*
        Post to wall callback. Currently Prime31 cannot return specific error messages
        because unitys WWW method doesn't allow it, so there is no good way to find out
        what is the problem when post graph request returns a "400 bad request" error
    */
    private void PostWallCompletionHandler(string error, object result){
        if( error != null )
            Debug.LogError( error );
        else
            Prime31.Utils.logObject( result );
    }
}
