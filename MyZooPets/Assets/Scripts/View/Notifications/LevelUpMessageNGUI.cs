using UnityEngine;
using System.Collections;

public class LevelUpMessageNGUI : PopupNotificationWithImageNGUI {

    public void GetTrophyMessageAndImage (){
        Message = GetMessage();
        GetTexture();
    }

    private string GetMessage(){
        string retVal = "hiii";

        return retVal;
    }

    private void GetTexture(){
        SetSprite("badgeAddonGold");
    }
    // // ================================================================================

    // protected override void Testing(){
    //     // GetTrophyMessageAndImage(BadgeTier.Gold);
    // }
}
