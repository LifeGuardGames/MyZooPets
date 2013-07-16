using UnityEngine;
using System.Collections;

public class ProgressBarController : MonoBehaviour {

    public UISlider slider; // for setup
    public ProgressBarAnimation animation;

    /*
        numSteps includes step 0. So if numSteps were '3', then the steps
        included would be 0, 1, 2.
    */
    void Init(int numSteps){
        slider.sliderValue = 0;
        slider.numberOfSteps = numSteps;
        animation.Init();
    }
	void Start () {
        Init(6); // 0 to 5
        // Init(7); // 0 to 6
	}

	// Update is called once per frame
	void Update () {

	}
}
