using UnityEngine;
using System.Collections;

/*
    LifeGuard Games customization of NGUITools functions
*/
public static class LgNGUITools{

    /// <summary>
    /// Instantiate an object and add it to the specified parent. use the position of the prefab
    /// </summary>
    static public GameObject AddChildWithPosition(GameObject parent, GameObject prefab) {
       GameObject go = NGUITools.AddChild(parent, prefab);
       if (go != null )
       {
               Transform t = go.transform;
               t.localPosition = prefab.transform.localPosition;
				t.localScale = prefab.transform.localScale;
       }
       
       return go;
    }
	
	//---------------------------------------------------
	// GetScreenPosition()
	// Returns the screen position of the incoming NGUI
	// object.  Note that by "screen position" I mean the
	// relative position of the gameobject given its
	// anchor.  i.e. 0,0 for a top anchored object will
	// be the upper left, but the center for a center object.
	// It's up to whoever uses this function to translate
	// the coordinates as they need them using the
	// CameraManager.TransformAnchorPosition function.
	//---------------------------------------------------		
	static public Vector3 GetScreenPosition( GameObject go ) {
		Vector3 vScreenPos = GetPosition( go );
		bool bTop = false;
		Transform transParent = go.transform.parent;
		
		while ( bTop == false && transParent ) {
			GameObject goParent = transParent.gameObject;
			bTop = goParent.GetComponent<UIAnchor>() != null;
			
			if ( !bTop ) {
				vScreenPos += GetPosition( goParent );
				transParent = transParent.parent;
			}
		}
		
		return vScreenPos;
	}
	
	//---------------------------------------------------
	// GetPosition()
	// Returns the position of an NGUI object; if the 
	// incoming object has a tween, it will instead return
	// the place that object is SUPPOSED TO be.
	//---------------------------------------------------	
	static private Vector3 GetPosition( GameObject go ) {
		Vector3 vPos = go.transform.localPosition;
		PositionTweenToggle scriptTween = go.GetComponent<PositionTweenToggle>();
		if ( scriptTween )
			vPos = scriptTween.GetShowPos();
		
		return vPos;
	}
}
