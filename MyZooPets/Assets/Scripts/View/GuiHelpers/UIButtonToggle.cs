using UnityEngine;
using System.Collections;

/// <summary>
/// User interface button toggle. -Sean
/// Primitive Toggle button with send message state functionality
/// </summary>

public class UIButtonToggle : MonoBehaviour {

	public bool isActive = true;
	public bool enforceLock = false;
	private bool isLocked = false;
	
	public GameObject target;
	public string functionName;
	public bool includeChildren = false;
	
	void Start(){
		
	}
	
	// Callback
	private void Unlock(){
		isLocked = false;
	}
	
	void OnClick(){
		if(enforceLock && enabled && !isLocked){
			isActive = !isActive;
			isLocked = true;
		}
		else{
			isActive = !isActive;
		}
	}
	
	void Send(){
		if (string.IsNullOrEmpty(functionName)) return;
		if (target == null) target = gameObject;

		if (includeChildren)
		{
			Transform[] transforms = target.GetComponentsInChildren<Transform>();

			for (int i = 0, imax = transforms.Length; i < imax; ++i)
			{
				Transform t = transforms[i];
				t.gameObject.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			target.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);

		}
	}
}
