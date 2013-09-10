using UnityEngine;
using System.Collections;

public class DGTCharacter : MonoBehaviour {
	
	public float fTime;
	
	private bool bMoving = false;

	// Use this for initialization
	void Start () {
		Move();	
	}
	
	private void Move() {
		bMoving = true;
		transform.position = new Vector3(0,350,0);
		
		//Change the 3 V3 to where icon should move
		Vector3[] path = new Vector3[4];
		path[0] = new Vector3(0,350,0);
		path[1] = new Vector3(434, 350, 0);
		path[2] = new Vector3(434, 350, 0);
		path[3] = new Vector3(434, 350, 0);
		
		Hashtable optional = new Hashtable();
		
		optional.Add("ease", LeanTweenType.linear);
		optional.Add ("onComplete", "Done1");
		//optional.Add("onCompleteTarget", gameObject);
		//animationSprite.transform.position = origin;
		//animationSprite.transform.localScale = new Vector3(90, 90, 1);
		//animationSprite.GetComponent<UISprite>().spriteName = sprite.GetComponent<UISprite>().spriteName;
		LeanTween.move(gameObject, path, fTime, optional);			
	}
	
	void OnMouseDown() {
		Debug.Log("Resetting");
		Move();
	}
	
	private void Done1() {
		//Change the 3 V3 to where icon should move
		Vector3[] path = new Vector3[4];
		path[0] = new Vector3(434,350,0);
		path[1] = new Vector3(434, 350, 0);
		path[2] = new Vector3(434, 350, 0);
		path[3] = new Vector3(881, 624, 0);
		
		Hashtable optional = new Hashtable();
		
		optional.Add("ease", LeanTweenType.linear);
		optional.Add ("onComplete", "DoneMoving");
		//optional.Add("onCompleteTarget", gameObject);
		//animationSprite.transform.position = origin;
		//animationSprite.transform.localScale = new Vector3(90, 90, 1);
		//animationSprite.GetComponent<UISprite>().spriteName = sprite.GetComponent<UISprite>().spriteName;
		LeanTween.move(gameObject, path, fTime, optional);			
	}
	
	private void DoneMoving() {
		bMoving = false;	
	}
	
	// Update is called once per frame
	void Update () {
		if ( bMoving )
			Debug.Log(transform.position);
	}
}
