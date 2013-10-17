using UnityEngine;
using System.Collections;

public class CoinItem : RunnerItem {
    public int CoinValue = 1;
	
	// pitch change per coin streak
	public float fPitchPerCoin;

	// Use this for initialization
	public override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
    public override void Update() {
        base.Update();
	}

    public override void OnPickup()
    {
		// picking up coins plays a special sound, with the pitch depending on the coin streak
		float fPitch = GetCoinStreakPitch();
		Hashtable hashOverride = new Hashtable();
		hashOverride["Pitch"] = fPitch;
		AudioManager.Instance.PlayClip( "StarSingle", hashOverride );
		
        ScoreManager.Instance.AddCoins(CoinValue);
        GameObject.Destroy(gameObject);
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
}
