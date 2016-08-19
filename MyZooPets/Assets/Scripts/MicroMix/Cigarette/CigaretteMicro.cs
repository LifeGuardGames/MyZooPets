using UnityEngine;
using System.Collections;

public class CigaretteMicro : Micro{
	public CigPlayerItem player;
	private MazeItem[] mazes;
	private int mazeIndex;

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
		yield return 0;
	}
}
