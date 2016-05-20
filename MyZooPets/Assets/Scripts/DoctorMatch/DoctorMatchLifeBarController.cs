using UnityEngine;

public class DoctorMatchLifeBarController : MonoBehaviour {
	public RectTransform barTransform;
	public AssemblyLineController lineController;
	private Vector2 barSize;
	private float barPercentage = 1f;
	private float hurtPercentage = -0.05f;
	private float plusPercentage = 0.05f;
	private float startDrainSpeed = 0.1f;
	private float currentDrainSpeed;
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
			barPercentage -= Time.deltaTime * startDrainSpeed;
			barTransform.sizeDelta = new Vector2(barSize.x * barPercentage, barSize.y);
			lineController.UpdateVisibleCount(barPercentage);
			if(barPercentage <= 0f){
				NotifyEmpty();
			}
		}
	}

	public void HurtBar(float multiplier = 1f){
		if(isDraining){
			UpdateBarPercentage(hurtPercentage*multiplier);
		}
	}

	public void PlusBar(float multiplier = 1f){
		if(isDraining){
			UpdateBarPercentage(plusPercentage*multiplier);
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
