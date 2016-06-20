﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InhalerGameProgressBarUIManager : Singleton<InhalerGameProgressBarUIManager> {
    public GameObject progressStep; //prefab that will be initiated for each steps

    public List<GameObject> sliderNodes; //list of nodes to show game steps

	private int sliderStep = 0; //current step of the slider
	private int nodeStep = 0; //current step of the color node


    void Start(){
        InhalerLogic.OnNextStep += UpdateProgressBar;
		Inhale.finish += UpdateProgressBar;
    }
    
    void OnDestroy(){
		InhalerLogic.OnNextStep -= UpdateProgressBar;
		Inhale.finish -= UpdateProgressBar;
    }


	// Increase slider by one step
    private void UpdateProgressBar(object sender, EventArgs args){
		//change this to fancy animation
		sliderNodes[sliderStep].SetActive(true);
		sliderStep++;
    } 
}
