using UnityEngine;
using System.Collections;

/// <summary>
/// Lg NGUI tools. LifeGuard Games customization of NGUITools functions
/// </summary>
public static class LgNGUITools{

	/// <summary>
	/// <para>Gets the screen position.</para>
	/// 
	/// <para>Returns the screen position of the incoming NGUI
	/// object with respect to the UIAnchor. </para>
	/// 
	/// <para>If the incoming NGUI object is spawned inside a UIGrid
	/// make sure to toggle isObjectInUIGrid otherwise this method might not
	/// return the right screen position</para>
	/// 
	/// Note that by "screen position" I mean the
	/// relative position of the gameobject given its
	/// anchor.  i.e. 0,0 for a top-left anchored object will
	/// be the upper left, but the center for a center anchored object.
	/// It's up to whoever uses this function to translate
	/// the coordinates as they need them using the
	/// CameraManager.TransformAnchorPosition function.
	/// </summary>
	/// <returns>The screen position.</returns>
	/// <param name="go">GameObject</param>
	/// <param name="isObjectInUIGrid">bool</param>
	static public Vector3 GetScreenPosition(GameObject go, bool isObjectInUIGrid = false){
		Vector3 screenPos = GetPosition(go, isObjectInUIGrid);
		bool isTheTop = false;
		Transform parent = go.transform.parent;
		
		while(isTheTop == false && parent){
			GameObject parentGO = parent.gameObject;
			isTheTop = parentGO.GetComponent<UIAnchor>() != null;
			
			if(!isTheTop){
				screenPos += GetPosition(parentGO, isObjectInUIGrid);
				parent = parent.parent;
			}
		}
		
		return screenPos;
	}

	/// <summary>
	/// Gets the position.
	/// Returns the position of an NGUI object; if the 
	/// incoming object has a tween, it will instead return
	/// the place that object is SUPPOSED to be. 
	/// </summary>
	/// <returns>The position.</returns>
	/// <param name="go">Go.</param>
	static private Vector3 GetPosition(GameObject go, bool isObjectInUIGrid){
		Vector3 pos = go.transform.localPosition;

		//using the position return from PositionTweenToggle doesn't work well if
		//the object is inside an UIGrid. The return position is a little bit off
		//from the desired position
		PositionTweenToggle scriptTween = go.GetComponent<PositionTweenToggle>();
		if(!isObjectInUIGrid && scriptTween)
			pos = scriptTween.GetShowPos();
		
		return pos;
	}
}
