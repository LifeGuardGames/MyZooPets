using UnityEngine;
using System.Collections;

public class TextureOffsetLoop : MonoBehaviour {
	
	public Vector2 increment;
	
	// Update is called once per frame
	void Update () {
		Vector2 offset = new Vector2(increment.x * Time.time, increment.y * Time.time);
		renderer.material.mainTextureOffset = offset;
	}
}
