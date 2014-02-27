using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class tk2dAnimationAdapter : MonoBehaviour {

	public Color color = Color.white;
	tk2dBaseSprite sprite = null;

	// Use this for initialization
	void Start() {
		sprite = GetComponent<tk2dBaseSprite>();

		if (sprite != null)
		{
			sprite.color = color;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (sprite != null && sprite.color != color)
		{
			sprite.color = color;
		}
	}
}