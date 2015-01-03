using UnityEngine;
using System.Collections;

/// <summary>
/// Memory card controller.
/// This controls the card in the Memory game. Cards in the game are instantiated 
/// through the MemoryBoardController and pings the MemoryGameManager when it is flipped
/// </summary>
public class MemoryCard : MonoBehaviour {

	private bool isSprite;

	private string triggerName;
	public string TriggerName{
		get{ return triggerName; }
	}

	public UISprite coverSprite;
	public UISprite triggerSprite;
	public UILocalize triggerLabelLocalize;

	public float activeTweenTime = 0.2f;
	public float activeTweenScale = 1.1f;

	private GameObject tweeningContentParent;
	private GameObject tweeningCoverParent;

	public void Initialize(ImmutableDataMemoryTrigger triggerData, bool isSprite){
		triggerName = triggerData.Name;
		this.isSprite = isSprite;

		tweeningCoverParent = coverSprite.transform.parent.gameObject;

		// Set components on start
		if(isSprite){
			triggerSprite.type = UISprite.Type.Simple;
			triggerSprite.spriteName = triggerData.SpriteName;
			triggerSprite.MakePixelPerfect();
			tweeningContentParent = triggerSprite.transform.parent.parent.gameObject;	// Get grandfather
			triggerLabelLocalize.transform.parent.gameObject.SetActive(false);
		}
		else{
			triggerLabelLocalize.key= triggerData.DisplayKey;
			triggerLabelLocalize.Localize();
			tweeningContentParent = triggerLabelLocalize.transform.parent.gameObject;
			triggerSprite.transform.parent.parent.gameObject.SetActive(false);	// Get grandfather
		}

		// Hide the original card content until its clicked
		tweeningContentParent.SetActive(false);
	}

	void OnTap(TapGesture gesture){
		CardFlipped();
	}

//	void OnFingerDown(FingerDownEvent e){ 
//		Debug.Log("DSD");
//	}

	public void CardFlipped(){
		// Check the manager to see if flip is allowed
		if(MemoryGameManager.Instance.IsFlipAllowed(this)){
//			Debug.Log(triggerName);
			// Ping the memory game manager this was flipped
			MemoryGameManager.Instance.NotifyClicked(this);

			PlayFlipSquenceOpen();
		}
	}

	private void PlayFlipSquenceOpen(){
		// Zoom in on the whole object
		LeanTween.scale(gameObject, new Vector3(activeTweenScale, activeTweenScale, 1), activeTweenTime);
		
		LeanTween.scaleX(tweeningCoverParent, 0f, activeTweenTime / 2f).setOnComplete(FlipSequenceOpenHelper);
	}
	
	private void FlipSequenceOpenHelper(){
		tweeningCoverParent.SetActive(false);

		tweeningContentParent.transform.localScale = new Vector3(0f, 1f, 1f);
		tweeningContentParent.SetActive(true);
		LeanTween.scaleX(tweeningContentParent, 1f, activeTweenTime / 2f);
	}

	private void PlayFlipSequenceClose(){
		// Zoom back out on the whole object
		LeanTween.scale(gameObject, new Vector3(1, 1, 1), activeTweenTime);
		
		LeanTween.scaleX(tweeningContentParent, 0f, activeTweenTime / 2f).setOnComplete(FlipSequenceCloseHelper);
	}
	
	private void FlipSequenceCloseHelper(){
		tweeningContentParent.SetActive(false);

		tweeningCoverParent.transform.localScale = new Vector3(0f, 1f, 1f);
		tweeningCoverParent.SetActive(true);
		LeanTween.scaleX(tweeningCoverParent, 1f, activeTweenTime / 2f);
	}

	/// <summary>
	/// External call from memory game manager
	/// </summary>
	/// <param name="isSuccess">If set to <c>true</c> is success.</param>
	public void FlipResult(bool isSuccess){
		if(isSuccess){
			Destroy(gameObject);
		}
		else{
			PlayFlipSequenceClose();
		}
	}
}
