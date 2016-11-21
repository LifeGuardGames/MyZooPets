using UnityEngine;

/// <summary>
/// World button that sends a message.
/// Checks the clickmanager before processing the button click.
/// </summary>
public class LgWorldButtonMessage : LgWorldButton {
	public GameObject target;
	public string functionName;

	protected override void ProcessClick() {
		if(string.IsNullOrEmpty(functionName)) {
			Debug.LogError("LgWorldButtonMessage functionName in parent (" + gameObject + ") cannot be null");
			return;
		}
		if(target == null) {
			Debug.LogError("LgWorldButtonMessage target in parent (" + gameObject + ") cannot be null", this);
			return;
		}
		target.SendMessage(functionName, gameObject, SendMessageOptions.RequireReceiver);
	}
}