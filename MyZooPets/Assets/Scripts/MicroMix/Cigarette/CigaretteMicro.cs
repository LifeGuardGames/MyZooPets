using UnityEngine;
using System.Collections;

public class CigaretteMicro : Micro{
	public CigPlayerItem player;
	public Transform[] tutorialPositions;
	private MazeItem[] mazes;
	private int mazeIndex;
	private MicroMixFinger finger;
	private float speed = 4f;
	private int tutorialIndex;
	private bool tutorialComplete;
	private bool waiting;
	private Vector3 zOffset;
	//Used to offset the zPosition of the finger

	public override string Title{
		get{
			return "Escape";
		}
	}

	public override int Background{
		get{
			return 2;
		}
	}

	void Update(){
		if(MicroMixManager.Instance.IsPaused || !MicroMixManager.Instance.IsTutorial || tutorialComplete || waiting){
			return;
		}
		finger.transform.position = Vector3.MoveTowards(finger.transform.position, tutorialPositions[tutorialIndex].position + zOffset, Time.deltaTime * speed);
		if(Vector3.Distance(finger.transform.position, tutorialPositions[tutorialIndex].position + zOffset) < .1f){
			tutorialIndex++;
			if(tutorialIndex == 2){
				waiting = true;
			}
			else if(tutorialIndex == tutorialPositions.Length){
				tutorialComplete = true;
			}
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		mazes = GetComponentsInChildren<MazeItem>(true);
		mazeIndex = 0;
		if(randomize){
			mazeIndex = Random.Range(0, mazes.Length);
		}
		mazes[mazeIndex].gameObject.SetActive(true);
		player.transform.position = mazes[mazeIndex].startPosition.transform.position;
		Debug.Log("Starting maze: " + mazes[mazeIndex].name);
		player.finishPos = mazes[mazeIndex].finishPosition.transform.position;
	}

	protected override void _EndMicro(){
		mazes[mazeIndex].gameObject.SetActive(false);
	}

	protected override void _Pause(){
		
	}

	protected override void _Resume(){
		
	}

	protected override IEnumerator _Tutorial(){
		finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		finger.blur.SetActive(false);
		zOffset = new Vector3(0, 0, finger.transform.position.z - tutorialPositions[0].position.z);
		finger.transform.position = tutorialPositions[0].position + zOffset;
		waiting = false;
		mazes = GetComponentsInChildren<MazeItem>(true);
		mazes[0].gameObject.SetActive(true);

		tutorialComplete = false;
		tutorialIndex = 0;
		while(!tutorialComplete){
			if(waiting){
				yield return WaitSecondsPause(.5f);
				waiting = false;
			}
			yield return 0;
		}

		finger.gameObject.SetActive(false);
	}
}
