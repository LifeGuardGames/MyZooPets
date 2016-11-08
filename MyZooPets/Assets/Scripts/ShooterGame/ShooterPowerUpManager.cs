using UnityEngine;
using System.Collections;

public class ShooterPowerUpManager : Singleton<ShooterPowerUpManager>{
	public enum PowerUpType{
		Normal,
		Triple,
		Bouncy,
		Inhaler,
		MiniPet
	}

	public MiniPetPowerUp buddy;
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
				PlayerShooterController.Instance.PlayPowerUpEffects();
				PlayerShooterController.Instance.IsTriple = true;
				StopCoroutine(ResetPowerUP());
				StartCoroutine(ResetPowerUP());
				break;
			case PowerUpType.Bouncy:
				PlayerShooterController.Instance.PlayPowerUpEffects();
				ShooterGameManager.Instance.BouncyWalls.SetActive(true);
				StopCoroutine(ResetPowerUP());
				StartCoroutine(ResetPowerUP());
				break;
			case PowerUpType.Inhaler:
				PlayerShooterController.Instance.PlayFlameUpEffects();
				ShooterInhalerManager.Instance.OnShooterGameInhalerButton();
				break;
			case PowerUpType.MiniPet:
				PlayerShooterController.Instance.PlayPowerUpEffects();
				buddy.WakeUp();
				break;
			}
	}

	// Powering down from power up
	private IEnumerator ResetPowerUP(){
		yield return new WaitForSeconds(timer);
		ChangePowerUp(PowerUpType.Normal);
	}
}