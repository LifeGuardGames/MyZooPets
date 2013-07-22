using UnityEngine;
using System.Collections;

public class LevelUpMessageNGUI : PopupNotificationWithImageNGUI {

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
            imageSprite.spriteName="trophyBronze";
            break;

            case TrophyTier.Silver:
            imageSprite.spriteName="trophySilver";
            break;

            case TrophyTier.Gold:
            imageSprite.spriteName="trophyGold";
            break;

        }

    }
    // ================================================================================

    protected override void Testing(){
        // GetTrophyMessageAndImage(TrophyTier.Gold);
    }
}
