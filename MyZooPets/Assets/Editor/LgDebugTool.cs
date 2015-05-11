﻿using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Xml.Serialization;

// Test time format: 2014-09-20 3:00:00
public class LgDebugTool : EditorWindow{
	#region constant values
    private const string CRITICAL_PATH = "/XML/Resources/Constants/_Critical.xml";
    private const string BUILDSETTING_PATH = "/XML/Resources/Constants/_BuildSetting.xml";
	private const string BUILDINFO_PATH = "/Editor/XMLSerialization/BuildInfo.xml";
	#endregion

	#region private values
    private List<Constant> criticalList;
    private List<Constant> buildSettingList;
	private List<Constant> buildInfoList;
	
    private CriticalConstants criticalConstants;
    private BuildSettingConstants buildSettingConstants;
	private BuildInfoConstants buildInfoConstants;

    private string proBundleID;
    private string proGameKey;
    private string proSecretKey;
    private string proProductName;
	private string gaBuildVersion;
	private Vector2 scrollPos;
	#endregion

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/LgDebugTool")]
    public static void ShowWindow(){
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(LgDebugTool));
    }

    void OnFocus(){
        criticalConstants = Deserialize<CriticalConstants>(CRITICAL_PATH);
        criticalList = criticalConstants.CriticalConstantList;

        buildSettingConstants = Deserialize<BuildSettingConstants>(BUILDSETTING_PATH);
        buildSettingList = buildSettingConstants.BuildSettingConstantList;

		buildInfoConstants = Deserialize<BuildInfoConstants>(BUILDINFO_PATH);
		buildInfoList = buildInfoConstants.BuildInfoConstantList;
    }

    void OnGUI(){
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.Label("Plist Editor", EditorStyles.boldLabel);
        if(GUILayout.Button("Delete Plist")){
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        GUILayout.Label("Critical Constants Editor", EditorStyles.boldLabel);

        if(criticalList != null){
            foreach(Constant constant in criticalList){
                switch(constant.ConstantType){
                    case "Bool":
                        bool toggleState = EditorGUILayout.Toggle(constant.Name, bool.Parse(constant.ConstantValue));
                        constant.ConstantValue = toggleState.ToString();
                    break;
                    case "String":
                        constant.ConstantValue = EditorGUILayout.TextField(constant.Name, constant.ConstantValue);
                    break;
                }
            }

            if(GUILayout.Button("Save")){
                Serialize<CriticalConstants>(CRITICAL_PATH, criticalConstants);
            }
        }

        GUILayout.Label("Build Setting Editor", EditorStyles.boldLabel);
        if(buildSettingList != null){
            foreach(Constant constant in buildSettingList){
                switch(constant.Name){
				case "BuildVersion":
					constant.ConstantValue = EditorGUILayout.TextField("Build Version", constant.ConstantValue);
					gaBuildVersion = constant.ConstantValue;
					PlayerSettings.bundleVersion = constant.ConstantValue;
					break;
				case "BuildVersionCode":
					constant.ConstantValue = EditorGUILayout.TextField("Android Build Version Code", constant.ConstantValue);
					PlayerSettings.Android.bundleVersionCode = int.Parse(constant.ConstantValue);
					break;
				case "AnalyticsEnabled":
					bool toggleState = EditorGUILayout.Toggle(
						new GUIContent("Is Game Analytics Enabled", "checking this box will also fill in the keys in GA_Setting"),
						bool.Parse(constant.ConstantValue));
					constant.ConstantValue = toggleState.ToString();
					
					if(toggleState){
						GA.SettingsGA.Build = gaBuildVersion;				//set the build version
						GA.SettingsGA.SetKeys(proGameKey, proSecretKey);	//set the api keys
					}
					else{
						GA.SettingsGA.SetKeys("", "");
					}
					break;
                }
            }

            if(GUILayout.Button("Save")){
                Serialize<BuildSettingConstants>(BUILDSETTING_PATH, buildSettingConstants);
            }

			GUILayout.Label("Build Info Editor (Not package with the game binary", EditorStyles.boldLabel);
			if(buildInfoList != null){
				foreach(Constant constant in buildInfoList){
					switch(constant.Name){
					case "ProBundleID":
						constant.ConstantValue = EditorGUILayout.TextField("Pro Bundle ID", constant.ConstantValue);
						proBundleID = constant.ConstantValue;
						break;
					case "ProProductName":
						constant.ConstantValue = EditorGUILayout.TextField("Pro Product Name", constant.ConstantValue);
						proProductName = constant.ConstantValue;
						break;
					case "WellapetsProGameKey":
						constant.ConstantValue = EditorGUILayout.TextField("Pro GA Game Key", constant.ConstantValue);
						proGameKey = constant.ConstantValue;
						break;
					case "WellapetsProSecretKey":
						constant.ConstantValue = EditorGUILayout.TextField("Pro GA Secret Key", constant.ConstantValue);
						proSecretKey = constant.ConstantValue;
						break;
					}
				}
			}
				
			if(GUILayout.Button("Save")){
				Serialize<BuildInfoConstants>(BUILDINFO_PATH, buildInfoConstants);
			}
				
			GUILayout.Label("Build Setting Tools", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Load Lite App Icon")){
				LoadAppIcon("WellaPetsIconLite");
			}
			if(GUILayout.Button("Load Pro App Icon")){
                LoadAppIcon("WellaPetsIcon");
            }
            EditorGUILayout.EndHorizontal(); 
        }
		EditorGUILayout.EndScrollView();
    }

    private void LoadAppIcon(string iconPrefix){
        string filePath = "Assets/Textures/MobileIcons/";
        int[] textureSizes = PlayerSettings.GetIconSizesForTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        Texture2D[] icons = new Texture2D[textureSizes.Length];

        for(int i=0; i<textureSizes.Length; i++){
            string assetFilePath = filePath + iconPrefix + textureSizes[i] + ".png";
            Debug.Log(assetFilePath);
            Texture2D appIcon = AssetDatabase.LoadAssetAtPath(assetFilePath, typeof(Texture2D)) as Texture2D;
            Debug.Log(appIcon);
            icons[i] = appIcon;
        }

        PlayerSettings.SetIconsForTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup, icons);
    }

    private void Serialize<T>(string filePath, object xmlData){
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        string path = Application.dataPath + filePath;

        using(TextWriter writer = new StreamWriter(path, false)){
            serializer.Serialize(writer, (T) xmlData);
        }  

        AssetDatabase.Refresh();
    }

    private T Deserialize<T>(string filePath){
        XmlSerializer deserializer = new XmlSerializer(typeof(T));
        string path = Application.dataPath + filePath; 
        TextReader reader = new StreamReader(path);
        object obj = deserializer.Deserialize(reader);
        reader.Close(); 
        return (T) obj;
    }
}
#endif