using UnityEngine;
using System.Collections;

public class RunnerAnimationController : TK2DAnimationController {

	private tk2dSpriteAnimationClip run;
	private tk2dSpriteAnimationClip jump;
	private tk2dSpriteAnimationClip fall;

	void Start(){
		base.Start();
		run = animator.GetClipByName("Run");
		jump = animator.GetClipByName("Jump");
		fall = animator.GetClipByName("fall");
	}

	void onPlayerJumpBegin(){
		animator.Play(jump);
	}

	void onPlayerJumpEnd(){
		animator.Play(run);
	}

	void onPlayerFallBegin(){
		animator.Play(fall);
	}

	void onPlayerFallEnd(){
		animator.Play(run);
	}
}
