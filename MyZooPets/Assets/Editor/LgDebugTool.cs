using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Xml.Serialization;
public class LgDebugTool : EditorWindow
{
	#region constant values
    private const string CRITICAL_PATH = "/XML/Resources/Constants/_Critical.xml";
    private const string BUILDSETTING_PATH = "/XML/Resources/Constants/_BuildSetting.xml";
	#endregion

	#region private values
    private List<Constant> criticalList;
    private List<Constant> buildSettingList;
    private CriticalConstants criticalConstants;
    private BuildSettingConstants buildSettingConstants;
    private string liteBundleID; 
    private string proBundleID;
    private string liteGameKey;
    private string liteSecretKey;
    private string proGameKey;
    private string proSecretKey;
    private bool isLiteVersion = false;
    private string liteProductName;
    private string proProductName;
	private string gaBuildVersion;
	private Vector2 scrollPos;
	#endregion

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/LgDebugTool")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(LgDebugTool));
    }

    void OnFocus(){
        criticalConstants = Deserialize<CriticalConstants>(CRITICAL_PATH);
        criticalList = criticalConstants.CriticalConstantList;

        buildSettingConstants = Deserialize<BuildSettingConstants>(BUILDSETTING_PATH);
        buildSettingList = buildSettingConstants.BuildSettingConstantList;
    }

    void OnGUI()
    {
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.Label ("Plist Editor", EditorStyles.boldLabel);
            if (GUILayout.Button("Delete Plist")){
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
				case "IsLiteVersion":
					isLiteVersion = EditorGUILayout.Toggle(
						new GUIContent("Is Lite Version", "Toggle this box to set Lite or Pro version. The approprite Lite or Pro build setting for the fields above will also be set"),
						bool.Parse(constant.ConstantValue));
					constant.ConstantValue = isLiteVersion.ToString();
					
					if(isLiteVersion){
						PlayerSettings.bundleIdentifier = liteBundleID;
						PlayerSettings.productName = liteProductName;
					}else{
						PlayerSettings.bundleIdentifier = proBundleID;
						PlayerSettings.productName = proProductName;
					}
					break;
				case "AnalyticsEnabled":
					bool toggleState = EditorGUILayout.Toggle(
						new GUIContent("Is Game Analytics Enabled", "checking this box will also fill in the keys in GA_Setting"),
						bool.Parse(constant.ConstantValue));
					constant.ConstantValue = toggleState.ToString();
					
					if(toggleState){
						//set the build version
						GA.SettingsGA.Build = gaBuildVersion;
						
						//set the api keys
						if(isLiteVersion)
							GA.SettingsGA.SetKeys(liteGameKey, liteSecretKey);
						else
							GA.SettingsGA.SetKeys(proGameKey, proSecretKey);
					}else
						GA.SettingsGA.SetKeys("", "");
					break;
                    case "LiteBundleID":
                        constant.ConstantValue = EditorGUILayout.TextField("Lite Bundle ID", constant.ConstantValue);
                        liteBundleID = constant.ConstantValue;
                    break;
                    case "ProBundleID":
                        constant.ConstantValue = EditorGUILayout.TextField("Pro Bundle ID", constant.ConstantValue);
                        proBundleID = constant.ConstantValue;
                    break;
                    case "LiteProductName":
                        constant.ConstantValue = EditorGUILayout.TextField("Lite Product Name", constant.ConstantValue);
                        liteProductName = constant.ConstantValue;
                    break;
                    case "ProProductName":
                        constant.ConstantValue = EditorGUILayout.TextField("Pro Product Name", constant.ConstantValue);
                        proProductName = constant.ConstantValue;
                    break;
                    case "WellapetsLiteGameKey":
                        constant.ConstantValue = EditorGUILayout.TextField("Lite GA Game Key", constant.ConstantValue);
                        liteGameKey = constant.ConstantValue;
                    break;
                    case "WellapetsLiteSecretKey":
                        constant.ConstantValue = EditorGUILayout.TextField("Lite GA Secret Key", constant.ConstantValue);
                        liteSecretKey = constant.ConstantValue;
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


            if(GUILayout.Button("Save")){
                Serialize<BuildSettingConstants>(BUILDSETTING_PATH, buildSettingConstants);
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