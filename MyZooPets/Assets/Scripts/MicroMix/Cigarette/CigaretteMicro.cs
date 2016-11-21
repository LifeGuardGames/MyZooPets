using UnityEngine;
using System.Collections;

public class CigaretteMicro : Micro {
	public CigPlayerItem player;
	public Transform[] tutorialPositions;
	public Animation headPlayer;

	private CigMazeItem[] mazes;
	private int mazeIndex;
	private MicroMixFinger finger;
	private float speed = 4f;
	private int tutorialIndex;
	private bool tutorialComplete;
	private bool waiting;
	private Vector3 zOffset;
	//Used to offset the zPosition of the finger

	public override string Title {
		get {
			return "Escape";
		}
	}

	public override int Background {
		get {
			return 2;
		}
	}

	void Update() {
		if(MicroMixManager.Instance.IsPaused || !MicroMixManager.Instance.IsTutorial || tutorialComplete || waiting) { //We use this to guide the finger along the maze for the tutorial
			return;
		}
		finger.transform.position = Vector3.MoveTowards(finger.transform.position, tutorialPositions[tutorialIndex].position + zOffset, Time.deltaTime * speed);
		player.transform.position = finger.transform.position;
		if(Vector3.Distance(finger.transform.position, tutorialPositions[tutorialIndex].position + zOffset) < .1f) { //Keep going to the next position in the array until we are done
			tutorialIndex++;
			if(tutorialIndex == 2) { //Yield for the cigarette
				waiting = true;
			}
			else if(tutorialIndex == tutorialPositions.Length) {
				tutorialComplete = true;
			}
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize) {
		mazes = GetComponentsInChildren<CigMazeItem>(true);
		mazeIndex = 0;
		if(randomize) {
			mazeIndex = Random.Range(0, mazes.Length);
		}
		mazes[mazeIndex].gameObject.SetActive(true);
		player.transform.position = mazes[mazeIndex].startPosition.transform.position;
		player.finishPos = mazes[mazeIndex].finishPosition.transform.position;
		headPlayer.Play("MicroMixHeadHappy");
	}

	protected override void _SetWon(bool won) {
		if(!won) {
			headPlayer.Play("MicroMixHeadSad");
		}
	}

	protected override void _EndMicro() {
		mazes[mazeIndex].gameObject.SetActive(false);
	}

	protected override void _Pause() {

	}

	protected override void _Resume() {

	}

	protected override IEnumerator _Tutorial() {
		finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		zOffset = new Vector3(0, 0, finger.transform.position.z - tutorialPositions[0].position.z);
		finger.transform.position = tutorialPositions[0].position + zOffset;
		waiting = false;
		mazes = GetComponentsInChildren<CigMazeItem>(true);
		mazes[0].gameObject.SetActive(true);

		tutorialComplete = false;
		tutorialIndex = 0;
		while(!tutorialComplete) {
			if(waiting) {
				yield return MicroMixManager.Instance.WaitSecondsPause(.5f);
				waiting = false;
			}
			yield return 0;
		}

		finger.gameObject.SetActive(false);
	}
}
