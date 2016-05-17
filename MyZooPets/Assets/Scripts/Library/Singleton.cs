using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour{
	protected static T instance;
	protected bool bBeingDestroyed = false; //

	/**
      Returns the instance of this singleton.
   */
	public static T Instance{
		get{
			if(instance == null){
				instance = (T)FindObjectOfType(typeof(T));
			}
			return instance;
		}
	}

	void OnDestroy(){
		bBeingDestroyed = true;
	}
}