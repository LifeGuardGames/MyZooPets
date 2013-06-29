using UnityEngine;
using System.Collections;

public class GrowShrink : MonoBehaviour {

    public float smallX = 1;
    public float smallY = 1;
    public float smallZ = 1;
    public float bigX = 1.1f;
    public float bigY = 1.1f;
    public float bigZ = 1.1f;

    // for testing only - when used with the code in the update loop,
    // Play() and Stop() can be triggered from the inspector.
    // public bool running = true;

    bool isPlaying = false;

    Vector3 smaller;
    Vector3 bigger;

    int leanTweenCurrent;

    // LeanTween optional hashtables
    Hashtable optionalGrow = new Hashtable();
    Hashtable optionalShrink = new Hashtable();

	void Awake () {
        smaller = new Vector3(smallX, smallY, smallZ);
        bigger = new Vector3(bigX, bigY, bigZ);
        // optionalGrow.Add("ease", LeanTweenType.easeInOutQuad);
        optionalGrow.Add("onComplete", "Shrink");

        // optionalShrink.Add("ease", LeanTweenType.easeInOutQuad);
        optionalShrink.Add("onComplete", "Grow");
	}

	// Update is called once per frame
	void Update () {
        // uncomment when testing
        // if (running){
        //     if (!isPlaying){
        //         Play();
        //     }
        // }
        // else {
        //     if (isPlaying){
        //         Stop();
        //     }
        // }
	}

    public void Play(){
        isPlaying = true;
        Grow();
    }

    public void Stop(){
        isPlaying = false;
        LeanTween.cancel(gameObject, leanTweenCurrent);
        transform.localScale = new Vector3(1,1,1); // reset to original size
    }

    void Grow(){
        leanTweenCurrent = LeanTween.scale(gameObject, bigger, 0.4f, optionalGrow);
    }

    void Shrink(){
        leanTweenCurrent = LeanTween.scale(gameObject, smaller, 0.4f, optionalShrink);
    }
}
