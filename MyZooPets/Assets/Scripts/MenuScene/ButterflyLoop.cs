using UnityEngine;
using System.Collections;

/// <summary>
/// Class that controls a butterfly sprite
/// This will fly to the end of the screen and then delay before looping again
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ButterflyLoop : MonoBehaviour {

	public Sprite butterflyUpSprite;
	public Sprite butterflyDownSprite;
	public float flapSpeed = 0.1f;
	public float moveDuration = 5f;
	public float delay = 2f;
	public float deviation = 2f;
	public float startingY = 12f;

	private bool flapToggle = true;
	private SpriteRenderer spriteRenderer;

	void Start(){
		spriteRenderer = GetComponent<SpriteRenderer>();
		InvokeRepeating("FlapToggle", 0f, flapSpeed);
		StartMovement();
	}

	void OnDestroy(){
		CancelInvoke("FlapToggle");
	}
	
	private void FlapToggle(){
		flapToggle = !flapToggle;
		spriteRenderer.sprite = flapToggle ? butterflyUpSprite : butterflyDownSprite;
	}

	private void StartMovement(){
		LeanTween.cancel(gameObject);
		Vector3[] path = GeneratePath(12, -14f, 14f);
		LeanTween.moveSpline(gameObject, path, moveDuration).setOnComplete(StartMovement).setDelay(delay);
	}

	private Vector3[] GeneratePath(int numPoints, float leftBound, float rightBound){
		Vector3[] path = new Vector3[numPoints + 1];
		float screenWidth = rightBound - leftBound;
		float ySoFar = startingY;

		// Randomly decide if we want right or left
		int randomNumber = Random.Range(0, 2);
		bool isRightToLeft = (randomNumber == 1) ? true : false;

		if(isRightToLeft){
			transform.localScale = new Vector3(1, 1, 1);
			for(int i = 0; i <= numPoints; i++){
				float randomYDelta = Random.Range(-1 * deviation, deviation);
				Vector3 point = new Vector3(rightBound - ((screenWidth/numPoints)*i), ySoFar += randomYDelta, 0);
				path[i] = point;
			}
		}
		else{
			transform.localScale = new Vector3(-1, 1, 1);
			for(int i = 0; i <= numPoints; i++){
				float randomYDelta = Random.Range(-1 * deviation, deviation);
				Vector3 point = new Vector3(leftBound + ((screenWidth/numPoints)*i), ySoFar += randomYDelta, 0);
				path[i] = point;
			}
		}
		return path;
	}
}

