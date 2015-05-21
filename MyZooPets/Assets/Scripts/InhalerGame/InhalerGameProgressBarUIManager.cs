using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InhalerGameProgressBarUIManager : Singleton<InhalerGameProgressBarUIManager> {
    public GameObject progressStep; //prefab that will be initiated for each steps
    public UISlider slider;

    private List<GameObject> sliderNodes; //list of nodes to show game steps
    private float increment; //How much to increment the slider by

	private int sliderStep = 0; //current step of the slider
	private int nodeStep = 0; //current step of the color node

    void Awake(){
        //Set up the progress slider
        slider.sliderValue = 0;
        slider.numberOfSteps = InhalerLogic.RESCUE_NUM_STEPS-1;
        increment = 1.0f / (slider.numberOfSteps - 1);
        sliderNodes = new List<GameObject>();
        SetUpProgressSteps();
    }

    void Start(){
        InhalerLogic.OnNextStep += UpdateProgressBar;
		InhalerLogic.OnNextStep += UpdateNodeColors;
		Inhale.finish += UpdateProgressBar;
		Inhale.finish += UpdateNodeColors;
    }
    
    void OnDestroy(){
		InhalerLogic.OnNextStep -= UpdateProgressBar;
		InhalerLogic.OnNextStep -= UpdateNodeColors;
		Inhale.finish -= UpdateProgressBar;
		Inhale.finish -= UpdateNodeColors;
    }

	/// <summary>
	/// Updates the node colors. Last node of the progress bar is called from
	/// Inhale.cs because OnNextStep listener is not called until the game quits
	/// </summary>
	public void UpdateNodeColors(){
		UpdateNodeColors(this, EventArgs.Empty);
	}

	// Set up the sliderNodes, based on how the slider is set up.
	private void SetUpProgressSteps(){
		Transform foreground = slider.transform.Find("Foreground");
		float width = foreground.transform.localScale.x;
		float increment = width / (slider.numberOfSteps - 1);
		
		for (int stepNum = 0; stepNum < slider.numberOfSteps; stepNum++){
			GameObject node = NGUITools.AddChild(this.gameObject, progressStep);
			node.layer = LayerMask.NameToLayer("NGUI");
			node.transform.localPosition = new Vector3(stepNum * increment, 0, 0);
			
			UILabel label = node.transform.Find("Label").GetComponent<UILabel>();
			label.text = (stepNum + 1).ToString();
			sliderNodes.Add(node);
		}
	}

	// Increase slider by one step
    private void UpdateProgressBar(object sender, EventArgs args){
		sliderStep++;
    	slider.sliderValue = sliderStep * increment;
    } 

	// Increase the node color by one step
    private void UpdateNodeColors(object sender, EventArgs args){
		if(nodeStep < sliderNodes.Count){
			GameObject nodeObject = sliderNodes[nodeStep];
			GameObject nodeSpriteObject = nodeObject.transform.Find("Sprite").gameObject;
			nodeSpriteObject.GetComponent<UISprite>().spriteName = "greenCircle";
			nodeSpriteObject.GetComponent<ParticleSystemController>().Play();
			nodeStep++;
		}
    }
}
