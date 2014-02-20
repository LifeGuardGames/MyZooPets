using UnityEngine;
using System.Collections;

/// <summary>
/// Parallax. By Sean Chung
/// This is used for infinite looping for parallax backgrounds
/// NOTE:Make sure to tile the shader to 2!
/// </summary>
public class Parallax : MonoBehaviour {
    public float moveTime;
    public Vector3 endLocalPos;
	
	private bool isPauseCheck = false;
	
    void Start(){
		ResetSelf();
	}
	
	private void ResetSelf(){
		Hashtable optional = new Hashtable();
		optional.Add("repeat", -1);
		LeanTween.moveLocal(gameObject, endLocalPos, moveTime * 2f, optional);  // Move to spawn Position and reset
	}
	
	public void Pause(){
		// NOTE: LT breaks if you call it twice directly
		if(!isPauseCheck){
			isPauseCheck = true;
			LeanTween.pause(gameObject);
		}
	}
	
	public void Resume(){
		if(isPauseCheck){
			isPauseCheck = false;
			LeanTween.resume(gameObject);
		}
	}
}
