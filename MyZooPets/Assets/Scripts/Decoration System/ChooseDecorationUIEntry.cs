using UnityEngine;
using System.Collections;

public class ChooseDecorationUIEntry : MonoBehaviour {
	
	// the ID of the decoration this UI entry represents
	private string strID;
	
	public void SetDecoID( string id ) {
		strID = id;	
	}
	
	public string GetDecoID() {
		return strID;
	}
}
