using UnityEngine;
using UnityEngine.UI;

public class ButtonSetHighlight : MonoBehaviour {
	public RectTransform firstButtonTrans;	// Starting location

	void Start(){
		if(firstButtonTrans != null){
			gameObject.GetComponent<RectTransform>().sizeDelta = firstButtonTrans.sizeDelta;
		}
		else{
			GetComponent<Image>().enabled = false;
		}
		_Start();
	}

	// Override this in child, used for conditional position changes
	protected virtual void _Start(){}
	
	public void ChangePosition(Transform trans){
		gameObject.transform.position = trans.position;
	}
}
