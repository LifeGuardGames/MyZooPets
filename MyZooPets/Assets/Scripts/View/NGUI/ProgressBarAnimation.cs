using UnityEngine;
using System.Collections;

public class ProgressBarAnimation : MonoBehaviour {

    public UISlider slider; // for setup
    public GameObject progressStep;

	void Start () {

        SetUpProgressSteps();
	}

	// Update is called once per frame
	void Update () {

	}

    void SetUpProgressSteps(){
        Transform foreground = slider.transform.Find("Foreground");
        float width = foreground.transform.localScale.x;
        float increment = width / (slider.numberOfSteps - 1);
        for (int i = 0; i < slider.numberOfSteps; i++){

            GameObject marker = Instantiate(progressStep) as GameObject;
            // Vector3 originalPos = marker.transform.position;
            marker.transform.parent = gameObject.transform;
            marker.transform.localScale = Vector3.one;
            // marker.transform.localPosition = originalPos;
            marker.transform.localPosition = new Vector3(i * increment, 0, 0);

            UILabel label = marker.transform.Find("Label").GetComponent<UILabel>();
            label.text = i.ToString();
        }
    }

    public void OnSliderChange(){
        Debug.Log("slider changed");
        // slider.sliderValue;
    }
}
