﻿using UnityEngine;
using UnityEngine.UI;

public class DoctorMatchLifeBarController : MonoBehaviour{
	public RectTransform barTransform;
	public AssemblyLineController lineController;
	public Text barCount;
	//X Left to clear
	private Vector2 barSize;
	private float barPercentage = 1f;
	private float plusPercentage = 0.01f;
	private float startDrainSpeed = .03333f;
	//Takes 30 seconds
	private float currentDrainSpeed;
	private bool isDraining = false;

	public float Percentage{
		get{
			return barPercentage;
		}
	}

	public float Speed{
		get{
			return currentDrainSpeed;
		}
	}

	public bool IsEmpty{
		get { return barPercentage <= 0f; }
	}

	void Start(){
		barSize = barTransform.sizeDelta;
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
			if(barPercentage <= 0f){
				NotifyEmpty();
			}
		}
	}

	public void PlusBar(float multiplier = 1f){
		UpdateBarPercentage(plusPercentage * multiplier);
	}

	public void UpdateCount(int count){
		if(isDraining || count == -1)
			barCount.text = "";
		else
			barCount.text = count + " Left";
	}

	public void KillBar(){
		UpdateBarPercentage(-1f);
	}

	private void UpdateBarPercentage(float deltaPercent){
		barPercentage += deltaPercent;
		if(barPercentage <= 0f){
			NotifyEmpty();
		}
		else if(barPercentage > 1){
			barPercentage = 1f;
		}
		barTransform.sizeDelta = new Vector2(barSize.x * barPercentage, barSize.y);
	}

	private void NotifyEmpty(){
		StopDraining();
		DoctorMatchManager.Instance.OnTimerBarEmpty();
	}
}
