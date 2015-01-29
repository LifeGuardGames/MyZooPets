using UnityEngine;
using System.Collections;

/// <summary>
/// Broadcast event.
/// Can be used for sending message in animation events
/// </summary>
public class SendMessageEvent : MonoBehaviour {

	public GameObject target1;
	public string functionName1;

	public GameObject target2;
	public string functionName2;

	public GameObject target3;
	public string functionName3;

	public GameObject intTarget1;
	public string intFunctionName1;

	public void Broadcast1(){
		if(target1 != null && functionName1 != null){
			target1.SendMessage(functionName1, gameObject, SendMessageOptions.DontRequireReceiver);
		}
		else{
			Debug.LogError("Null references to broadcast1");
		}
	}

	public void Broadcast2(){
		if(target2 != null && functionName2 != null){
			target2.SendMessage(functionName2, gameObject, SendMessageOptions.DontRequireReceiver);
		}
		else{
			Debug.LogError("Null references to broadcast2");
		}
	}

	public void Broadcast3(){
		if(target3 != null && functionName3 != null){
			target3.SendMessage(functionName3, gameObject, SendMessageOptions.DontRequireReceiver);
		}
		else{
			Debug.LogError("Null references to broadcast3");
		}
	}

	public void BroadcastInt1(int boolAux){
		if(intTarget1 != null && intFunctionName1 != null){
			intTarget1.SendMessage(intFunctionName1, boolAux);
		}
		else{
			Debug.LogError("Null references to boolBroadcast1");
		}
	}
}
