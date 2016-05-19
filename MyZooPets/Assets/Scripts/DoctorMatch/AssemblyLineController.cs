using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssemblyLineController : MonoBehaviour {
	public Transform StartPosition;
	public Transform EndPosition;
	public GameObject itemParent;
	public GameObject itemPrefab;
	public bool growsLowHealth = true; //Line grows as you decrease in health
	public int visibleCount = 2;
	public bool constantCount = false;
	private Queue<AssemblyLineItem> itemQueue;
	private float distanceBetween = -50;
	private int startingCount = 4;
	private bool working=true;

	// Fill the list with items and in the right positions
	public void Initialize() {
		StartCoroutine(InitializeHelper());
	}

	private IEnumerator InitializeHelper() {
		DestroyItems();
		yield return 0;

		/*if(positionList == null || positionList.Count == 0){
			Debug.LogError("Position list not initialized or empty");
			yield break;
		}*/

		itemQueue = new Queue<AssemblyLineItem>();
		// Init and throw it into the queue
		PopulateQueue();
	}

	public AssemblyLineItem PopFirstItem() {
		AssemblyLineItem poppedItem = itemQueue.Dequeue();
		return poppedItem;
	}

	public void ClearLine(){
		Debug.Log("Called");
		foreach (AssemblyLineItem itemScript in itemQueue) {
			itemScript.Complete(EndPosition.position, new Vector3(distanceBetween, 0));
		}
		itemQueue.Clear();
		PopulateQueue();
	}


	public void ShiftAndAddNewItem() {
		if (!working)
			return;
		foreach (AssemblyLineItem itemScript in itemQueue) {
			int newIndex = itemScript.GetIncrementIndex();
			LeanTween.cancel(itemScript.gameObject);
			LeanTween.move(itemScript.gameObject, StartPosition.position + newIndex * new Vector3(distanceBetween, 0), 0.1f);
		}

		// Add new item
		GameObject item = GameObjectUtils.AddChild(itemParent, itemPrefab);
		int newItemIndex = startingCount;
		item.transform.position = StartPosition.position + newItemIndex * new Vector3(distanceBetween, 0);
		AssemblyLineItem newItemScript = item.GetComponent<AssemblyLineItem>();
		newItemScript.Init(newItemIndex);
		itemQueue.Enqueue(newItemScript);
		newItemScript.CompareVisible(visibleCount);
	}

	public void DestroyItems() {
		foreach (Transform child in itemParent.transform) {
			Destroy(child.gameObject);
		}
		if (itemQueue!=null)
			itemQueue.Clear();
	}

	public void UpdateVisibleCount(float percentage) {
		if (constantCount||itemQueue==null)
			return;
		if (percentage < .2f) {
			visibleCount=(growsLowHealth) ? 5 : 1;
		} else if (percentage < .4f) {
			visibleCount=(growsLowHealth) ? 4 : 2;
		} else if (percentage < .6f) {
			visibleCount=(growsLowHealth) ? 3 : 3;
		} else if (percentage < .8f) {
			visibleCount=(growsLowHealth) ? 2 : 4;
		} else {
			visibleCount=(growsLowHealth) ? 1 : 5;
		}
		foreach (AssemblyLineItem itemScript in itemQueue) {
			itemScript.CompareVisible(visibleCount);
		}
	}
	private void PopulateQueue() {
		Debug.Log(itemQueue.Count);
		for (int i = 0; i < startingCount + 1; i++) {
			GameObject item = GameObjectUtils.AddChild(itemParent, itemPrefab);
			item.transform.position = StartPosition.position + i * new Vector3(distanceBetween, 0);
			AssemblyLineItem itemScript = item.GetComponent<AssemblyLineItem>();
			itemScript.Init(i);
			itemQueue.Enqueue(itemScript);
		}
	}
}
