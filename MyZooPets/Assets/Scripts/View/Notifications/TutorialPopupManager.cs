using UnityEngine;
using System.Collections;

/// <summary>
/// Tutorial popup controller.
/// Wrapper interface for tutorial popup prefabs. This controls turning on/off buttons, show/hide,
/// and other populating things.
/// </summary>

public class TutorialPopupManager : BackDrop {

	public GameObject titleSprite;
	public GameObject button1;
	public GameObject contentSprite;

	public delegate void CallBack();
	public CallBack NextButtonCallBack;

	// public void SetButton1Message(GameObject button1Target){ 
	// 	D.assert(button1 != null, "No button1 in notification");

	// 	UIButtonMessage buttonMessage = button1.GetComponent<UIButtonMessage>();
	// 	buttonMessage.target = (button1Target != null)? button1Target : this.gameObject;
	// 	buttonMessage.functionName = functionName;
	// }

	protected override void Awake(){
		base.Awake();
		backDropParent = gameObject;
	}

	//Handler for button
	public void OnNextButtonClick(){
		if(NextButtonCallBack != null) NextButtonCallBack();
		Hide();
	}

	// public void SetTitle(string atlasName, string spriteName, Vector2 spriteScale){
	// 	D.assert(titleSprite != null, "No titleSprite in notification");

	// 	UIAtlas atlas = Resources.Load("Atlas/" + atlasName) as UIAtlas;
	// 	D.assert(atlas != null);

	// 	UISprite sprite = titleSprite.GetComponent<UISprite>();
	// 	sprite.atlas = atlas;
	// 	spriteName = spriteName;
	// 	titleSprite.transform.localScale = new Vector3(spriteScale.x, spriteScale.y, 1f);
	// }

	public void SetContent(string spriteName){
		// D.assert(contentSprite != null, "No contentSprite in notification");

		// UIAtlas atlas = Resources.Load("Atlas/" + atlasName) as UIAtlas;
		// D.assert(atlas != null);

		UISprite sprite = contentSprite.GetComponent<UISprite>();
		sprite.spriteName = spriteName;
		// sprite.atlas = atlas;
		// spriteName = spriteName;
		// contentSprite.transform.localScale = new Vector3(spriteScale.x, spriteScale.y, 1f);
	}

	public void Display(){
		MoveTweenToggleDemultiplexer moveToggleDemux = this.GetComponent<MoveTweenToggleDemultiplexer>();
		if(moveToggleDemux != null){
			moveToggleDemux.Show();
			DisplayBackDrop();
		}else{
			Debug.LogError("No move tween script detected");
		}
	}

	private void Hide(){
		MoveTweenToggleDemultiplexer moveToggleDemux = this.GetComponent<MoveTweenToggleDemultiplexer>();
		if(moveToggleDemux != null){
			moveToggleDemux.Hide();
			DisplayBackDrop();
		}else{
			Debug.LogError("No move tween script detected");
		}
	}
}
