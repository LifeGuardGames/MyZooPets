using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgressBarAnimation : MonoBehaviour {

    public UISlider slider; // for setup
    public GameObject progressStep;
    private List<GameObject> markers;

	public void Init () {
        markers = new List<GameObject>();
        SetUpProgressSteps();
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
            markers.Add(marker);
        }
    }

    public void OnSliderChange(){
        int step = (int) (slider.sliderValue * (slider.numberOfSteps - 1)); // same as slider.sliderValue / (1 / (slider.numberOfSteps - 1))
        Debug.Log("Step " + step + " completed.");
    }
}
