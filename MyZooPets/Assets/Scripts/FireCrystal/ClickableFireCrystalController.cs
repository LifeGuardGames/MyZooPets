using UnityEngine;
using System.Collections;

public class ClickableFireCrystalController : MonoBehaviour{
	public AnimationControl animControl;
	public GameObject animParent;
	public Collider thisCollider;

	void Start(){
		animControl.Play();
	}

	void OnMouseUpAsButton(){
		thisCollider.enabled = false;
		LeanTween.move(gameObject, InventoryUIManager.Instance.itemFlyToTransform, 1f)
			.setEase(LeanTweenType.easeOutQuad)
			.setOnComplete(RewardPlayerFireCrystal);
	}

	public void RewardPlayerFireCrystal(){
		InventoryManager.Instance.AddItemToInventory("Usable1");
		InventoryUIManager.Instance.PulseInventory();
		InventoryUIManager.Instance.RefreshPage();

		animParent.SetActive(false);
		StartCoroutine(CloseFireCrystalUIManager());
	}

	private IEnumerator CloseFireCrystalUIManager(){
		yield return new WaitForSeconds(1);
		FireCrystalUIManager.Instance.CloseUIBasedOnScene();
	}
}
