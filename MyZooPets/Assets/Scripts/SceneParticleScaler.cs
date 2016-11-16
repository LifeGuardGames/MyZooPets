using UnityEngine;
using System.Collections;

/// <summary>
/// Normalize particles based on the scene's UI canvas camera size
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class SceneParticleScaler : MonoBehaviour {

	void Awake() {
		
	}
	
	public float GetScaleFactorByScene(string levelName) {
		float baseModifier = 10f;		// Canvas size of the bedroom and yard
		if(levelName == SceneUtils.INHALERGAME) {   // Canvas size: 16.14
			return 16.14f / baseModifier;
		}
		else if(levelName == SceneUtils.TRIGGERNINJA) { // Canvas size: 5
			return 5f / baseModifier;
		}
		else if(levelName == SceneUtils.MEMORY) {   // Canvas size: 10
			return 1f;
		}
		else if(levelName == SceneUtils.RUNNER) {   // Canvas size: 26
			return 26f / baseModifier;
		}
		else if(levelName == SceneUtils.DOCTORMATCH) {  // Canvas size: 150
			return 150f / baseModifier;
		}
		else if(levelName == SceneUtils.SHOOTER) {  // Canvas size: 3.140527
			return 3.140527f / baseModifier;
		}
		else if(levelName == SceneUtils.MICROMIX) { // Canvas size: 5
			return 5f / baseModifier;
		}
		else {
			return 1f;
		}
	}
}
