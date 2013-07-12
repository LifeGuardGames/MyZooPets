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
	
	// //Below are for Icon pulsing.
	// //Each 1 2 3, coorespond to Health, Mood, Food
 //    private float grow;
 //    private float shrink;
 //    private float starGrow;
 //    private float starShrink;
	// private bool IconSwitch1 = true;
	// private bool IconSwitch2 = true;
	// private bool IconSwitch3 = true;
	// private bool IconSwitch4 = true;
	// private int leantween1;
	// private int leantween2;
	// private int leantween3;
	// private int leantween4;
	// Hashtable optionalGrow1 = new Hashtable();
	// Hashtable optionalGrow2 = new Hashtable();
	// Hashtable optionalGrow3 = new Hashtable();
	// Hashtable optionalGrow4 = new Hashtable();
 //    Hashtable optionalShrink1 = new Hashtable();
 //    Hashtable optionalShrink2 = new Hashtable();
 //    Hashtable optionalShrink3 = new Hashtable();
 //    Hashtable optionalShrink4 = new Hashtable();
	
	
	public void Init()
	{
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

        // hud = this.GetComponent<HUD>();
        // grow = hud.healthIcon.width + 10;
        // shrink = hud.healthIcon.width;
        // starGrow = hud.starIcon.width + 10;
        // starShrink = hud.starIcon.width;
		
		// //Had to make 3 hashtable for each icon pulsing
		// optionalGrow1.Add("onCompleteTarget", gameObject);	
		// optionalGrow2.Add("onCompleteTarget", gameObject);	
		// optionalGrow3.Add("onCompleteTarget", gameObject);	
		// optionalGrow4.Add("onCompleteTarget", gameObject);	
		// optionalShrink1.Add("onCompleteTarget", gameObject);
		// optionalShrink2.Add("onCompleteTarget", gameObject);
		// optionalShrink3.Add("onCompleteTarget", gameObject);
		// optionalShrink4.Add("onCompleteTarget", gameObject);
  //   	optionalGrow1.Add("onComplete", "ShrinkHealthIcon");
  //   	optionalGrow2.Add("onComplete", "ShrinkMoodIcon");
  //   	// optionalGrow3.Add("onComplete", "ShrinkFoodIcon");
  //   	optionalGrow4.Add("onComplete", "ShrinkStarIcon");
  //       optionalShrink1.Add("onComplete", "GrowHealthIcon");
  //       optionalShrink2.Add("onComplete", "GrowMoodIcon");
  //       optionalShrink4.Add("onComplete", "GrowStarIcon");
	}
    
	void FixedUpdate(){
		if(!LoadDataLogic.IsDataLoaded) return;
		
		PointsAnimation();
		StarsAnimation();
		HealthAnimation();
		MoodAnimation();
	}

	//==================GUI Animation=========================
	private void StarsAnimation(){
		if(dataStars != DataManager.Stars){
			if(displayStars > DataManager.Stars){
				displayStars--;

				// if(IconSwitch4){ //grow & shrink stars icon
				// 	GrowStarIcon();
				// 	IconSwitch4 = false;
				// }
			}else if(displayStars < DataManager.Stars){
				displayStars++;

				// if(IconSwitch4){ //grow & shrink stars icon
				// 	GrowStarIcon();
				// 	IconSwitch4 = false;
				// }
			}else{
				dataStars = DataManager.Stars;

				// //stop grow & shrink. reset icon size

				// LeanTween.cancel(LeanTween.TweenEmpty ,leantween4);
				// LeanTween.scale(hud.starIconRect,new Vector2(starShrink, starShrink),0.1f);
				// IconSwitch4 = true;
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
				LevelUpEventCheck(); //Check if progress bar reach level max
			}else{ //animation done
				dataPoints = DataManager.Points;	
			}
		}
	}

	private void HealthAnimation(){
		if(dataHealth != DataManager.Health){
			if(displayHealth > DataManager.Health){
				displayHealth--;

				// if(IconSwitch1){ //Growth & shrink health icon
				// 	GrowHealthIcon();
				// 	IconSwitch1 = false;				
				// }
			}else if(displayHealth < DataManager.Health){
				displayHealth++;

				// if(IconSwitch1){ //Growth & shrink health icon
				// 	GrowHealthIcon();
				// 	IconSwitch1 = false;				
				// }
			}else{
				dataHealth = DataManager.Health;

				// //Stop grow & shrink. reset icon size

				// LeanTween.cancel(LeanTween.TweenEmpty,leantween1);
				// LeanTween.scale(hud.healthIconRect,new Vector2(hud.healthIcon.width,hud.healthIcon.height),0.1f);
				// IconSwitch1 = true;
			}
		}
	}

	private void MoodAnimation(){
		if(dataMood != DataManager.Mood){
			if(displayMood > DataManager.Mood){
				displayMood--;

				// if(IconSwitch2){ //Grow & shrink mood icon
				// 	GrowMoodIcon();
				// 	IconSwitch2 = false;				
				// }
			}else if(displayMood < DataManager.Mood){
				displayMood++;
				// if(IconSwitch2){ //Grow & shrink mood icon
				// 	GrowMoodIcon();
				// 	IconSwitch2 = false;				
				// }
			}else{
				dataMood = DataManager.Mood;	

				// //Stop grow & shrink. reset icon size
				// LeanTween.cancel(LeanTween.TweenEmpty,leantween2);
				// LeanTween.scale(hud.moodIconRect,new Vector2(shrink,shrink),0.1f);
				// IconSwitch2 = true;
			}
		}

	}

	//================================================================

	// //Below functions for Icon pulsing.
 //    private void GrowHealthIcon(){
 //    	leantween1 = LeanTween.scale(hud.healthIconRect,new Vector2(grow,grow),0.2f, optionalGrow1);
 //    }
 //    private void GrowMoodIcon(){
 //    	leantween2 = LeanTween.scale(hud.moodIconRect,new Vector2(grow,grow),0.2f, optionalGrow2);
 //    }
 //    private void GrowStarIcon(){
 //    	leantween4 = LeanTween.scale(hud.starIconRect,new Vector2(starGrow,starGrow),0.2f, optionalGrow4);
 //    }

 //    private void ShrinkHealthIcon(){
 //    	leantween1 = LeanTween.scale(hud.healthIconRect,new Vector2(shrink,shrink),0.2f, optionalShrink1);
 //    }
 //    private void ShrinkMoodIcon(){
 //    	leantween2 = LeanTween.scale(hud.moodIconRect,new Vector2(shrink,shrink),0.2f, optionalShrink2);
 //    }
 //    private void ShrinkStarIcon(){
 //    	leantween4 = LeanTween.scale(hud.starIconRect,new Vector2(starShrink,starShrink),0.2f, optionalShrink4);
 //    }

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
