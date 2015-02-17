﻿using UnityEngine;
using System.Collections;

public class ClickableFireCrystalController : MonoBehaviour{
	public AnimationControl animControl;
	public GameObject animParent;
	public Collider thisCollider;

	void Start(){
		animControl.Play();
	}

	public void ObjectClicked(){
		thisCollider.enabled = false;
		LeanTween.move(this.gameObject, InventoryUIManager.Instance.GetItemFlyToPosition(), 1f)
			.setEase(LeanTweenType.easeOutQuad)
			.setOnComplete(RewardPlayerFireCrystal);
	}

	public void RewardPlayerFireCrystal(){
		InventoryLogic.Instance.AddItem("Usable1", 1);
		StartCoroutine(CloseFireCrystalUIManager());
	}

	private IEnumerator CloseFireCrystalUIManager(){
		animParent.SetActive(false);
		yield return new WaitForSeconds(1);
		FireCrystalUIManager.Instance.CloseUIBasedOnScene();
	}
}