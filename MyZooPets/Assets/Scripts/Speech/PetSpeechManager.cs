using UnityEngine;
using System.Collections;

public class PetSpeechManager : SpeechController<PetSpeechManager>{
    // Message options keys
    // MessageText:
    // ImageTextureName: (use the sprite name in the atlas)
    // ImageClicktarget: (use this if image is clickable. target of the image click)
    // ImageClickFunctionName: (the function name that the image click should call)
    public enum Keys{
        MessageText,
        ImageTextureName,
		AtlasName,
        ImageClickTarget,
        ImageClickFunctionName,
		BubbleSpriteName,
		Follow3DTarget
    } 

	public GameObject spawnParent;

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

			currentMessage = LgNGUITools.AddChildWithPosition(spawnParent, petSpeechWithImageAndTextPrefab);

			// Assign the follow target for the dialogue box
			currentMessage.GetComponent<FollowObjectRaycast>().target = (GameObject) message[Keys.Follow3DTarget];

			UILabel label = currentMessage.transform.Find("LabelParent/Label_Message").GetComponent<UILabel>();
			label.text = (string) message[Keys.MessageText];

			if(message.ContainsKey(Keys.BubbleSpriteName)){
				UISprite bubbleSprite = currentMessage.transform.Find("BubbleParent/Sprite_Bubble").GetComponent<UISprite>();
				bubbleSprite.spriteName = (string) message[Keys.BubbleSpriteName];
			}

			UISprite sprite = currentMessage.transform.Find("Image/Sprite_Message").GetComponent<UISprite>();

			//switch atlas if necessary
			if(message.ContainsKey(Keys.AtlasName)){
				string atlasName = (string) message[Keys.AtlasName];
//				Debug.Log(atlasName);
//				Debug.Log(sprite.atlas.gameObject.name);
				GameObject atlas = (GameObject) Resources.Load(atlasName);
				sprite.atlas = atlas.GetComponent<UIAtlas>();
//				sprite.atlas = Resources.Load(atlasName, typeof(UIAtlas)) as UIAtlas;
			}
			sprite.spriteName = (string) message[Keys.ImageTextureName];
		
            //also check if the image should be make clickable. 
            if(message.ContainsKey(Keys.ImageClickTarget) && message.ContainsKey(Keys.ImageClickFunctionName)){
                LgButtonMessage buttonMessage = sprite.gameObject.AddComponent<LgButtonMessage>();
                buttonMessage.target = (GameObject) message[Keys.ImageClickTarget];
                buttonMessage.functionName = (string) message[Keys.ImageClickFunctionName];
                sprite.gameObject.AddComponent<BoxCollider>();
            }
        }
        //Use SpeechWithText prefab
        else if(message.ContainsKey(Keys.MessageText)){
            if(petSpeechWithTextPrefab == null)
                petSpeechWithTextPrefab = Resources.Load("PetSpeechWithText") as GameObject;

			currentMessage = LgNGUITools.AddChildWithPosition(spawnParent, petSpeechWithTextPrefab);

			// Assign the follow target for the dialogue box
			currentMessage.GetComponent<FollowObjectRaycast>().target = (GameObject) message[Keys.Follow3DTarget];

			UILabel label = currentMessage.transform.Find("LabelParent/Label_Message").GetComponent<UILabel>();
			label.text = (string) message[Keys.MessageText];
        }
        //Use SpeechWithImage prefab
        else if(message.ContainsKey(Keys.ImageTextureName)){
            if(petSpeechWithImagePrefab == null)
                petSpeechWithImagePrefab = Resources.Load("PetSpeechWithImage") as GameObject;

			currentMessage = LgNGUITools.AddChildWithPosition(spawnParent, petSpeechWithImagePrefab);

			// Assign the follow target for the dialogue box
			currentMessage.GetComponent<FollowObjectRaycast>().target = (GameObject) message[Keys.Follow3DTarget];

			currentMessage.transform.Find("Image/Sprite_Message").GetComponent<UISprite>().spriteName = (string) message[Keys.ImageTextureName];
        }
    }

    // void OnGUI(){
    //     if(isDebug){
    //         if(GUI.Button(new Rect(20, 20, 20, 20), "1")){
    //             Hashtable msgOption = new Hashtable();
    //             msgOption.Add(Keys.MessageText, "Give me food!");
    //             msgOption.Add(Keys.ImageTextureName, "iconStore");
    //             Talk(msgOption);
    //         }
    //         if(GUI.Button(new Rect(50, 20, 20, 20), "2")){
    //             Hashtable msgOption = new Hashtable();
    //             msgOption.Add(Keys.ImageTextureName, "speechImageHeart");
    //             Talk(msgOption);
    //         }
    //         if(GUI.Button(new Rect(80, 20, 20, 20), "3")){
    //             Hashtable msgOption = new Hashtable();
    //             msgOption.Add(Keys.MessageText, "fit as many words in this text box as possible. let's go");
    //             Talk(msgOption);
    //         }
    //         if(GUI.Button(new Rect(110, 20, 20, 20), "4")){
    //             Hashtable msgOption = new Hashtable();
    //             msgOption.Add(Keys.MessageText, "Give me food!");
    //             msgOption.Add(Keys.ImageTextureName, "iconStore");
    //             msgOption.Add(Keys.ImageClickTarget, this.gameObject);
    //             msgOption.Add(Keys.ImageClickFunctionName, "");
    //             Talk(msgOption);
    //         }
    //     }
    // }
}