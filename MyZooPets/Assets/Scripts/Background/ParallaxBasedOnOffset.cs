using UnityEngine;
using System.Collections;

/// <summary>
/// Parallax based on offset.
/// Parallax different layers of objects based on source's x-displacement
/// </summary>
public class ParallaxBasedOnOffset : MonoBehaviour {
	public GameObject source;
	private Vector3 sourceOriginalPosition;

	public GameObject layer1;
	public float layer1Factor;
	private Vector3 layer1OriginalPosition;

	public GameObject layer2;
	public float layer2Factor;
	private Vector3 layer2OriginalPosition;

	public GameObject layer3;
	public float layer3Factor;
	private Vector3 layer3OriginalPosition;
	
	void Start(){
		sourceOriginalPosition = source.transform.localPosition;
		if(layer1 != null){
			layer1OriginalPosition = layer1.transform.localPosition;
		}
		if(layer2 != null){
			layer2OriginalPosition = layer2.transform.localPosition;
		}
		if(layer3 != null){
			layer3OriginalPosition = layer3.transform.localPosition;
		}
	}

	void Update(){
		float displacement = source.transform.localPosition.x - sourceOriginalPosition.x;
		if(layer1 != null){
			layer1.transform.localPosition = new Vector3(layer1OriginalPosition.x + (displacement * layer1Factor), 
			                                             layer1OriginalPosition.y,
			                                             layer1OriginalPosition.z);
		}
		if(layer2 != null){
			layer2.transform.localPosition = new Vector3(layer2OriginalPosition.x + (displacement * layer2Factor), 
			                                             layer2OriginalPosition.y,
			                                             layer2OriginalPosition.z);
		}
		if(layer3 != null){
			layer3.transform.localPosition = new Vector3(layer3OriginalPosition.x + (displacement * layer3Factor), 
			                                             layer3OriginalPosition.y,
			                                             layer3OriginalPosition.z);
		}
	}
}
