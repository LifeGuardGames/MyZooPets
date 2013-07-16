using UnityEngine;
using System.Collections;

public class ProgressBarController : MonoBehaviour {

    public UISlider slider; // for setup
    public ProgressBarAnimation animation;

    int increment;

    /*
        numOfNodes includes step 0. So if numOfNodes were '3', then the steps
        included would be 0, 1, 2.
    */
    public void Init(int numOfNodes){
        slider.sliderValue = 0;
        slider.numberOfSteps = numOfNodes;
        increment = 1 / numOfNodes;
        if (animation != null) animation.Init();
    }
    // testing
	void Start () {
        Init(6); // 0 to 5
        // Init(7); // 0 to 6
	}

	// Update is called once per frame
	void Update () {

	}

    public void UpdateStep(int currentStep){
        slider.sliderValue = currentStep * increment;
    }

}
