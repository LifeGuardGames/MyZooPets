using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssemblyLineController : MonoBehaviour {
	public Transform StartPosition;
	public Transform EndPosition;
	public Transform OffscreenPosition;
	public GameObject itemParent;
	public GameObject itemPrefab;
	public GameObject particlePrefab;
	public bool growsLowHealth = true;
	//Line grows as you decrease in health
	private int visibleCount = 2;
	public bool constantCount = false;
	private Queue<AssemblyLineItem> itemQueue;
	private float distanceBetween = -50;
	private int startingCount = 4;
	private float clearTime = .05f;
	private const float moveTime = .2f;
	// Fill the list with items and in the right positions
	public float ClearTime {
		get {
			return visibleCount * clearTime * 4 + moveTime;
		}
	}

	public bool LineComplete {
		get {
			return itemQueue.Count == 0;
		}
	}

	public int Count {
		get {
			return itemQueue.Count;
		}
	}

	public IEnumerator Initialize(bool isTutorial) {
		DestroyItems();
		yield return new WaitForEndOfFrame();
		itemQueue = new Queue<AssemblyLineItem>();
		if (!isTutorial)
			PopulateQueue(true);
	}

	public AssemblyLineItem PopFirstItem() {
		return itemQueue.Dequeue();
	}

	public AssemblyLineItem PeekFirstItem() {
		return itemQueue.Peek();
	}

	public void ShiftAndAddNewItem() {
		MoveUpLine();
		// Add new item
		GameObject item = GameObjectUtils.AddChild(itemParent, itemPrefab);
		int newItemIndex = itemQueue.Count;
		item.transform.position = StartPosition.position + newItemIndex * new Vector3(distanceBetween, 0);
		AssemblyLineItem newItemScript = item.GetComponent<AssemblyLineItem>();
		newItemScript.Init(newItemIndex);
		itemQueue.Enqueue(newItemScript);
		newItemScript.CompareVisible(visibleCount);
	}

	public void SpawnTutorialSet(int stage) {
		for (int i = 1; i < AssemblyLineItem.SPRITE_COUNT; i++) {
			GameObject item = GameObjectUtils.AddChild(itemParent, itemPrefab);
			item.transform.position = StartPosition.position + (i - 1) * new Vector3(distanceBetween, 0);
			AssemblyLineItem newItemScript = item.GetComponent<AssemblyLineItem>();
			newItemScript.Init(i - 1, stage, i); 
			itemQueue.Enqueue(newItemScript);
		}
	}

	public void DestroyItems() {
		foreach (Transform child in itemParent.transform) {
			Destroy(child.gameObject);
		}
		if (itemQueue != null)
			itemQueue.Clear();
	}

	public void MoveUpLine(bool compare = true) {
		UpdateVisibleCount();
		foreach (AssemblyLineItem itemScript in itemQueue) {
			int newIndex = itemScript.GetIncrementIndex();
			LeanTween.cancel(itemScript.gameObject);
			LeanTween.move(itemScript.gameObject, StartPosition.position + newIndex * new Vector3(distanceBetween, 0), moveTime);
			if (compare)
				itemScript.CompareVisible(visibleCount);
		}
	}

	public void PopulateQueue(bool compare = false, int count = -1, int indexOffset=0) {
		UpdateVisibleCount();
		int toSpawn = (count == -1) ? startingCount + 1 : count;
		for (int i = 0; i < toSpawn; i++) {
			GameObject item = GameObjectUtils.AddChild(itemParent, itemPrefab);
			item.transform.position = StartPosition.position + (i+indexOffset) * new Vector3(distanceBetween, 0);
			AssemblyLineItem itemScript = item.GetComponent<AssemblyLineItem>();
			itemScript.Init((i+indexOffset));
			itemQueue.Enqueue(itemScript);
			if (compare)
				itemScript.CompareVisible(visibleCount);
		}
	}

	private void UpdateVisibleCount() {
		float percentage = DoctorMatchManager.Instance.lifeBarController.Percentage; //Connection w/o manager
		if (constantCount || itemQueue == null || DoctorMatchManager.Instance.Paused)
			return;
		if (percentage < .2f) {
			visibleCount = (growsLowHealth) ? 5 : 1;
		} else if (percentage < .4f) {
			visibleCount = (growsLowHealth) ? 4 : 2;
		} else if (percentage < .6f) {
			visibleCount = (growsLowHealth) ? 3 : 3;
		} else if (percentage < .8f) {
			visibleCount = (growsLowHealth) ? 2 : 4;
		} else {
			visibleCount = (growsLowHealth) ? 1 : 5;
		}
	}
}
