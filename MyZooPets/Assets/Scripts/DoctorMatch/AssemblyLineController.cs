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
	public int visibleCount = 2;
	public bool constantCount = false;
	private Queue<AssemblyLineItem> itemQueue;
	private float distanceBetween = -50;
	private int startingCount = 4;
	private float clearTime = .05f;
	private float moveTime = .2f;
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
		PopulateQueue(StartPosition.position);
	}

	public AssemblyLineItem PopFirstItem() {
		AssemblyLineItem poppedItem = itemQueue.Dequeue();
		return poppedItem;
	}

	public IEnumerator ClearLine() {
		
		foreach (AssemblyLineItem itemScript in itemQueue) {
			if (itemScript.itemSprite.isVisible) {
				yield return new WaitForSeconds(clearTime * 4);
				GameObject particle = Instantiate(particlePrefab);
				particle.transform.position = itemScript.transform.position;
			}
			Destroy(itemScript.gameObject);
		}
		itemQueue.Clear();
		PopulateQueue(OffscreenPosition.position,1);
		MoveUpLine(.2f);
		DoctorMatchManager.Instance.FinishClear();
	}


	public void ShiftAndAddNewItem() {
		MoveUpLine(moveTime);

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
		if (itemQueue != null)
			itemQueue.Clear();
	}

	public void UpdateVisibleCount(float percentage) {
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
		foreach (AssemblyLineItem itemScript in itemQueue) {
			itemScript.CompareVisible(visibleCount);
		}
	}

	private void PopulateQueue(Vector3 startPos, int indexOffset=0) {
		for (int i = 0; i < startingCount + 1; i++) {
			GameObject item = GameObjectUtils.AddChild(itemParent, itemPrefab);
			item.transform.position = startPos + i * new Vector3(distanceBetween, 0);
			AssemblyLineItem itemScript = item.GetComponent<AssemblyLineItem>();
			itemScript.Init(i+indexOffset);
			itemQueue.Enqueue(itemScript);
			itemScript.CompareVisible(visibleCount);
		}
	}

	private void MoveUpLine(float timeToTake) {
		foreach (AssemblyLineItem itemScript in itemQueue) {
			int newIndex = itemScript.GetIncrementIndex();
			LeanTween.cancel(itemScript.gameObject);
			LeanTween.move(itemScript.gameObject, StartPosition.position + newIndex * new Vector3(distanceBetween, 0), timeToTake);
		}
	}
}
