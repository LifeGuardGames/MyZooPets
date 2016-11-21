using UnityEngine;

/// <summary>
/// Parallax. By Sean Chung
/// This is used for infinite looping for parallax backgrounds
/// NOTE: Make sure to tile the shader to 2!
/// </summary>
public class Parallax : MonoBehaviour {
    public float moveTime;
    public Vector3 endLocalPos;
	public bool isStartOnAwake = true;
	
	private bool isPauseCheck = false;
	
    void Start(){
		if(isStartOnAwake){
			ResetSelf();
		}
	}
	
	private void ResetSelf(){
		// Move to spawn position and reset
		LeanTween.moveLocal(gameObject, endLocalPos, moveTime * 2f).setRepeat(-1);
	}
	
	public void Pause(){
		// NOTE: LT breaks if you call it twice directly
		if(!isPauseCheck){
			isPauseCheck = true;
			LeanTween.pause(gameObject);
		}
	}
	
	public void Play(){
		if(!isStartOnAwake){
			isStartOnAwake = true; 	// Lock the init start functionality
			ResetSelf();
		}
		else if(isPauseCheck){
			isPauseCheck = false;
			LeanTween.resume(gameObject);
		}
	}
	
//	void OnGUI(){
//		if(GUI.Button(new Rect(10, 10, 150, 20), "Play")){
//			Play();
//		}
//		if(GUI.Button(new Rect(10, 30, 150, 20), "Pause")){
//			Pause();
//		}
//	}
}
