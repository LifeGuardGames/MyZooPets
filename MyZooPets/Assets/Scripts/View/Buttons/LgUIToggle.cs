using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LgUIToggle : Toggle {
	public int modeTypeSize = 1;                        // Figure out many is valid, up to 3
	public UIModeTypes modeType1 = UIModeTypes.None;    // Default to none
	public UIModeTypes modeType2 = UIModeTypes.None;
	public UIModeTypes modeType3 = UIModeTypes.None;

	public override void OnPointerClick(PointerEventData eventData) {
		UnityEngine.Debug.Log("BUTTON LCIK");
		if(ClickManager.Instance.CanRespondToTap(gameObject)) {
			UnityEngine.Debug.Log("PASS");
			// Accept the click call
			base.OnPointerClick(eventData);
		}
	}
}
