﻿using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Xml.Serialization;
public class LgDebugTool : EditorWindow
{
    private List<CriticalConstant> constants;
    private CriticalConstants XmlData;
    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/LgDebugTool")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(LgDebugTool));
    }

    void OnFocus(){
        constants = Deserialize();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Label ("Plist Editor", EditorStyles.boldLabel);
            if (GUILayout.Button("Delete Plist")){
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }

        GUILayout.Label("Critical Constants Editor", EditorStyles.boldLabel);

        EditorGUILayout.EndVertical(); 

        if(constants != null){
            foreach(CriticalConstant constant in constants){
                switch(constant.ConstantType){
                    case "Bool":
                        bool toggleState = EditorGUILayout.Toggle(constant.Name, bool.Parse(constant.ConstantValue));
                        constant.ConstantValue = toggleState.ToString();
                    break;
                    case "String":
                        constant.ConstantValue = EditorGUILayout.TextField(constant.Name, constant.ConstantValue);
                    break;
                }
                constant.Filler = " ";
            }


            if(GUILayout.Button("Save")){
                Serialize();
            }
        }
    }

    private void Serialize(){
        XmlSerializer serializer = new XmlSerializer(typeof(CriticalConstants));
        string path = Application.dataPath + "/XML/Resources/Constants/_Critical.xml";

        using(TextWriter writer = new StreamWriter(path, false))
        {
            serializer.Serialize(writer, XmlData);
        }  
        constants = Deserialize();

        AssetDatabase.Refresh();
    }

    private List<CriticalConstant> Deserialize(){
        XmlSerializer deserializer = new XmlSerializer(typeof(CriticalConstants));
        string path = Application.dataPath + "/XML/Resources/Constants/_Critical.xml";
        TextReader reader = new StreamReader(path);
        object obj = deserializer.Deserialize(reader);
        XmlData = (CriticalConstants)obj;
        reader.Close(); 
        return XmlData.criticalConstantList;
    }
}
#endif