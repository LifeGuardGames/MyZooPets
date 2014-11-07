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

	public UISprite triggerSprite;
	public UILabel triggerLabel;

	public void Initialize(ImmutableDataMemoryTrigger triggerData, bool isSprite){
		triggerName = triggerData.Name;
		this.isSprite = isSprite;
		if(isSprite){
			triggerSprite.spriteName = triggerData.SpriteName;
		}
		else{
			triggerLabel.text = triggerData.Name;
		}
	}

	public void CardFlipped(){
		// Check the manager to see if flip is allowed
		if(MemoryGameManager.Instance.IsFlipAllowed()){
			// Ping the memory game manager this was flipped
			MemoryGameManager.Instance.NotifyClicked(this);
		}
	}

	public void FlipResult(bool isSuccess){
		if(isSuccess){

		}
		else{
			// Flip back the card and do nothing
		}
	}
}
