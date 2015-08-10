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


	// Use this for initialization
	void Start () {
		memory = Social.CreateLeaderboard();
		memory.id = "MemoryLeaderBoard";
		memory.LoadScores(result => DidLoadLeaderboard(result,memory));
		Debug.Log("createing Memory Leaderboard");
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
	}

	private void DidLoadLeaderboard (bool result, ILeaderboard ledboard) {
		Debug.Log("Received " + ledboard.scores.Length + " scores");
		foreach (IScore score in ledboard.scores)
			Debug.Log(score);
	}
	
	public void enterScore(long score, string ledBoardId){
		Social.ReportScore(score,ledBoardId,success => {
			Debug.Log(success ? "Reported score successfully" : "Failed to report score");
		});
	}
}
