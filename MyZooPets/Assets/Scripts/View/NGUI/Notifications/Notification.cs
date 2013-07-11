using UnityEngine;
using System.Collections;

public class Notification : MonoBehaviour {

    public UILabel textArea;
    public int numOfButtons = 1;
    public UILabel button1;
    public UILabel button2;

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

    // These two functions are called when the buttons are clicked.
    void Button1Action(){
        Button1Callback();
    }
    void Button2Action(){
        Button2Callback();
    }

    // =========================================================================================================================================================
    void Start () {
        Testing();
    }

    void Testing(){
        // testing code
        Message = "Hello there!";
        Button1Text = "Hey!";
        Button1Callback = message1;
        if (numOfButtons >= 2){
            Button2Text = "Fuck off.";
            Button2Callback = message2;
        }
    }

    void message1(){
        Debug.Log("button 1 clicked");
    }
    void message2(){
        Debug.Log("button 2 clicked");
    }
}
