using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ProgressBarUIManager : MonoBehaviour {
    public GameObject progressStep;
    public UISlider slider;

    private List<GameObject> sliderNodes; //list of nodes to show game steps
    private int stepCompleted;
    private float increment; //How much to increment the slider by

    void Awake(){
        //Set up the progress slider
        slider.sliderValue = 0;
        slider.numberOfSteps = InhalerLogic.RESCUE_NUM_STEPS;
        increment = 1.0f / (slider.numberOfSteps - 1);
        stepCompleted = 0;

        sliderNodes = new List<GameObject>();
        SetUpProgressSteps();
        UpdateNodeColors();
    }

    void Start(){
        InhalerLogic.OnNextStep += UpdateProgressBar;
    }
    
    void OnDestroy(){
        InhalerLogic.OnNextStep -= UpdateProgressBar;
    }

    //Event listener. listens to OnNext Step and Fill progress bar by one node
    private void UpdateProgressBar(object sender, EventArgs args){
        stepCompleted = InhalerLogic.Instance.CurrentStep;
        slider.sliderValue = stepCompleted * increment;
        UpdateNodeColors();
    } 

    // Set up the sliderNodes, based on how the slider is set up.
    private void SetUpProgressSteps(){
        Transform foreground = slider.transform.Find("Foreground");
        float width = foreground.transform.localScale.x;
        float increment = width / (slider.numberOfSteps - 1);

        for (int i = 0; i < slider.numberOfSteps; i++){
            GameObject node = NGUITools.AddChild(this.gameObject, progressStep);
            node.layer = LayerMask.NameToLayer("NGUI");
            node.transform.localPosition = new Vector3(i * increment, 0, 0);
            string stepNumber = (i + 1).ToString();

            UILabel label = node.transform.Find("Label").GetComponent<UILabel>();
            label.text = stepNumber; 
            sliderNodes.Add(node);
        }
    }

    // Loop through all step sliderNodes, and set their colors accordingly.
    private void UpdateNodeColors(){
        for (int i = 0; i < sliderNodes.Count; i++){
            GameObject stepObject = sliderNodes[i].transform.Find("Sprite").gameObject;

            if (i <= stepCompleted){
                stepObject.GetComponent<UISprite>().spriteName="circleRed";
                if(i == stepCompleted){
                    stepObject.transform.parent.GetComponent<AnimationControl>().Play();
                }
            }
            else {
                stepObject.GetComponent<UISprite>().spriteName="circleGray";
            }
        }
    }
}
