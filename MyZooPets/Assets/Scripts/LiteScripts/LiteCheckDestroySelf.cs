using UnityEngine;
using System.Collections;

/// <summary>
/// Lite check destroy self.
/// This script checks to see if the current version is lite, and destroys itself if it is
/// </summary>
public class LiteCheckDestroySelf : MonoBehaviour{
	void Awake(){
		if(VersionManager.IsLite()){
			// Destroy this gameobject
			Destroy(gameObject);
		}else{
			// Destroy this script instance
			Destroy(this);
		}
	}
}
