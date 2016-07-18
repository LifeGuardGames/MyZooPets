using UnityEngine;
using System.Collections;

public class MicroItem : MonoBehaviour{
	public Micro parent;
	public virtual void StartItem(){ //Does nothing, but nothing may be necessary, so we are virtual, not abstract
	}
	public virtual void OnComplete(){
	}
}
