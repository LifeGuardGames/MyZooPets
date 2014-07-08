using UnityEngine;
using System.Collections;

public class SpriteChangeOrder : MonoBehaviour {
	public float sortingOrder = 999999f;
	private float lastSortingOrder = 999999f;
	private SpriteRenderer sprite;

	void Start(){
		sprite = gameObject.GetComponent<SpriteRenderer>();
		lastSortingOrder = sortingOrder;
	}

	void Update(){
		if(sortingOrder != lastSortingOrder){
			lastSortingOrder = sortingOrder;
			ChangeOrder((int)sortingOrder);
		}
	}

	public void ChangeOrder(int order){
		if(sprite){
			sprite.sortingOrder = order;
		}
		else{
			Debug.LogError("No sprite found for script");
		}
	}
}
