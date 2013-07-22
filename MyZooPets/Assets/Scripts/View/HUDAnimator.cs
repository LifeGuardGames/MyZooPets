using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Room GUI animator.
/// This is the middleman proxy class that listens to the statLogic to display animation for HUD.
///
/// this animator will also have a series of events. These events will be raised
/// when special things are happening to the changing stats. Client classes will need
/// to provide listeners to handle these events.
/// For example, Event will be raised when the level up progress bar reached the leveling up point
/// If any other class wants to display UI elements when this event happens, a listener will need to
/// be provided.
/// </summary>

public class HUDAnimator : MonoBehaviour {
	//================Events================
	//call when the pet levels up. used this to level up UI components
    public delegate void OnLevelUpEventHandlers(object sender, EventArgs e);
    public static OnLevelUpEventHandlers OnLevelUp;
    //========================================

    public int DisplayPoints{
    	get{return displayPoints;}
    }

    public int DisplayStars{
    	get{return displayStars;}
    }

    public int DisplayHealth{
    	get{return displayHealth;}
    }

    public int DisplayMood{
    	get{return displayMood;}
    }

    public int NextLevelPoints{
    	get{return nextLevelPoints;}
    }

    public Level LastLevel{
    	get{return lastLevel;}
    }

    private int dataPoints, dataStars, dataHealth, dataMood, dataHunger;
	private int displayPoints, displayStars, displayHealth, displayMood, displayHunger;
	private int nextLevelPoints; //the minimum requirement for next level up
	private Level lastLevel; //pet's last level
	// private HUD hud;

	// Icon pulsing
	public GameObject healthIconAnim;
	public GameObject moodIconAnim;
	public GameObject starIconAnim;
	private AnimationControl starAnimControl;
	private AnimationControl healthAnimControl;
	private AnimationControl moodAnimControl;

	void Awake(){
		starAnimControl = starIconAnim.GetComponent<AnimationControl>();
		healthAnimControl = healthIconAnim.GetComponent<AnimationControl>();
		moodAnimControl = moodIconAnim.GetComponent<AnimationControl>();
	}

	void Start(){
		dataPoints = DataManager.Points;
		dataStars = DataManager.Stars;
		dataHealth = DataManager.Health;
		dataMood = DataManager.Mood;
		dataHunger = DataManager.Hunger;

		displayPoints = DataManager.Points;
		displayStars = DataManager.Stars;
		displayHealth = DataManager.Health;
		displayMood = DataManager.Mood;

		lastLevel = DataManager.CurrentLevel;
		nextLevelPoints = LevelUpLogic.NextLevelPoints();
	}

	void FixedUpdate(){
		if (!LoadLevelManager.IsPaused){
			PointsAnimation();
			StarsAnimation();
			HealthAnimation();
			MoodAnimation();
		}
	}

	//==================GUI Animation=========================
	private void StarsAnimation(){
		if(dataStars != DataManager.Stars){
			if(displayStars > DataManager.Stars){
				displayStars--;
				starAnimControl.Play();
			}else if(displayStars < DataManager.Stars){
				displayStars++;
				starAnimControl.Play();
			}else{
				dataStars = DataManager.Stars;

				// Stop grow & shrink, reset icon size
				starAnimControl.Stop();
			}
		}
	}

	private void PointsAnimation(){
		if(dataPoints != DataManager.Points){
			if(displayPoints < DataManager.Points){ //animate
				if(displayPoints + 3 <= DataManager.Points){
					displayPoints += 3;
				}else{
					displayPoints += DataManager.Points - displayPoints;
				}
				LevelUpEventCheck(); // Check if progress bar reach level max
			}else{ //animation done
				dataPoints = DataManager.Points;
			}
		}
	}

	private void HealthAnimation(){
		if(dataHealth != DataManager.Health){
			if(displayHealth > DataManager.Health){
				displayHealth--;
				healthAnimControl.Play();
			}else if(displayHealth < DataManager.Health){
				displayHealth++;
				healthAnimControl.Play();
			}else{
				dataHealth = DataManager.Health;

				// Stop grow & shrink. reset icon size
				healthAnimControl.Stop();
			}
		}
	}

	private void MoodAnimation(){
		if(dataMood != DataManager.Mood){
			if(displayMood > DataManager.Mood){
				displayMood--;
				moodAnimControl.Play();
			}else if(displayMood < DataManager.Mood){
				displayMood++;
				moodAnimControl.Play();
			}else{
				dataMood = DataManager.Mood;

				// Stop grow & shrink. reset icon size
				moodAnimControl.Stop();
			}
		}

	}

	//================================================================

    //Check if the points progress bar has reached the level requirement
	//if it does call on event listeners and reset the exp points progress bar
	private void LevelUpEventCheck(){
		if(displayPoints >= nextLevelPoints){ //logic for when progress bar reaches level requirement
			int remainderPoints = DataManager.Points - nextLevelPoints; //points to be added after leveling up


			if(OnLevelUp != null){
                OnLevelUp(this, EventArgs.Empty); //Level up. call the UI event listeners
            }else{
                Debug.LogError("OnLevelUp listener is null");
            }

			//reset the progress bar for next level
			DataManager.ResetPoints();
			nextLevelPoints = LevelUpLogic.NextLevelPoints(); //set the requirement for nxt level
			DataManager.AddPoints(remainderPoints);
			displayPoints = 0;
			dataPoints = 0;
			lastLevel = DataManager.CurrentLevel;
		}
	}
}
