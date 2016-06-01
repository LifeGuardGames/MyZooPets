using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoinItem : RunnerItem {
    public int CoinValue = 1;
	// pitch change per coin streak
	public float fPitchPerCoin;
	public CoinItem nextCoin;
	private bool magnetized;
	private bool collectedInLine=false;
	private float CoinSpeed = .15f;
	private bool lastCoin=false;
	private float CoinToCombo = .02f;
	private float speed = 0;
	private float acceleration=0; //Set under Magnetize
	public bool LastCoin {
		set {
			lastCoin=value;
		}
	}
	public bool NextToCollect {
		set {
			collectedInLine=value;
			if (value){
				GetComponentInChildren<tk2dSprite>().color=Color.red;
			}
		}
	}
	public bool Magnetize {
		set {
			magnetized=value;
			acceleration=PlayerController.Instance.Speed;
		}
	}
	// Use this for initialization
	public override void Start () {
        base.Start();
		MegaHazard.Instance.SpeedUp(CoinValue);
	}
	
	// Update is called once per frame
    public override void Update() {
        base.Update();
		if (magnetized) {
			//Vector3 aimPosition = PlayerController.Instance.transform.position+PlayerController.Instance.collider.
			transform.position = Vector3.Lerp(transform.position,PlayerController.Instance.collider.bounds.center,speed*Time.deltaTime);
			speed+=acceleration*Time.deltaTime;
		}
	}
    public override void OnPickup(){
		// picking up coins plays a special sound, with the pitch depending on the coin streak
		float fPitch = GetCoinStreakPitch();
		Hashtable hashOverride = new Hashtable();
		hashOverride["Pitch"] = fPitch;
        hashOverride["Volume"] = 0.5f;
		AudioManager.Instance.PlayClip("StarSingle", option: hashOverride );
		ScoreManager.Instance.AddCoins((int)Mathf.Sqrt(CoinValue)); //Sqrt is used because it makes consecutive strings less super high valued compared to short strings.
        GameObject.Destroy(gameObject); //Still, it gives a better scaling than 1 point coins.
		MegaHazard.Instance.IncrementHealth(CoinValue);
		PlayerController.Instance.IncreaseSpeed(CoinSpeed);
		if (!lastCoin){
			if (nextCoin)
				nextCoin.NextToCollect=collectedInLine;
		} else if (collectedInLine||magnetized) { //If there is no coin after us we are the last in line. Sometimes when we are magnetized we are collected in incorrect order, but if the last one is magnetized so is every other one
			Bonus(); //If we were collected in order, activate bonus!
		}
    }
	//---------------------------------------------------
	// GetCoinStreakPitch()
	// Based on the player's coin streak, returns the
	// pitch at which the coin sound should play.
	//---------------------------------------------------	
	public float GetCoinStreakPitch() {
		int nStreak = ScoreManager.Instance.GetCoinStreak();
		float fPitch = 1.0f + ( nStreak * fPitchPerCoin );

		return fPitch;
	}
	private void Bonus(){
		SpawnFloatyText(floatingTime: .25f);
		ScoreManager.Instance.IncrementCombo(CoinToCombo*Mathf.Sqrt(CoinValue));
	}

}
