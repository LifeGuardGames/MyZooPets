using UnityEngine;
using System.Collections;

public class PopupNotificationWithImageNGUI : PopupNotificationNGUI {

    public UISprite imageSprite;
//    public UILabel title;
//    public string Title{
//        get{return title.text;}
//        set{title.text = value;}
//    }

    public void SetSprite(string name){
        imageSprite.spriteName = name;
    }

}
