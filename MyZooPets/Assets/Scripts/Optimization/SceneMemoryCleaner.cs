using UnityEngine;
using System.Collections;

/*
    Scene memory cleaner is necessary in each scene to clean up what we refer to as
    phantom memory. Some how assets from other scenes leak into newly loaded scene
    because they cannot be unload on time. After some experimenting we found that
    we need a gameobject in each scene to unload unused assets on start
    Refer to Memory Management in LifeguardGames google doc to read more...
*/
public class SceneMemoryCleaner : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(CleanUp());
	}

    private IEnumerator CleanUp(){
        yield return new WaitForSeconds(1.0f);
        Resources.UnloadUnusedAssets();
    }
}
