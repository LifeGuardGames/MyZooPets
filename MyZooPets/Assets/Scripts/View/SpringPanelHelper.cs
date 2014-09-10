using UnityEngine;
using System.Collections;

/// <summary>
/// Spring panel helper.
/// Spring panels screw up tween toggles if it is still active
/// </summary>
public class SpringPanelHelper : MonoBehaviour {
	void StopSpring(){
		SpringPanel spring = GetComponent<SpringPanel>();
		if(spring != null){
			spring.enabled = false;
		}
	}
}
