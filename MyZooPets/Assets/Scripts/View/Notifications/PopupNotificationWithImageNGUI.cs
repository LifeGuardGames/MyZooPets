using UnityEngine;
using System.Collections;

public class PopupNotificationWithImageNGUI : PopupNotificationNGUI {

    public UISprite imageSprite;

    public void SetSprite(string name){
        imageSprite.spriteName = name;
    }

}
