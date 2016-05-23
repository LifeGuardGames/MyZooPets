﻿using UnityEngine;

public class DoctorMatchLifeBarController : MonoBehaviour {
	public RectTransform barTransform;
	public AssemblyLineController lineController;
	private Vector2 barSize;
	private float barPercentage = 1f;
	private float hurtPercentage = -0.05f;
	private float plusPercentage = 0.05f;
	private float startDrainSpeed = .03333f; //Takes 30 seconds
	private float currentDrainSpeed;
	private float drainSpeedIncrement = .001f;
	private bool isDraining = false;

	public float Percentage{
		get {
			return barPercentage;
		}
	}
	public float Speed {
		get {
			return currentDrainSpeed;
		}
	}

	void Start(){
		barSize = barTransform.sizeDelta;
		ResetBar();
	}

	public void ResetBar(){
		isDraining = false;
		currentDrainSpeed = startDrainSpeed;
		barPercentage = 1f;
	}

	public void StartDraining(){
		isDraining = true;
	}

	public void StopDraining(){
		isDraining = false;
	}

	// Want uniform calls in between
	void FixedUpdate(){
		if(isDraining){
			barPercentage -= Time.deltaTime * currentDrainSpeed;
			barTransform.sizeDelta = new Vector2(barSize.x * barPercentage, barSize.y);
			lineController.UpdateVisibleCount(barPercentage);
			if(barPercentage <= 0f){
				NotifyEmpty();
			}
		}
	}
	void OnGUI() {
		//GUI.Box(new Rect(Screen.width/2+150,Screen.height/2, 50, 50), currentDrainSpeed.ToString());
	}
	public void HurtBar(float multiplier = 1f){
		if(isDraining){
			UpdateBarPercentage(hurtPercentage*multiplier);
			currentDrainSpeed-=drainSpeedIncrement;
			if (currentDrainSpeed<startDrainSpeed)
				currentDrainSpeed=startDrainSpeed;
		}
	}

	public void PlusBar(float multiplier = 1f){
		if(isDraining){
			UpdateBarPercentage(plusPercentage*multiplier);
			currentDrainSpeed+=drainSpeedIncrement;
		}
	}

	private void UpdateBarPercentage(float deltaPercent){
		barPercentage += deltaPercent;
		if(barPercentage <= 0f){
			NotifyEmpty();
		} else if (barPercentage > 1) {
			barPercentage=1f;
		}
		barTransform.sizeDelta = new Vector2(barSize.x * barPercentage, barSize.y);
	}

	private void NotifyEmpty(){
		StopDraining();
		DoctorMatchManager.Instance.OnTimerBarEmpty();
	}
}
