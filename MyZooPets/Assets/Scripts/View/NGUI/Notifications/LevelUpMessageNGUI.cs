using UnityEngine;
using System.Collections;

public class LevelUpMessageNGUI : PopupNotificationNGUI {

    public UISprite trophyImage;

    public void GetTrophyMessageAndImage (TrophyTier trophy){
        Message = GetMessage(trophy);
        GetTexture(trophy);
    }

    private string GetMessage(TrophyTier trophy){
        string retVal = "";

        switch (trophy){

            case TrophyTier.Null:
            retVal = "Too bad, better luck next time.";
            break;

            case TrophyTier.Bronze:
            retVal = "Nice try, but you can do better.";
            break;

            case TrophyTier.Silver:
            retVal = "Not bad!";
            break;

            case TrophyTier.Gold:
            retVal = "Excellent work!";
            break;
        }

        return retVal;
    }

    private void GetTexture(TrophyTier trophy){

        switch (trophy){

            case TrophyTier.Null:
            break;

            case TrophyTier.Bronze:
            trophyImage.spriteName="trophyBronze";
            break;

            case TrophyTier.Silver:
            trophyImage.spriteName="trophySilver";
            break;

            case TrophyTier.Gold:
            trophyImage.spriteName="trophyGold";
            break;

        }

    }
    // ================================================================================

    protected override void Testing(){
        // GetTrophyMessageAndImage(TrophyTier.Gold);
    }
}
