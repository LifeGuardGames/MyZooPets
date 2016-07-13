using UnityEngine;
using System.Collections;

public abstract class Micro : MonoBehaviour {
	protected abstract string Title {
		get;
	}

	public virtual void StartMicro(int difficulty) {
		Camera.main.transform.position=transform.position;
	}
	public virtual void EndMicro(bool won){
		if (won){
			MicroMixManager.Instance.WinMicro();
		} else {
			MicroMixManager.Instance.LoseMicro();
		}
	}
}
