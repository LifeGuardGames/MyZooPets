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
	public UILabel triggerLabel;

	public void Initialize(ImmutableDataMemoryTrigger triggerData, bool isSprite){
		triggerName = triggerData.Name;
		this.isSprite = isSprite;
		if(isSprite){
			triggerSprite.type = UISprite.Type.Simple;
			triggerSprite.spriteName = triggerData.SpriteName;
		}
		else{
			triggerLabel.text = triggerData.Name;
		}

		// Set components on start
		coverSprite.gameObject.SetActive(true);
		triggerSprite.gameObject.SetActive(false);
		triggerLabel.gameObject.SetActive(false);
	}

	void OnTap(TapGesture gesture){
		CardFlipped();
	}

	public void CardFlipped(){
		// Check the manager to see if flip is allowed
		if(MemoryGameManager.Instance.IsFlipAllowed(this)){
//			Debug.Log(triggerName);
			// Ping the memory game manager this was flipped
			MemoryGameManager.Instance.NotifyClicked(this);

			coverSprite.gameObject.SetActive(false);
			if(isSprite){
				triggerSprite.gameObject.SetActive(true);
			}
			else{
				triggerLabel.gameObject.SetActive(true);
			}
		}
	}

	public void FlipResult(bool isSuccess){
		if(isSuccess){
			Destroy(gameObject);
		}
		else{
			// Flip back the card and do nothing
			coverSprite.gameObject.SetActive(true);

			if(isSprite){
				triggerSprite.gameObject.SetActive(false);
			}
			else{
				triggerLabel.gameObject.SetActive(false);
			}
		}
	}
}
