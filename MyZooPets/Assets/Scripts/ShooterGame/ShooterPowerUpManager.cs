using UnityEngine;
using System.Collections;

public class ShooterPowerUpManager : Singleton<ShooterPowerUpManager>{
	public enum PowerUpType{
		Normal,
		Triple,
		Bouncy
	}

	public float timer;

	public void ChangePowerUp(PowerUpType powerUp){
		switch(powerUp){
		case PowerUpType.Normal:
			PlayerShooterController.Instance.IsTriple = false;
			ShooterGameManager.Instance.BouncyWalls.SetActive(false);
			// Refresh its own state so it knows what size fireball to give
			PlayerShooterController.Instance.ChangeFire();
			break;
		case PowerUpType.Triple:
			AudioManager.Instance.PlayClip("shooterPowerUp");
			PlayerShooterController.Instance.IsTriple = true;
			StopCoroutine(ResetPowerUP());
			StartCoroutine(ResetPowerUP());
			break;
		case PowerUpType.Bouncy:
			AudioManager.Instance.PlayClip("shooterPowerUp");
			ShooterGameManager.Instance.BouncyWalls.SetActive(true);
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