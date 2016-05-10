using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoctorMatchLifeBarController : MonoBehaviour {
	public RectTransform barTransform;
	private Vector2 barSize;
	private float barPercentage = 1f;
	private float hurtPercentage = -0.03f;
	private float plusPercentage = 0.05f;
	private float startDrainSpeed = 0.1f;

	private float currentDrainSpeed;
	private bool isDraining = false;

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
			barPercentage -= Time.deltaTime * startDrainSpeed;
			barTransform.sizeDelta = new Vector2(barSize.x * barPercentage, barSize.y);

			if(barPercentage <= 0f){
				NotifyEmpty();
			}
		}
	}

	public void HurtBar(){
		if(isDraining){
			UpdateBarPercentage(hurtPercentage);
		}
	}

	public void PlusBar(){
		if(isDraining){
			UpdateBarPercentage(plusPercentage);
		}
	}

	private void UpdateBarPercentage(float deltaPercent){
		barPercentage += deltaPercent;
		barTransform.sizeDelta = new Vector2(barSize.x * barPercentage, barSize.y);

		if(barPercentage <= 0f){
			NotifyEmpty();
		}
	}

	private void NotifyEmpty(){
		Debug.Log("Notify empty");
		StopDraining();
		DoctorMatchManager.Instance.OnTimerBarEmpty();
	}
}
