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
}
