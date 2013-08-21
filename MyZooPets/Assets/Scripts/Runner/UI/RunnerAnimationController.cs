using UnityEngine;
using System.Collections;

public class RunnerAnimationController : TK2DAnimationController {

	void Start(){
		base.Initialize();
	}

	public void onPlayerJumpBegin(){
		animator.Play("Jump");
	}

	public void onPlayerJumpEnd(){
		animator.Play("Run");
	}

	public void onPlayerFallBegin(){
		animator.Play("Fall");
	}

	public void onPlayerFallEnd(){
		animator.Play("Run");
	}
}
