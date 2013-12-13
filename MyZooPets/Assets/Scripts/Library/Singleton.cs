using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
 	protected static T instance;
 
	public EventHandler<EventArgs> OnBeingDestroyed;	// callback when this class is about to be destroyed
	protected bool bBeingDestroyed = false;

   /**
      Returns the instance of this singleton.
   */
   public static T Instance
   {
      get
      {
         if(instance == null)
         {
            instance = (T) FindObjectOfType(typeof(T));
 
         }
 
         return instance;
      }
   }

   void OnDestroy(){
		// if this singleton is being destroyed, send out a callback to anything that may care
		bBeingDestroyed = true;
		if(OnBeingDestroyed != null) 
			OnBeingDestroyed(this, EventArgs.Empty);		
		
      instance = null;
   }
}