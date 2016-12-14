using UnityEngine;

public class SpriteAlternating : MonoBehaviour {
	public Sprite sprite1;
	public Sprite sprite2;
	public SpriteRenderer targetSprite;
	private bool toggleAux = false;

	void Start() {
		InvokeRepeating("ToggleSprite", 0, 1f);
	}

	private void ToggleSprite() {
		toggleAux = !toggleAux;
		targetSprite.sprite = toggleAux ? sprite1 : sprite2;
	}
}
