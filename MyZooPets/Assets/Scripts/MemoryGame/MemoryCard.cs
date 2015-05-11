using UnityEngine;
using System.Collections;

/// <summary>
/// Memory card controller.
/// This controls the card in the Memory game. Cards in the game are instantiated 
/// through the MemoryBoardController and pings the MemoryGameManager when it is flipped
/// </summary>
public class MemoryCard : MonoBehaviour {
	private string triggerName;
	public string TriggerName{
		get{ return triggerName; }
	}
	public UISprite coverSprite;
	public UISprite triggerSprite;
	public UILocalize triggerLabelLocalize;
	public GameObject NGUIParent;

	public float pressDownScale = 0.9f;
	public float activeTweenTime = 0.2f;
	public float activeTweenScale = 1.1f;

	public ParticleSystem endingParticle;
	public float waitForParticle = 1.5f;

	private GameObject tweeningContentParent;
	private GameObject tweeningCoverParent;
	private ParticleSystem triggerTypeParticle;

	private bool isClickable = true;	// When the card is showing/animating lock the click, internal check of card state

	public void Initialize(ImmutableDataMemoryTrigger triggerData, bool isSprite){
		triggerName = triggerData.Name;

		tweeningCoverParent = coverSprite.transform.parent.gameObject;

		// Set components on start
		if(isSprite){
			triggerSprite.type = UISprite.Type.Simple;
			triggerSprite.spriteName = triggerData.SpriteName;
			triggerSprite.MakePixelPerfect();
			triggerSprite.name = triggerData.SpriteName;
			tweeningContentParent = triggerSprite.transform.parent.parent.gameObject;	// Get grandfather
			triggerLabelLocalize.transform.parent.gameObject.SetActive(false);	// Disable the unused half
		}
		else{
			triggerLabelLocalize.key= triggerData.DisplayKey;
			triggerLabelLocalize.Localize();
			tweeningContentParent = triggerLabelLocalize.transform.parent.gameObject; // Get grandfather
			triggerSprite.transform.parent.parent.gameObject.SetActive(false);	// Disable the unused half
		}

		// Hide the original card content until its clicked
		tweeningContentParent.SetActive(false);

		// Assign the respective trigger type particle
		GameObject particlePrefab = Resources.Load(triggerData.TypeParticlePrefab) as GameObject;
		triggerTypeParticle = GameObjectUtils.AddChildWithTransform(this.gameObject, particlePrefab).particleSystem;
	}

	void OnTap(TapGesture gesture){
		if(isClickable && MemoryGameManager.Instance.GetGameState() == MinigameStates.Playing){
			CardFlipped();
		}
	}

	void OnFingerDown(FingerDownEvent e){
		if(isClickable && MemoryGameManager.Instance.GetGameState() == MinigameStates.Playing){
			gameObject.transform.localScale = new Vector3(pressDownScale, pressDownScale, 1f);
		}
	}

	void OnFingerHover( FingerHoverEvent e){
		if(isClickable && MemoryGameManager.Instance.GetGameState() == MinigameStates.Playing){
			if( e.Phase == FingerHoverPhase.Exit){
				GameObjectUtils.ResetLocalScale(this.gameObject);
			}
		}
	}

	public void CardFlipped(){
		// Check the manager to see if flip is allowed
		if(MemoryGameManager.Instance.IsFlipAllowed(this)){
			// Ping the memory game manager this was flipped
			MemoryGameManager.Instance.NotifyClicked(this);

			isClickable = false;
			PlayFlipSquenceOpen();
		}
	}

	private void PlayFlipSquenceOpen(){
		AudioManager.Instance.PlayClip("memoryCard1");

		// Zoom in on the whole object
		LeanTween.scale(gameObject, new Vector3(activeTweenScale, activeTweenScale, 1), activeTweenTime);
		LeanTween.scaleX(tweeningCoverParent, 0f, activeTweenTime / 2f).setOnComplete(FlipSequenceOpenHelper);
	}
	
	private void FlipSequenceOpenHelper(){
		tweeningCoverParent.SetActive(false);

		tweeningContentParent.transform.localScale = new Vector3(0f, 1f, 1f);
		tweeningContentParent.SetActive(true);
		LeanTween.scaleX(tweeningContentParent, 1f, activeTweenTime / 2f).setOnComplete(FlipSequenceOpenFinished);
	}

	private void FlipSequenceOpenFinished(){
		tweeningContentParent.GetComponent<AnimationControl>().Play();
		triggerTypeParticle.Play();
	}

	private void PlayFlipSequenceClose(){
		AudioManager.Instance.PlayClip("memoryCard2");
		tweeningContentParent.GetComponent<AnimationControl>().Stop();
		triggerTypeParticle.Stop();

		// Zoom back out on the whole object
		LeanTween.scale(gameObject, new Vector3(1, 1, 1), activeTweenTime);
		LeanTween.scaleX(tweeningContentParent, 0f, activeTweenTime / 2f).setOnComplete(FlipSequenceCloseHelper);
	}
	
	private void FlipSequenceCloseHelper(){
		tweeningContentParent.SetActive(false);

		tweeningCoverParent.transform.localScale = new Vector3(0f, 1f, 1f);
		tweeningCoverParent.SetActive(true);
		LeanTween.scaleX(tweeningCoverParent, 1f, activeTweenTime / 2f).setOnComplete(FlipSequenceCloseFinished);
	}

	private void FlipSequenceCloseFinished(){
		isClickable = true;	// Unlock the click so the user can click on it again
	}

	/// <summary>
	/// External call from memory game manager
	/// </summary>
	/// <param name="isSuccess">If set to <c>true</c> is success.</param>
	public void FlipResult(bool isSuccess){
		if(isSuccess){
			LeanTween.scale(NGUIParent, new Vector3(0, 0, 0), 0.1f).setOnComplete(SuccessHelper);
		}
		else{
			PlayFlipSequenceClose();
		}
	}

	/// <summary>
	/// Callback for tween finished, play particle effects here
	/// </summary>
	private void SuccessHelper(){
		tweeningContentParent.SetActive(false);
		tweeningCoverParent.SetActive(false);
		endingParticle.Play();
		triggerTypeParticle.Stop();
		Destroy(gameObject, waitForParticle);
	}
}
