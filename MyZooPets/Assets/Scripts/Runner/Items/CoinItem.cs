using UnityEngine;
using System.Collections;

public class CoinItem : RunnerItem {
	public int coinValue = 1;
	public int CoinValue {
		get { return coinValue; }
		set { coinValue = value; }
	}

	public Color activeColor;
	public float fPitchPerCoin;     // pitch change per coin streak

	public CoinItem nextCoin;
	public CoinItem NextCoin {
		set { nextCoin = value; }
	}

	private bool magnetized;
	private bool collectedInLine = false;
	private float coinSpeed = .15f;
	private float coinToCombo = .02f;
	private float speed = 0;
	private float acceleration = 0;

	//Set under Magnetize
	private bool lastCoin = false;
	public bool LastCoin {
		set { lastCoin = value; }
	}

	public bool NextToCollect {
		set {
			collectedInLine = value;
			if(value) {
				GetComponent<SpriteRenderer>().color = activeColor;
			}
		}
	}

	void Start() {
		MegaHazard.Instance.SpeedUp(coinValue);
	}

	void Update() {
		if(magnetized && !RunnerGameManager.Instance.IsPaused) {
			acceleration = 2 * PlayerController.Instance.Speed;
			//Vector3 aimPosition = PlayerController.Instance.transform.position+PlayerController.Instance.collider.
			Vector3 aimPosition = PlayerController.Instance.GetComponent<Collider>().bounds.center;
			transform.position = Vector3.Lerp(transform.position, aimPosition, speed * Time.deltaTime);
			speed += acceleration * Time.deltaTime;
			if(Vector3.Distance(transform.position, aimPosition) < .5f) {
				transform.position = aimPosition;
			}
		}
	}

	public void Magnetize() {
		magnetized = true;
	}

	public override void OnPickup() {
		// picking up coins plays a special sound, with the pitch depending on the coin streak
		Hashtable hashOverride = new Hashtable();
		hashOverride["Pitch"] = GetCoinStreakPitch();
		hashOverride["Volume"] = 0.5f;
		AudioManager.Instance.PlayClip("runnerCoin", option: hashOverride);

		RunnerGameManager.Instance.AddCoins((int)Mathf.Sqrt(coinValue)); //Sqrt is used because it makes consecutive strings less super high valued compared to short strings.
		Destroy(gameObject); //Still, it gives a better scaling than 1 point coins.
		MegaHazard.Instance.IncrementHealth(coinValue);
		PlayerController.Instance.IncreaseSpeed(coinSpeed);
		if(!lastCoin) {
			if(nextCoin) {
				nextCoin.NextToCollect = collectedInLine;
			}
		}
		else if(collectedInLine || magnetized) { //If there is no coin after us we are the last in line. Sometimes when we are magnetized we are collected in incorrect order, but if the last one is magnetized so is every other one
			Bonus(); //If we were collected in order, activate bonus!
		}
	}

	//---------------------------------------------------
	// GetCoinStreakPitch()
	// Based on the player's coin streak, returns the
	// pitch at which the coin sound should play.
	//---------------------------------------------------
	public float GetCoinStreakPitch() {
		int nStreak = RunnerGameManager.Instance.CoinStreak;
		float fPitch = 1.0f + (nStreak * fPitchPerCoin);
		return fPitch;
	}

	private void Bonus() {
		SpawnFloatyCoin();
		RunnerGameManager.Instance.IncrementCombo(coinToCombo * Mathf.Sqrt(coinValue));
	}
}
