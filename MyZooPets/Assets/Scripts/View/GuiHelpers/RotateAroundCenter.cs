using UnityEngine;
using System.Collections;

public class RotateAroundCenter : MonoBehaviour {
	public float speed = 100;

	private void Update()
	{
		transform.Rotate(Vector3.forward * Time.deltaTime * speed);
	}
}
