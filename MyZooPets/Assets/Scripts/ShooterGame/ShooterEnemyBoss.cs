﻿using UnityEngine;
using System.Collections;

public class ShooterEnemyBoss : ShooterEnemy {
	public GameObject bulletPrefab;
	private GameObject bulletAux;
	private GameObject upper;
	private GameObject lower;
	private GameObject midPoint;
	
	// Use this for initialization
	void Start(){
		animator.SetBool("IsSpitMode", true);
		// movement positions
		upper = GameObject.Find("Upper");
		lower = GameObject.Find("Lower");
		midPoint = GameObject.Find("MidPoint");
		LeanTween.move(gameObject, midPoint.transform.position, moveDuration).setOnComplete(StartRepeatMove);
	}
	
	// function that moves around the smog monster between two points randomly
	void StartRepeatMove(){
		if(Random.Range(0, 2) == 0){
			LeanTween.move(gameObject, upper.transform.position, moveDuration).setOnComplete(RepeatMove);
			StartCoroutine(WaitASecond());
		}
		else{
			LeanTween.move(gameObject, lower.transform.position, moveDuration).setOnComplete(RepeatMove);
			StartCoroutine(WaitASecond());
		}
	}
	
	// shoots a smog ball at the player
	void ShootSmogBall(){
		if(!isDead){	
			AudioManager.Instance.PlayClip("shooterEnemySpit");
			animator.SetBool("Spit",true);
			animator.SetBool("IsSpitMode", false);
			bulletAux = GameObjectUtils.AddChild(null, bulletPrefab);
			bulletAux.transform.position = transform.position;
			LeanTween.moveX(bulletAux, player.transform.position.x, 2.0f);
			StartCoroutine(WaitASecond());
		}
	}
	
	// gives a 2 sec breather between shots
	IEnumerator WaitASecond(){
		yield return new WaitForSeconds(0.2f);
		ShootSmogBall();
	}
	
	void RepeatMove(){
		if(transform.position == upper.transform.position){
			LeanTween.move(gameObject, lower.transform.position, moveDuration).setOnComplete(RepeatMove);
		}
		else{
			LeanTween.move(gameObject, upper.transform.position, moveDuration).setOnComplete(RepeatMove);
		}
	}
	
	void OnDestroy(){
		ShooterSpawnManager.Instance.BossDead();
		LeanTween.cancel(gameObject);
		
		if(bulletAux != null){
			Destroy(bulletAux);
		}
	}
}
