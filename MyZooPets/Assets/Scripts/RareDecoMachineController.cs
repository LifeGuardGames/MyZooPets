using UnityEngine;
using System.Collections;

public class RareDecoMachineController : MonoBehaviour {

public void OpenMachine() {
		GameObject.Find("RareDecoMachineUIController").GetComponent<RareDecoMachine>().OnUse();
	}
}
