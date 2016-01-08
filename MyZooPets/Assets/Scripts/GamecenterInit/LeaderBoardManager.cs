using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

public class LeaderBoardManager : Singleton<LeaderBoardManager> {
	//here we need to create the leaderboards edit this script to change howmany leaderboards are created
	ILeaderboard memory;
	ILeaderboard ninja;
	ILeaderboard doctor;
	ILeaderboard shooter;
	ILeaderboard runner;

	void Awake(){
		#if UNITY_IOS && !UNITY_EDITOR
		Social.localUser.Authenticate (success => {
			if (success) {
				Debug.Log ("Authentication successful");
				string userInfo = "Username: " + Social.localUser.userName + 
					"\nUser ID: " + Social.localUser.id + 
						"\nIsUnderage: " + Social.localUser.underage;
				LoadLeaderBoard();
			}
			else
				Debug.Log ("Authentication failed");
		});
		#endif
	}

	// Use this for initialization
	private void LoadLeaderBoard (){
		#if UNITY_IOS && !UNITY_EDITOR
		memory = Social.CreateLeaderboard();
		memory.id = "MemoryLeaderBoard";
		memory.LoadScores(result => DidLoadLeaderboard(result,memory));
		ninja = Social.CreateLeaderboard();
		ninja.id = "NinjaLeaderBoard";
		ninja.LoadScores(result => DidLoadLeaderboard(result,ninja));
		doctor = Social.CreateLeaderboard();
		doctor.id = "DoctorLeaderBoard";
		doctor.LoadScores(result => DidLoadLeaderboard(result,doctor));
		shooter = Social.CreateLeaderboard();
		shooter.id = "ShooterLeaderBoard";
		shooter.LoadScores(result => DidLoadLeaderboard(result,shooter));
		runner = Social.CreateLeaderboard();
		runner.id = "RunnerLeaderBoard";
		runner.LoadScores(result => DidLoadLeaderboard(result,runner));
		#endif
	}

	private void DidLoadLeaderboard(bool result, ILeaderboard leaderBoard) {
//		foreach (IScore score in leaderBoard.scores){
//			Debug.Log(score);
//		}
	}
	
	public void EnterScore(long score, string leaderBoardID){
		#if UNITY_IOS && !UNITY_EDITOR
		Social.ReportScore(score,leaderBoardID,success => {
			Debug.Log(success ? "Reported score successfully" : "Failed to report score");
		});
		#endif
	}

}
