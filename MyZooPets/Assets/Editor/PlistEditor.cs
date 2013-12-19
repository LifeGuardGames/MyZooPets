using UnityEngine;
using UnityEditor;
using System.Collections;

public class PlistEditor : EditorWindow
{
    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/PlistEditor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(PlistEditor));
    }

    void OnGUI()
    {
        GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
        GUILayout.BeginArea(new Rect(0, 0, 100, 40));
            if (GUILayout.Button("Delete Plist")){
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }

        GUILayout.EndArea(); 

        // groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
        //     myBool = EditorGUILayout.Toggle ("Toggle", myBool);
        //     myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
        // EditorGUILayout.EndToggleGroup ();
    }
}