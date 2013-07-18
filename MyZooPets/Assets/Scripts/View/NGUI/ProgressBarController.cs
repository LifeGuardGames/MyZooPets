using UnityEngine;
using System.Collections;

public class ProgressBarController : MonoBehaviour {

    public UISlider slider; // for setup
    public ProgressBarAnimation anim;

    float increment;

    /*
        numOfNodes includes step 0. So if numOfNodes were '3', then the steps
        included would be 0, 1, 2.
    */
    public void Init(int numOfNodes){
        slider.sliderValue = 0;
        slider.numberOfSteps = numOfNodes;
        increment = 1.0f / (numOfNodes - 1);
        if (anim != null) anim.Init(numOfNodes);
    }
    // testing
	// void Start () {
 //        Init(6); // 0 to 5
 //        // Init(7); // 0 to 6
	// }

	// Update is called once per frame
	void Update () {

	}

    public void UpdateStep(int lastCompletedStep){
        slider.sliderValue = lastCompletedStep * increment;
        anim.UpdateStep(lastCompletedStep);
    }

}
