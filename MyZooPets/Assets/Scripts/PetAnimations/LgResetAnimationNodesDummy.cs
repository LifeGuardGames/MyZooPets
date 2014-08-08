using UnityEngine;
using System.Collections;

/// <summary>
/// Dummy script used in MenuScene because ResetAnimation calls are done in bedroom and some animations are shared between MenuScene and Bedroom
/// </summary>
public class LgResetAnimationNodesDummy : MonoBehaviour {

	public void ResetAnimation(string animationState){}
	public void ResetAnimations(string animationState){}
}
