using UnityEngine;
using System.Collections;

public class PopupNotificationNGUI : MonoBehaviour {

    public UILabel textArea;
    public UILabel button1;
    public UILabel button2;

    public UISprite backdrop;

    // set the following

    public int numOfButtons = 1;

    public delegate void Callback();
    public Callback Button1Callback;
    public Callback Button2Callback;

    public string Message{
        get{return textArea.text;}
        set{textArea.text = value;}
    }
    public string Button1Text{
        get{return button1.text;}
        set{button1.text = value;}
    }
    public string Button2Text{
        get {
            if (numOfButtons < 2){
                Debug.LogError("Number of buttons is set to less than 2.");
                return null;
            }
            return button2.text;
        }
        set{
            if (numOfButtons < 2){
                Debug.LogError("Number of buttons is set to less than 2.");
            }
            button2.text = value;
        }
    }

    protected void Awake(){
        backdrop.active = false;
    }

    // These two functions are called when the buttons are clicked.
    protected void Button1Action(){
        if (Button1Callback != null) Button1Callback();
        Hide();
    }
    protected void Button2Action(){
        if (Button2Callback != null) Button2Callback();
        Hide();
    }

    // Display the popup panel
    public void Display(){
        backdrop.active = true;
        // Hashtable optional = new Hashtable();
        // optional.Add("ease", LeanTweenType.easeOutBounce);
        // LeanTween.move(panelRect, finalPosition, 1.0f, optional);
        Time.timeScale = 0;
        ClickManager.ClickLock();
        GetComponent<MoveTweenToggle>().Show(0.5f);
    }

    // Hide the popup panel
    protected void Hide(){
        backdrop.active = false;
        // Hashtable optional = new Hashtable();
        // optional.Add("onCompleteTarget", gameObject);
        // optional.Add("onComplete", "DestroyNotification");
        GetComponent<MoveTweenToggle>().Hide(0.5f);
        // optional.Add("ease", LeanTweenType.easeInOutQuad);
        // LeanTween.move(panelRect, initPosition, 0.5f, optional);
        Time.timeScale = 1;
        ClickManager.ReleaseClickLock();
        Destroy(gameObject, 3.0f);
    }

    // =========================================================================================================================================================
    // Code for Testing
    protected void Start () {
        Testing();
    }

    protected virtual void Testing(){
        // // testing code
        // Message = "Hello there!";
        // Button1Text = "Hey!";
        // Button1Callback = message1;
        // if (numOfButtons >= 2){
        //     Button2Text = "Fuck off.";
        //     Button2Callback = message2;
        // }
    }

    // protected void message1(){
    //     Debug.Log("button 1 clicked");
    // }
    // protected void message2(){
    //     Debug.Log("button 2 clicked");
    //     DestroyNotification();
    // }
}
