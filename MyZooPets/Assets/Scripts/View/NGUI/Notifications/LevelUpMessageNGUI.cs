using UnityEngine;
using System.Collections;

public class LevelUpMessageNGUI : PopupNotificationWithImageNGUI {

    public void GetTrophyMessageAndImage (BadgeTier Badge){
        Message = GetMessage(Badge);
        GetTexture(Badge);
    }

    private string GetMessage(BadgeTier Badge){
        string retVal = "";

        switch (Badge){

            case BadgeTier.Null:
            retVal = "Too bad, better luck next time.";
            break;

            case BadgeTier.Bronze:
            retVal = "Nice try, but you can do better.";
            break;

            case BadgeTier.Silver:
            retVal = "Not bad!";
            break;

            case BadgeTier.Gold:
            retVal = "Excellent work!";
            break;
        }

        return retVal;
    }

    private void GetTexture(BadgeTier Badge){

        switch (Badge){

            case BadgeTier.Null:
            break;

            case BadgeTier.Bronze:
            SetSprite("TrophyBronze");
            break;

            case BadgeTier.Silver:
            SetSprite("TrophySilver");
            break;

            case BadgeTier.Gold:
            SetSprite("TrophyGold");
            break;

        }

    }
    // ================================================================================

    protected override void Testing(){
        // GetTrophyMessageAndImage(BadgeTier.Gold);
    }
}
