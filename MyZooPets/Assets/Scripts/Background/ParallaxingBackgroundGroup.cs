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
	
	// Update is called once per frame
    void Update() {
        // if(!RunnerGameManager.Instance.GameRunning) return;

        // foreach (ParallaxingBackground currentParallax in ParralaxingTextures)
        // {
        //     float currentSpeed = currentParallax.ScrollSpeed * Time.time;

        //     currentParallax.renderer.material.mainTextureOffset = new Vector2(currentSpeed, 0f);
        // }
	  }

    public void SetAlpha(float alpha) {
         // foreach (ParallaxingBackground currentParallax in ParralaxingTextures) {
			
         //    Color currentColor = currentParallax.renderer.material.color;
         //    currentColor.a = alpha;
         //    currentParallax.renderer.material.color = currentColor;
         // }
    }
}
