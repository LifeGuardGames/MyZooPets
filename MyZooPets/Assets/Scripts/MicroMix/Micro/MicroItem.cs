using UnityEngine;
using System.Collections;

public abstract class MicroItem : MonoBehaviour{
	protected Micro parent;

	public abstract void StartItem();

	public abstract void OnComplete();

	public void SetParent(Micro parent){
		this.parent = parent;
	}
}
