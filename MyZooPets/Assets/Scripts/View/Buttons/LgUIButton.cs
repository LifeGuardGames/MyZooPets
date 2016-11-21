using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LgUIButton : Button {
	public int modeTypeSize = 1;						// Figure out many is valid, up to 3
	public UIModeTypes modeType1 = UIModeTypes.None;    // Default to none
	public UIModeTypes modeType2 = UIModeTypes.None;
	public UIModeTypes modeType3 = UIModeTypes.None;
	public bool hasSound = true;
	public bool isPositiveSound = true;

	public override void OnPointerClick(PointerEventData eventData) {
		if(ClickManager.Instance.CanRespondToTap(gameObject)) {
			// Accept the click call
			base.OnPointerClick(eventData);
			if(hasSound) {
				AudioManager.Instance.PlayClip(isPositiveSound ? "buttonPositive" : "buttonNegative");
			}
		}
	}
}
