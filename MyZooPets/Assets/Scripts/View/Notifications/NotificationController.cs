using UnityEngine;
using System.Collections;

/// <summary>
/// Notification controller.
/// Wrapper interface for notification prefabs. This controls turning on/off buttons, show/hide,
/// and other populating things.
/// </summary>

public class NotificationController : MonoBehaviour {

	public GameObject backdrop;

	public GameObject titleLabel;
	public GameObject titleSprite;

	public GameObject button1;
	public GameObject button2;

	public GameObject contentLabel;
	public GameObject contentSprite;

	void Init(){
		backdrop.SetActive(false);
	}

	public void setButton1Message(GameObject button1Target, string functionName){
		D.assert(button1 != null, "No button1 in notification");

		UIButtonMessage buttonMessage = button1.GetComponent<UIButtonMessage>();
		buttonMessage.target = (button1Target != null)? button1Target : this.gameObject;
		buttonMessage.functionName = functionName;
	}

	public void setButton2Message(GameObject button2Target, string functionName){
		D.assert(button2 != null, "No button2 in notification");

		UIButtonMessage buttonMessage = button2.GetComponent<UIButtonMessage>();
		buttonMessage.target = (button2Target != null)? button2Target : this.gameObject;
		buttonMessage.functionName = functionName;
	}

	public void SetTitle(string atlasName, string spriteName, Vector2 spriteScale){
		D.assert(titleSprite != null, "No titleSprite in notification");

		UIAtlas atlas = Resources.Load("Atlas/" + atlasName) as UIAtlas;
		D.assert(atlas != null);

		UISprite sprite = titleSprite.GetComponent<UISprite>();
		sprite.atlas = atlas;
		spriteName = spriteName;
		titleSprite.transform.localScale = new Vector3(spriteScale.x, spriteScale.y, 1f);
	}

	public void SetTitle(string text){
		D.assert(titleLabel != null, "No titleLabel in notification");

		UILabel label = titleLabel.GetComponent<UILabel>();
		label.text = text;
	}

	public void SetContent(string atlasName, string spriteName, Vector2 spriteScale){
		D.assert(contentSprite != null, "No contentSprite in notification");

		UIAtlas atlas = Resources.Load("Atlas/" + atlasName) as UIAtlas;
		D.assert(atlas != null);

		UISprite sprite = contentSprite.GetComponent<UISprite>();
		sprite.atlas = atlas;
		spriteName = spriteName;
		contentSprite.transform.localScale = new Vector3(spriteScale.x, spriteScale.y, 1f);
	}

	public void SetContent(string text){
		D.assert (contentLabel != null, "No contentLabel in notification");

		UILabel label = contentLabel.GetComponent<UILabel>();
		label.text = text;
	}

	public void Show(){
		MoveTweenToggleDemultiplexer moveToggleDemux = this.GetComponent<MoveTweenToggleDemultiplexer>();
		MoveTweenToggle moveToggle = this.GetComponent<MoveTweenToggle>();
		if(moveToggleDemux != null){
			moveToggleDemux.Show();
			backdrop.SetActive(false); // TODO Callback maybe
		}
		else if(moveToggle != null){
			moveToggle.Show();
			backdrop.SetActive(false); // TODO Callback maybe
		}
		else{
			Debug.LogError("No move tween script detected");
		}
	}

	public void Hide(){
		MoveTweenToggleDemultiplexer moveToggleDemux = this.GetComponent<MoveTweenToggleDemultiplexer>();
		MoveTweenToggle moveToggle = this.GetComponent<MoveTweenToggle>();
		if(moveToggleDemux != null){
			moveToggleDemux.Hide();
			backdrop.SetActive(true); // TODO Callback maybe
		}
		else if(moveToggle != null){
			moveToggle.Hide();
			backdrop.SetActive(true); // TODO Callback maybe
		}
		else{
			Debug.LogError("No move tween script detected");
		}
	}
}
