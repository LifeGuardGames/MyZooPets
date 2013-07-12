using UnityEngine;
using System.Collections;

public class GameOverRewardMessageNGUI : PopupNotificationNGUI {

    public void SetRewardMessage(int deltaStars, int deltaPoints){
        string message = "";
        if (deltaStars > 0){
            message += "Stars +" + deltaStars;
        }
        if (deltaPoints > 0){
            message += "\nPoints +" + deltaPoints;
        }
        Message = message;
    }

    // ================================================================================

    protected override void Testing(){
        // // testing code
        // SetRewardMessage(30,30);
        // Button1Text = "Nice!";
        // Button1Callback = message1;
        // if (numOfButtons >= 2){
        //     Button2Text = "Shit.";
        //     Button2Callback = message2;
        // }
    }
}
