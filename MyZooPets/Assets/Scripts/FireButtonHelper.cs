using UnityEngine;

/// <summary>
/// This is the UI input for firebutton dropping and pointer up/down
/// </summary>
public class FireButtonHelper : MonoBehaviour, IDropInventoryTarget {
	public Collider worldCollider;

	public void OnItemDropped(InventoryItem itemData) {
		FireButtonManager.Instance.Step1_SetButtonActiveWithItem(itemData);
	}

	public void OnPointerDown() {
		Debug.Log("Pointer down");
		FireButtonManager.Instance.Step3_ChargeFire();
	}

	public void OnPointerUp() {
		Debug.Log("Pointer up");
		FireButtonManager.Instance.Step4_ReleaseCharge();
	}

	public void Toggle3DCollider(bool isOn) {
		worldCollider.enabled = isOn;
	}
}
