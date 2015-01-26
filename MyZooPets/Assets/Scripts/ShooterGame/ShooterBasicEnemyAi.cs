using UnityEngine;
using System.Collections;

public class ShooterBasicEnemyAi :EnemyAiHandeler{
	// Use this for initialization
	void Start () {
		Speed = 2.5f;
		ScoreVal=1;
		Damage = 1;
		health=1;
		LeanTween.moveX(this.gameObject,Player.transform.position.x,Speed);
	}
}
