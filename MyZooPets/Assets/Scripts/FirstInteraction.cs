using UnityEngine;
using System.Collections;

public class FirstInteraction : Singleton<FirstInteraction> {

	public string firstInteractable = " ";

	public void SetString(string _item){
		firstInteractable = _item;
		Analytics.Instance.FirstInteraction(firstInteractable);
	}
}
