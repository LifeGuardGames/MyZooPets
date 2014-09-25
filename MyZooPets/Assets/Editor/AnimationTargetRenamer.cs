using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Animation target renamer.
/// This script allows animation curves to be moved from one target to another.
/// 
/// Usage:
///     1) Select the gameobject whose curves you wish to move.
///     2) Ensure that the animation to be modified is the default animation on the game object.
///     3) Open the Animation Target Renamer from the Window menu in the Unity UI.
///     4) Change the names in the textboxes on the right side of the window to the names of the objects you wish to move the animations to.
///         NOTE: if the objects do not exist, the original objects will be duplicated and renamed.     
///     5) Select whether or not the old object should be deleted.
///     6) Press Apply.
/// </summary>
public class AnimationTargetRenamer : EditorWindow
{
	/// <summary>
	/// The animation whose curves to change.
	/// </summary>
	private static Animation ani;
	
	/// <summary>
	/// The curve data for the animation.
	/// </summary>
	private static AnimationClipCurveData[] curveDatas;
	
	/// <summary>
	/// The new animation clip.
	/// </summary>
	private static AnimationClip newClip;
	
	/// <summary>
	/// Whether or not the curve data is currently initialized.
	/// </summary>
	private static bool initialized = false;
	
	/// <summary>
	/// Whether or not to delete the old names of objects.
	/// </summary>
	private static bool deleteOldObjects = false;
	
	/// <summary>
	/// The names of the original GameObjects.
	/// </summary>
	private static List<string> origObjectPaths;
	
	
	/// <summary>
	/// The names of the target GameObjects.
	/// </summary>
	private static List<string> targetObjectPaths;
	
	[MenuItem("Window/Animation Target Renamer")]
	public static void OpenWindow ()
	{
		AnimationTargetRenamer window = (AnimationTargetRenamer)GetWindow<AnimationTargetRenamer> ("Animation Target Renamer");
		initialized = false;
	}
	
	void OnGUI ()
	{
		// find the game object we're working on.
		GameObject go = Selection.activeGameObject;
		
		if (!initialized) {
			
			// If the object isn't set or doesn't have an animation,
			/// we can't initialize the cuve data, so do nothing.
			if (go != null && go.animation != null) {
				
				// if we haven't looked at the curve data, do so now, 
				// and initialize the list of original object paths.
				ani = go.animation;
				
				curveDatas = AnimationUtility.GetAllCurves (ani.clip, true);
				
				origObjectPaths = new List<string> ();
				foreach (AnimationClipCurveData curveData in curveDatas) {
					if (!origObjectPaths.Contains (curveData.path)) {
						origObjectPaths.Add (curveData.path);
					}
				}
				initialized = true;
			}
			
		} 
		
		if (go != null && go.animation != null) {
			// if we got here, we have all the data we need to work with,
			// so we should be able to build the UI.
			
			targetObjectPaths = new List<string> ();
			
			foreach (AnimationClipCurveData curveData in curveDatas) {
				if (!targetObjectPaths.Contains (curveData.path)) {
					// if we haven't already added a target, add it to the list.
					targetObjectPaths.Add (curveData.path);
				}
			}
			
			
			// build the list of textboxes for renaming.
			Dictionary<string, string> newNames = new Dictionary<string, string> ();
			
			for (int t=0; t<targetObjectPaths.Count; t++) {
				string newName = EditorGUILayout.TextField (origObjectPaths [t], targetObjectPaths [t]);
				
				if (newName != targetObjectPaths [t]) {
					newNames.Add (targetObjectPaths [t], newName);
				}
				
			}
			
			// set the curve data to the new values.    
			foreach (KeyValuePair<string,string> pair in newNames) {
				string oldName = pair.Key;
				string newName = pair.Value;
				
				foreach (var curveData in curveDatas) {
					if (curveData.path == oldName) {
						curveData.path = newName;
					}
				}
			}
			
			// display the check box with the option to delete old objects.
			deleteOldObjects = EditorGUILayout.Toggle("Delete old objects", deleteOldObjects);
			
		} else {
			GUILayout.Label ("Please select an object with an animation.");
			initialized = false;
		}       
		
		
		if (GUILayout.Button ("Apply")) {
			
			// get the actual gameobjects we're working on.
			List<GameObject> originalObjects = new List<GameObject>();
			List<GameObject> targetObjects = new List<GameObject>();
			
			for(int i =0; i < origObjectPaths.Count; i++) {
				originalObjects.Add(GameObject.Find(origObjectPaths[i]));
				
				GameObject target = GameObject.Find(targetObjectPaths[i]);
				if(target == null) 
				{
					// If the target object doesn't exist, duplicate the source object,
					// and rename it.
					target = GameObject.Instantiate(originalObjects[i]) as GameObject;  
					target.name = targetObjectPaths[i];
					target.transform.parent = originalObjects[i].transform.parent;
				}
				targetObjects.Add(target);
				
			}
			
			// set up the curves based on the new names.
			ani.clip.ClearCurves ();
			foreach (var curveData in curveDatas) {
				ani.clip.SetCurve (curveData.path, curveData.type, curveData.propertyName, curveData.curve);
			}
			origObjectPaths.Clear ();
			foreach (AnimationClipCurveData curveData in curveDatas) {
				if (!origObjectPaths.Contains (curveData.path)) {
					origObjectPaths.Add (curveData.path);
				}
				
			}
			
			// if necessary, delete the old objects.
			if(deleteOldObjects) {
				for(int i = 0; i < originalObjects.Count; i++) {
					if(originalObjects[i] != targetObjects[i]) {
						DestroyImmediate(originalObjects[i]);   
					}
				}
			}
		}
	}
	
}