using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssemblyLineController : MonoBehaviour {
	public List<Transform> positionList;
	public GameObject itemParent;
	public GameObject itemPrefab;

	private Queue<AssemblyLineItem> itemQueue;

	// Fill the list with items and in the right positions
	public void Initialize() {
		StartCoroutine(InitializeHelper());
	}

	private IEnumerator InitializeHelper() {
		DestroyItems();
		yield return 0;

		if(positionList == null || positionList.Count == 0){
			Debug.LogError("Position list not initialized or empty");
			yield break;
		}

		itemQueue = new Queue<AssemblyLineItem>();

		// Init and throw it into the queue
		for(int i = 0; i < positionList.Count; i++){
			GameObject item = GameObjectUtils.AddChild(itemParent, itemPrefab);
			item.transform.position = positionList[i].position;
			AssemblyLineItem itemScript = item.GetComponent<AssemblyLineItem>();
			itemScript.Init(i);
			itemQueue.Enqueue(itemScript);
		}
	}

	public AssemblyLineItem PopFirstItem(){
		AssemblyLineItem poppedItem = itemQueue.Dequeue();
		return poppedItem;
	}

	public void ShiftAndAddNewItem(){
		foreach(AssemblyLineItem itemScript in itemQueue){
			int newIndex = itemScript.GetIncrementIndex();
			LeanTween.cancel(itemScript.gameObject);
			LeanTween.move(itemScript.gameObject, positionList[newIndex].position, 0.1f);
		}

		// Add new item
		GameObject item = GameObjectUtils.AddChild(itemParent, itemPrefab);
		int newItemIndex = positionList.Count - 1;
		item.transform.position = positionList[newItemIndex].position;
		AssemblyLineItem newItemScript = item.GetComponent<AssemblyLineItem>();
		newItemScript.Init(newItemIndex);
		itemQueue.Enqueue(newItemScript);
	}

	public void DestroyItems(){
		foreach (Transform child in itemParent.transform) {
			Destroy(child.gameObject);
		}
	}
}
