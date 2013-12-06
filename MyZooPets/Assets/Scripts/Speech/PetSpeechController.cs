using UnityEngine;
using System.Collections;

public class PetSpeechController : SpeechController<PetSpeechController>{
    // Message options keys
    // MessageText:
    // ImageTextureName: (use the sprite name in the atlas)
    // ImageClicktarget: (use this if image is clickable. target of the image click)
    // ImageClickFunctionName: (the function name that the image click should call)
    public enum Keys{
        MessageText,
        ImageTextureName,
        ImageClickTarget,
        ImageClickFunctionName
    } 

    private GameObject petSpeechWithTextPrefab;
    private GameObject petSpeechWithImagePrefab;
    private GameObject petSpeechWithImageAndTextPrefab; 

    //---------------------------------------------------
    // SpawnMessage()
    // Look at the message keys and decide what is the 
    // appropriate layout to use for this message
    //---------------------------------------------------
    protected override void SpawnMessage(Hashtable message){
        currentMessage = null;

        //Use SpeechWithImageAndText prefab
        if(message.ContainsKey(Keys.MessageText) && message.ContainsKey(Keys.ImageTextureName)){
            if(petSpeechWithImageAndTextPrefab == null)
                petSpeechWithImageAndTextPrefab = Resources.Load("PetSpeechWithImageAndText") as GameObject;

            currentMessage = LgNGUITools.AddChildWithPosition(this.gameObject, petSpeechWithImageAndTextPrefab);
            currentMessage.transform.Find("Label_Message").GetComponent<UILabel>().text = (string) message[Keys.MessageText];
            currentMessage.transform.Find("Image/Sprite_Message").GetComponent<UISprite>().spriteName = (string) message[Keys.ImageTextureName];

            //also check if the image should be make clickable. 
            if(message.ContainsKey(Keys.ImageClickTarget) && message.ContainsKey(Keys.ImageClickFunctionName)){
                GameObject imageGO = currentMessage.transform.Find("Image").gameObject;
                LgButtonMessage buttonMessage = imageGO.AddComponent<LgButtonMessage>();

                buttonMessage.target = (GameObject) message[Keys.ImageClickTarget];
                buttonMessage.functionName = (string) message[Keys.ImageClickFunctionName];
                imageGO.AddComponent<BoxCollider>();
            }

        }
        //Use SpeechWithText prefab
        else if(message.ContainsKey(Keys.MessageText)){
            if(petSpeechWithTextPrefab == null)
                petSpeechWithTextPrefab = Resources.Load("PetSpeechWithText") as GameObject;

            currentMessage = LgNGUITools.AddChildWithPosition(this.gameObject, petSpeechWithTextPrefab);
            currentMessage.transform.Find("Label_Message").GetComponent<UILabel>().text = (string) message[Keys.MessageText];
        }
        //Use SpeechWithImage prefab
        else if(message.ContainsKey(Keys.ImageTextureName)){
            if(petSpeechWithImagePrefab == null)
                petSpeechWithImagePrefab = Resources.Load("PetSpeechWithImage") as GameObject;

            currentMessage = LgNGUITools.AddChildWithPosition(this.gameObject, petSpeechWithImagePrefab);
            currentMessage.transform.Find("Sprite_Message").GetComponent<UISprite>().spriteName = (string) message[Keys.ImageTextureName];
        }
        else{
        }
    }

    void OnGUI(){
        if(isDebug){
            if(GUI.Button(new Rect(20, 20, 20, 20), "1")){
                Hashtable msgOption = new Hashtable();
                msgOption.Add(Keys.MessageText, "Give me food!");
                msgOption.Add(Keys.ImageTextureName, "iconStore");
                Talk(msgOption);
            }
            if(GUI.Button(new Rect(50, 20, 20, 20), "2")){
                Hashtable msgOption = new Hashtable();
                msgOption.Add(Keys.ImageTextureName, "speechImageHeart");
                Talk(msgOption);
            }
            if(GUI.Button(new Rect(80, 20, 20, 20), "3")){
                Hashtable msgOption = new Hashtable();
                msgOption.Add(Keys.MessageText, "fit as many words in this text box as possible. let's go");
                Talk(msgOption);
            }
            if(GUI.Button(new Rect(110, 20, 20, 20), "4")){
                Hashtable msgOption = new Hashtable();
                msgOption.Add(Keys.MessageText, "Give me food!");
                msgOption.Add(Keys.ImageTextureName, "iconStore");
                msgOption.Add(Keys.ImageClickTarget, this.gameObject);
                msgOption.Add(Keys.ImageClickFunctionName, "");
                Talk(msgOption);
            }
        }
    }
}
