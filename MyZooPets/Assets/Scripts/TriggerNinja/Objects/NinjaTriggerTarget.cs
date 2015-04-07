﻿using UnityEngine;
using System.Collections;

/// <summary>
/// This is like a piece of fruit from Fruit Ninja
/// it is a positive object that the player wants to destroy.
/// </summary>
public class NinjaTriggerTarget : NinjaTrigger{

	public int points = 1;			// how much is this trigger worth when the player cuts it?
	public Renderer rendererFace;	// renderer for cockroach face

	protected override void Start(){
		base.Start();	
		
		// pick a face for this roach
		int totalFacesCount = Constants.GetConstant<int>("Ninja_NumFaces");
		string faceKey = Constants.GetConstant<string>("Ninja_FaceKey");
		int randomFace = Random.Range(1, totalFacesCount + 1); // faces index starts at 1, so get 1-max inclusive
		SetFace(faceKey + randomFace);
	}
	
	//---------------------------------------------------
	// SetFace()
	// Sets this roach's face to the incoming string
	// referenced material.
	//---------------------------------------------------	
	private void SetFace(string faceString){
		Material loadedMaterial = Resources.Load(faceString) as Material;
		
		if(loadedMaterial != null){
			rendererFace.material = loadedMaterial;
		}
		else{
			Debug.LogError("Attempting to set cockroach face to non-existant material with face " + faceString);
		}
	}
		
	protected override void _OnCut(){
		// award points
		NinjaManager.Instance.UpdateScore(points);

		if(!NinjaManager.Instance.bonusRound){
			NinjaManager.Instance.increaseChain();
		}
		else if(NinjaManager.Instance.bonusRoundEnemies != 0){
			NinjaManager.Instance.bonusRoundEnemies--;
			NinjaManager.Instance.CheckBonusRound();
		}
		// increase the player's combo
		NinjaManager.Instance.IncreaseCombo(1);
		
		// set the cockroach's face to dead
		string strFaceKey = Constants.GetConstant<string>("Ninja_FaceKey");
		SetFace(strFaceKey + "Dead");	

		Destroy(this.gameObject);
	}
	
	//---------------------------------------------------
	// _OnMissed()
	//---------------------------------------------------	
	protected override void _OnMissed(){
		if(!NinjaManager.Instance.IsTutorialRunning()){
			// the player loses a life
			if(NinjaManager.Instance.bonusRound == false){
			NinjaManager.Instance.UpdateLives(-1);
			NinjaManager.Instance.resetChain();
			}
			else if (NinjaManager.Instance.bonusRoundEnemies !=0){
				NinjaManager.Instance.bonusRoundEnemies--;
				NinjaManager.Instance.CheckBonusRound();
			}
		}
	}	
}
