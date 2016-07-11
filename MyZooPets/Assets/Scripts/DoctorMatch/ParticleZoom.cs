using UnityEngine;
using System.Collections;

public class ParticleZoom : MonoBehaviour{
	private int particleCount = 0;
	private ParticleSystem.Particle[] particles;
	private Vector3 particleAim;
	private float particleRunTime = 0;
	private ParticleSystem localSystem;
	private bool started = false;

	public void StartZoom(Vector3 particleAim){
		localSystem = GetComponent<ParticleSystem>();
		particles = new ParticleSystem.Particle[localSystem.maxParticles]; //We will never need more space than this
		this.particleAim = particleAim;
		AnimationCurve newCurve = new AnimationCurve();
		newCurve.AddKey(0, 0); //Constant zero throughout curve
		newCurve.AddKey(1, 0);
		ParticleSystem.LimitVelocityOverLifetimeModule limitModule = localSystem.limitVelocityOverLifetime;
		limitModule.limit = new ParticleSystem.MinMaxCurve(0, newCurve); //Freeze our particles
		particleCount = localSystem.GetParticles(particles); //Cache our particles and set our count
		started = true;
	}

	void Update(){
		if(!started)
			return;
		for(int i = 0; i < particleCount; i++){
			particles[i].position = Vector3.Lerp(particles[i].position, particleAim, particleRunTime);	
		}
		localSystem.SetParticles(particles, particleCount);
		particleRunTime += Time.deltaTime;
		if(particleRunTime > .5){
			particleCount = 0;
			Destroy(gameObject);
		}
	}
}
