using UnityEngine;
using System.Collections;

public class ShooterPowerUpManager : Singleton<ShooterPowerUpManager>{
	public enum PowerUpType{
		Normal,
		Triple,
		Pierce
	}

	public float timer;

	public void ChangePowerUp(PowerUpType powerUp){
		switch(powerUp){
		case PowerUpType.Normal:
			PlayerShooterController.Instance.IsPiercing = false;
			PlayerShooterController.Instance.IsTriple = false;
			// Refresh its own state so it knows what size fireball to give
			PlayerShooterController.Instance.ChangeFire();
			break;
		case PowerUpType.Triple:
			AudioManager.Instance.PlayClip("shooterPowerUp");
			PlayerShooterController.Instance.IsTriple = true;
			StopCoroutine(ResetPowerUP());
			StartCoroutine(ResetPowerUP());
			break;
		case PowerUpType.Pierce:
			AudioManager.Instance.PlayClip("shooterPowerUp");
			PlayerShooterController.Instance.IsPiercing = true;
			StopCoroutine(ResetPowerUP());
			StartCoroutine(ResetPowerUP());
			break;
		}
	}

	// Powering down from power up
	private IEnumerator ResetPowerUP(){
		yield return new WaitForSeconds(timer);
		ChangePowerUp(PowerUpType.Normal);
	}
}
