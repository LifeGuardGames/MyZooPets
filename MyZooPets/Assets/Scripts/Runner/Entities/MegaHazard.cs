using UnityEngine;
using System.Collections;

public class MegaHazard : Singleton<MegaHazard>{
	public ParticleSystem hazardParticle;
	public ParticleSystem hazardParticle2;
	public Transform bottomPosition;
	public float maxOffset = 24;
	//How far to the left of the player should we be when they are at max health
	public float startingHealth = 100;
	private float health;
	private float healthTick = 1.5f;
	private float currentOffset;
	private float lerpRate = .5f;
	private float graceTime = 0;
	private float runTime = 0;
	private bool lost = false;
	private float maxTicked = 0;
	private float baseMaxHealth = 100;
	private float maxHealthMultiplier = 15;
	//Max health is base + healthTick*multiplier
	// Use this for initialization
	void Start(){
		Reset();
		hazardParticle.Play();
		hazardParticle2.Play();
	}
	
	// Update is called once per frame
	void Update(){
		if(RunnerGameManager.Instance.IsPaused){
			return;
		}
		runTime += Time.deltaTime;
		graceTime -= Time.deltaTime;
		HandleHealth();
		HandlePosition();
	}

	public void IncrementHealth(float increment){
		health += increment;
	}

	public void TriggerSlowdown(){
		health = Mathf.Pow(health, .75f);
		if(runTime > 30 && graceTime <= 0){ //If they are more than 30 seconds in to a run, they get a second chance, however, if they have already hit a hazard recently, they do not get this chance
			graceTime = runTime / 3; //The higher grace time is set, the longer you must wait to be saved again 
			healthTick /= 2f; //Slow down our speed to give them recovery time
		}
		else{
			healthTick *= 1.7f;
		}
	}

	public void SpeedUp(float increment){
		increment = Mathf.Sqrt(increment) / 30;
		if(healthTick > 40 || PlayerController.Instance.StarMode){ //Cap our speed at 40 ish and dont increase if we are star mode
			increment *= 0;
		}
		else if(healthTick > 30){ //At a certain point, it gets ridiculous, so help the player out.
			increment *= .3f;
		}
		else if(healthTick > 15){
			increment *= .6f;
		}
		healthTick += increment;
	}

	public void Reset(){
		healthTick = 0;
		health = startingHealth;
		runTime = 0;
		lost = false;
	}

	public void PauseParticles(){
		hazardParticle.Pause();
		hazardParticle2.Pause();
	}

	public void PlayParticles(){
		hazardParticle.Play();
		hazardParticle2.Play();
	}

	private void HandleHealth(){ //Caps our health and tick rate, and decrements it. Also handles losing
		if(healthTick > maxTicked){
			maxTicked = healthTick;
		}
		if(health > baseMaxHealth + healthTick * maxHealthMultiplier){
			health = baseMaxHealth + healthTick * maxHealthMultiplier;
		}
		if(health <= 0 && !lost){ 
			lost = true;
		}
		else if(currentOffset < 1 && lost){
			RunnerGameManager.Instance.GameOver();
		}
		if(PlayerController.Instance.StarMode){
			health += healthTick * .75f * Time.deltaTime;
		}
		else{
			health -= healthTick * Time.deltaTime;
		}
	}

	private void HandlePosition(){
		float percentage = health / (baseMaxHealth + healthTick * maxHealthMultiplier);
		percentage = Mathf.Clamp01(percentage);
		if(lost)
			currentOffset = Mathf.Lerp(currentOffset, 0, lerpRate * Time.deltaTime); //+2 looks better and makes it seem close but not impossible when percentage is very low
		else if(PlayerController.Instance.StarMode)
			currentOffset = Mathf.Lerp(currentOffset, maxOffset + 8, 2.5f * lerpRate * Time.deltaTime); //Go offscreen and give them a moment of glory
		else
			currentOffset = Mathf.Lerp(currentOffset, percentage * maxOffset + 2, lerpRate * Time.deltaTime);
		transform.position = new Vector3(PlayerController.Instance.transform.position.x - currentOffset, transform.position.y, transform.position.z);
	}
}
