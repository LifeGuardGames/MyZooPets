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
}
