using UnityEngine;
using System.Collections;

public class RunnerAnimationController : TK2DAnimationController {
    //private bool mbStarted = false;

	void Start(){
		base.Initialize();
        //mbStarted = true;
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

    public void onPlayerGrounded() {
        animator.Play("Run");
    }

    public void onPlayerFreeFall() {
        animator.Play("Fall");
    }

    public void Reset() {
        if (animator != null)
            animator.Play("Run");
    }
}
