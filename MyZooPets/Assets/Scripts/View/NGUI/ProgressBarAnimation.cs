using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgressBarAnimation : MonoBehaviour {

    public UISlider slider; // for setup
    public GameObject progressStep;
    private List<GameObject> markers;
    private int stepCompleted;
    private bool initCalled = false;

    // Call this once at the beginning, after having instantiated the Progress Bar.
	public void Init () {
        // Destroy all old markers if there are any.
        if (markers != null && markers.Count > 0){
            foreach (GameObject marker in markers){
                Destroy(marker);
            }
        }

        initCalled = true;

        markers = new List<GameObject>();
        SetUpProgressSteps();
        UpdateMarkerColors();
	}

    // Set up the markers, based on how the slider is set up.
    void SetUpProgressSteps(){
        Transform foreground = slider.transform.Find("Foreground");
        float width = foreground.transform.localScale.x;
        float increment = width / (slider.numberOfSteps - 1);
        for (int i = 0; i < slider.numberOfSteps; i++){

            GameObject marker = NGUITools.AddChild(gameObject, progressStep);
            marker.transform.localPosition = new Vector3(i * increment, 0, 0);

            UILabel label = marker.transform.Find("Label").GetComponent<UILabel>();
            label.text = i.ToString();
            markers.Add(marker);
        }
    }

    public void UpdateStep(int lastCompletedStep){
        if (!initCalled) return;

        // Update the number of steps completed.
        stepCompleted = lastCompletedStep;

        UpdateMarkerColors();
    }

    // Loop through all step markers, and set their colors accordingly.
    void UpdateMarkerColors(){
        for (int i = 0; i < markers.Count; i++){
            if (i <= stepCompleted){
                markers[i].transform.Find("Sprite").GetComponent<UISprite>().spriteName="circleRed";
            }
            else {
                markers[i].transform.Find("Sprite").GetComponent<UISprite>().spriteName="circleGray";
            }
        }
    }
}
