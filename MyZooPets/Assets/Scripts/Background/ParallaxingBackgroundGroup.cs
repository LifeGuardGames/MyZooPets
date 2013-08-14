using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxingBackgroundGroup : MonoBehaviour {
    public string GroupID = "";
    public List<ParallaxingBackground> ParralaxingTextures = new List<ParallaxingBackground>();

    private PlayerRunner mPlayerRunner = null;

	// Use this for initialization
	void Start () {
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
            mPlayerRunner = playerObject.GetComponent<PlayerRunner>();
	}
	
	// Update is called once per frame
	void Update () {
        foreach (ParallaxingBackground currentParallax in ParralaxingTextures)
        {
            float currentSpeed = currentParallax.ScrollSpeed * Time.time;
            if (mPlayerRunner != null)
                currentSpeed *= mPlayerRunner.Speed;

            Vector2 newOffset = new Vector2(currentSpeed, 0f);
            currentParallax.renderer.material.SetTextureOffset("_MainTex", newOffset);
        }
	}

    public void SetAlpha(float inAlpha) {
        foreach (ParallaxingBackground currentParallax in ParralaxingTextures)
        {
            Color currentColor = currentParallax.renderer.material.color;
            currentColor.a = inAlpha;
            currentParallax.renderer.material.color = currentColor;
        }
    }
}
