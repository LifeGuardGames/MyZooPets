using UnityEngine;
using System.Collections;
using Parse;

public class LgParseInitializeBehaviour : ParseInitializeBehaviour {
	[SerializeField]
	public bool isProduction;
	
	[SerializeField]
	public string devApplicationID;
	[SerializeField]
	public string devDotnetKey;
	
	[SerializeField]
	public string prodApplicationID;
	[SerializeField]
	public string prodDotnetKey;
	
	public override void Awake() {
		
		if(isProduction){
			base.applicationID = prodApplicationID;
			base.dotnetKey = prodDotnetKey;
		}else{
			base.applicationID = devApplicationID;
			base.dotnetKey = devDotnetKey;
		}
		base.Awake();
	}
}
