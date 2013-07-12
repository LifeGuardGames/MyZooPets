using UnityEngine;
using System.Collections;

public class GameOverRewardMessageNGUI : PopupNotificationNGUI {

    public void SetRewardMessage(int deltaStars, int deltaPoints){
        Message = "Stars +" + deltaStars + "\nPoints +" + deltaPoints;
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
