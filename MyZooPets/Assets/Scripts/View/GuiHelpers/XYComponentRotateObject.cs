using UnityEngine;
using System.Collections;

/// <summary>
/// XY component rotate object.
/// Given vector2 (x,y) components, rotate the object according to a unit circle
/// </summary>
public class XYComponentRotateObject : MonoBehaviour {
	
	public float x;
	public float y;
	
	void Start(){
		Vector2 toVector2 = new Vector2(y, x);
		Vector2 fromVector2 = new Vector2(0, 1);
	     
	    float ang = Vector2.Angle(fromVector2, toVector2);
	    Vector3 cross = Vector3.Cross(fromVector2, toVector2);
	     
	    if (cross.z > 0)
	    ang = 360 - ang;
	    
	    //Debug.Log(ang);
		gameObject.transform.Rotate(Vector3.forward, ang);
	}
}
