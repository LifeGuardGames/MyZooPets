/* Sean Duane
 * ParallaxingBackgroundGroup.cs
 * 8:26:2013   14:43
 * Description:
 * Handles updating the current translation of the spawned ParralaxingBackgrounds.
 * The mesh never moves, we jiust set the texture offset to let that do the "moving:
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxingBackgroundGroup : MonoBehaviour {
    public string GroupID = "";
    public List<ParallaxingBackground> ParralaxingTextures = new List<ParallaxingBackground>();

	// Use this for initialization
	void Start() {
	}
	
	// Update is called once per frame
    void LateUpdate() {
        foreach (ParallaxingBackground currentParallax in ParralaxingTextures)
        {
            float currentSpeed = currentParallax.ScrollSpeed * Time.time;

            Vector2 newOffset = new Vector2(currentSpeed, 0f);
            UITexture theTexture = currentParallax.GetComponent<UITexture>();
            theTexture.material.SetTextureOffset("_MainTex", newOffset);
        }
	}

    public void SetAlpha(float inAlpha) {
        // foreach (ParallaxingBackground currentParallax in ParralaxingTextures) {
        //     UITexture theTexture = currentParallax.GetComponent<UITexture>();
        //     Color currentColor = theTexture.material.color;
        //     currentColor.a = inAlpha;
        //     theTexture.material.color = currentColor;
        // }
    }
}
