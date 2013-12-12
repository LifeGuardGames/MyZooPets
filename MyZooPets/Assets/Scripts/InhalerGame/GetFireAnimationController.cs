using UnityEngine;
using System.Collections;

public class GetFireAnimationController : MonoBehaviour {
	
	public ParticleSystemController petFireBackgroundParticle;
	private ParticleSystem petFireBackgroundParticleSystem;
	
	public Transform hudFireIconObject;
	public ParticleSystemController fireActivateParticle;
	
	public TweenAmbientColor tweenColorScript;
	
	public Camera m_Camera;
	public Camera nguiCamera;
	public Transform worldStarPosition;
	public bool retainZDepth;
	private float zDepth;
	
	public float fireWait;
	public float fireDiminishTime;
	
	public float fireStartTime;
	public float fireEndTime;
	private float fireTimeDiff;
	
	public float fireStartSize;
	public float fireEndSize;
	private float fireSizeDiff;
	
	public float fireStartSpeed;
	public float fireEndSpeed;
	private float fireSpeedDiff;
	
	public float lightDimPercentage;
	
	void Start(){
		
		// Place the particle over the HudFireIconObject
		fireActivateParticle.gameObject.transform.position = hudFireIconObject.position;
		
		petFireBackgroundParticleSystem = petFireBackgroundParticle.gameObject.GetComponent<ParticleSystem>();
		
		// Move the Aux to where the fire icon is according to world space
		worldStarPosition.position = m_Camera.ScreenToWorldPoint(nguiCamera.WorldToScreenPoint(hudFireIconObject.position));
		// Retain the Z value of the original gameobject
		if(retainZDepth){
			worldStarPosition.position = new Vector3(worldStarPosition.position.x, worldStarPosition.position.y, petFireBackgroundParticle.transform.position.z);
		}
		
		// Initialize the particles to tween
		fireTimeDiff = fireStartTime - fireEndTime;
		fireSizeDiff = fireStartSize - fireEndSize;
		fireSpeedDiff = fireStartSpeed - fireEndSpeed;
		petFireBackgroundParticleSystem.startLifetime = fireStartTime;
		petFireBackgroundParticleSystem.startSize = fireStartSize;
		petFireBackgroundParticleSystem.startSpeed = fireStartSpeed;
	}
	
	void OnGUI(){
		if(GUI.Button(new Rect(100, 100, 100, 100), "Joe's button!")){
			PlaySequence();
		}
	}
	
	public void PlaySequence(){
		petFireBackgroundParticle.Play();
		
		tweenColorScript.StartTween();
		Invoke("PlaySecondSequence", fireWait);
	}
	
	private void PlaySecondSequence(){
		petFireBackgroundParticle.GetComponent<MoveTowards>().enabled = true;
		Hashtable optional = new Hashtable();
		LeanTween.value(gameObject, "DecreaseParticle", 0, 1, fireDiminishTime, optional);
	}
	
	private void DecreaseParticle(float factor){
		petFireBackgroundParticleSystem.startLifetime = fireStartTime - (fireTimeDiff * factor);
		petFireBackgroundParticleSystem.startSize = fireStartSize - (fireSizeDiff * factor);
		petFireBackgroundParticleSystem.startSpeed = fireStartSpeed - (fireSpeedDiff * factor);
	}
	
	public void PlayGetFireAnimation(){
		fireActivateParticle.Play();
	}
}
