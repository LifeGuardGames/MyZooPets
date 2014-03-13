using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WellapadActionButtonUIController : MonoBehaviour {
    public GameObject backButton;
    public GameObject rewardButton;

    private string missionID = "";

	// Use this for initialization
	void Start () {
        WellapadUIManager.Instance.OnTweenDone += OnWellapadVisible;
	}

    private void OnWellapadVisible(object sender, UIManagerEventArgs args){
        List<string> currentMissionIDs = WellapadMissionController.Instance.GetCurrentMissions();

        //since we don't really use multiple missions in the wellapad right now. there will only be
        //one id in the list
        missionID = currentMissionIDs[0];

        //use GetMission(missionid) to get the mission data
        Mission mission = WellapadMissionController.Instance.GetMission(missionID);

        if(mission != null){
            RewardStatuses status = mission.RewardStatus;

            //if reward status is claimed or unearned, so regular back button
            if(status == RewardStatuses.Claimed || status == RewardStatuses.Unearned)
                ShowBackButton();
            //if reward status is unclaimed, so reward button
            else
                ShowRewardButton();
        }

    }

    //after reward button is clicked show animation. disable it and show regular back button
    private void RewardButtonClicked(){
        WellapadMissionController.Instance.ClaimReward(missionID);
        WellapadMissionController.Instance.RefreshCheck();

        ShowBackButton();
    }

    private void BackButtonClicked(){
        WellapadUIManager.Instance.CloseUI();

        HideBothButtons();
    }

    private void ShowBackButton(){
        backButton.SetActive(true);
        rewardButton.SetActive(false);
    }

    private void ShowRewardButton(){
        backButton.SetActive(false);
        rewardButton.SetActive(true);
    }

    private void HideBothButtons(){
        backButton.SetActive(false);
        rewardButton.SetActive(false);
    }
}
