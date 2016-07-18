using UnityEngine;
using System.Collections;

/* Both SpawnFirework and SpawnFloatyText access ComboController through DoctorMatchManager
 * but do not ping DoctorMatchManager
 */
public class ParticleFXController : MonoBehaviour{
	public GameObject fireworkPrefab;
	public GameObject floatyPrefab;
	public RectTransform scoreTransform;
	public RectTransform counterTransform;
	public RectTransform barTransform;

	private Vector3 particleAim;
	private float particleRunTime;
	private float horizontalRange = 80;
	private float verticalHeight = 180;

	public IEnumerator SpawnFirework(float comboMod, Vector3 startPosition){
		GameObject firework = Instantiate(fireworkPrefab);
		firework.transform.position = startPosition;
		ParticleSystem pSystem = firework.GetComponent<ParticleSystem>();
		float comboBonusScalar = 1; //Applied to the size during these special ones
		Vector3 toAim;
		float yieldTime = .2f;
		if(DoctorMatchManager.Instance.comboController.ComboLevel == 2){ //Big combo bonus
			pSystem.startColor = Color.blue;
			comboBonusScalar = 1.1f;
			toAim = counterTransform.position;
			yieldTime = .6f;
		}
		else if(DoctorMatchManager.Instance.comboController.ComboLevel == 1){ //Small combo bonus
			pSystem.startColor = Color.green;
			comboBonusScalar = 1.1f;
			toAim = scoreTransform.position;
			yieldTime = .4f;
		}
		else{
			toAim = DoctorMatchManager.Instance.comboController.comboText.transform.position;//GetComboPosition(DoctorMatchManager.Instance.comboController.Combo);
		}
		ParticleSystem.LimitVelocityOverLifetimeModule emissionModule = pSystem.limitVelocityOverLifetime; //HACK: Currently, you cannot modify particle system module curves directly, so we save it here and modify it later
		AnimationCurve ourCurve = new AnimationCurve();
		for(float i = 0; i <= 1; i += .1f){
			ourCurve.AddKey(i, 250 * Mathf.Pow(i - 1, 4)); //Kind of like quadratic but it gets roughly flat after .5
		}
		emissionModule.limit = new ParticleSystem.MinMaxCurve((1 + comboMod / 20) * comboBonusScalar, ourCurve);
		pSystem.startSize *= (1 + comboMod / 10) * comboBonusScalar;
		yield return new WaitForSeconds(yieldTime); //Need to wait for the burst to fully stretch before we move it
		pSystem.GetComponent<ParticleZoom>().StartZoom(toAim);
	}

	public void SpawnFloatyText(float comboMod, bool correct, Transform buttonTransform){
		string wordText;
		string comboText;
		Color color;
		int size = 40 + (int)(comboMod * 2f);//= Random.Range(75, 95);
		int index = Random.Range(0, 9);
		if(correct){ //TODO: Color differently based on combo
			wordText = Localization.Localize("DOCTOR_RIGHT_" + index);
			comboText = "x" + DoctorMatchManager.Instance.comboController.Combo;
			color = new Color(0, 1 - (10 - comboMod) / 30, 0);//new Color(Random.Range(.0f, .4f), Random.Range(.7f, 1f), Random.Range(.0f, .4f));
		}
		else{
			comboText = "X";
			wordText = Localization.Localize("DOCTOR_WRONG_" + index);
			color = new Color(Random.Range(.7f, 1f), Random.Range(.0f, .2f), Random.Range(.0f, .2f));
		}
		float horizontalOffset = barTransform.position.x - buttonTransform.position.x;

		UGUIFloaty wordFloaty = Instantiate(floatyPrefab).GetComponent<UGUIFloaty>();
		Vector3 wordOffset = new Vector3(horizontalOffset + Random.Range(-horizontalRange, horizontalRange), verticalHeight);
		Vector3 spawnPos = buttonTransform.position + new Vector3(0, 25);//buttonObject.transform.position-(Vector3)buttonObject.GetComponent<RectTransform>().rect.center;
		wordFloaty.transform.SetParent(buttonTransform, true);
		wordFloaty.transform.localScale = new Vector3(1, 1, 1);
		wordFloaty.StartFloaty(spawnPos, text: wordText, textSize: size, riseTime: .6f, toMove: wordOffset, color: color);

		float comboBonusScalar = 1; //Applied to the size during these special ones
		if(DoctorMatchManager.Instance.comboController.ComboLevel == 2){ //Big combo bonus
			comboBonusScalar = 1.5f;
		}
		else if(DoctorMatchManager.Instance.comboController.ComboLevel == 1){ //Small combo bonus
			comboBonusScalar = 1.25f;
		}

		Vector3 comboOffset = new Vector3(horizontalOffset + Random.Range(-horizontalRange, horizontalRange), verticalHeight * .8f);
		UGUIFloaty comboFloaty = Instantiate(floatyPrefab).GetComponent<UGUIFloaty>();
		comboFloaty.transform.SetParent(buttonTransform, true);
		comboFloaty.transform.localScale = new Vector3(1, 1, 1);
		comboFloaty.StartFloaty(spawnPos, text: comboText, textSize: (int)(size / 1.2 * comboBonusScalar), riseTime: .6f, toMove: comboOffset, color: color); //Shrink the number slightly

		comboFloaty.GetComponent<Canvas>().overrideSorting = true;
		wordFloaty.GetComponent<Canvas>().overrideSorting = true;

	}
}
