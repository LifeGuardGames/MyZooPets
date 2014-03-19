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
    private List<Constant> criticalList;
    private List<Constant> buildSettingList;
    private CriticalConstants criticalConstants;
    private BuildSettingConstants buildSettingConstants;

    private const string CRITICAL_PATH = "/XML/Resources/Constants/_Critical.xml";
    private const string BUILDSETTING_PATH = "/XML/Resources/Constants/_BuildSetting.xml";

        string liteBundleID = "";
        string proBundleID = "";
        string liteGameKey= "";
        string liteSecretKey = "";
        string proGameKey = "";
        string proSecretKey = "";
        bool isLiteVersion = false;
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
        // EditorGUILayout.BeginVertical();
        GUILayout.Label ("Plist Editor", EditorStyles.boldLabel);
            if (GUILayout.Button("Delete Plist")){
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }

        GUILayout.Label("Critical Constants Editor", EditorStyles.boldLabel);

        // EditorGUILayout.EndVertical(); 

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
                    case "LiteBundleID":
                        constant.ConstantValue = EditorGUILayout.TextField("Lite Bundle ID", constant.ConstantValue);
                        liteBundleID = constant.ConstantValue;
                    break;
                    case "ProBundleID":
                        constant.ConstantValue = EditorGUILayout.TextField("Pro Bundle ID", constant.ConstantValue);
                        proBundleID = constant.ConstantValue;
                    break;
                    case "IsLiteVersion":
                        isLiteVersion = EditorGUILayout.Toggle(
                            new GUIContent("Is Lite Version", "Toggle this box to set Lite or Pro version. The approprite bundle ID will also be set"),
                            bool.Parse(constant.ConstantValue));
                        constant.ConstantValue = isLiteVersion.ToString();
                        if(isLiteVersion)
                            PlayerSettings.bundleIdentifier = liteBundleID;
                        else
                            PlayerSettings.bundleIdentifier = proBundleID;
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
                    case "AnalyticsEnabled":
                        bool toggleState = EditorGUILayout.Toggle(
                            new GUIContent("Is Game Analytics Enabled", "checking this box will also fill in the keys in GA_Setting"),
                            bool.Parse(constant.ConstantValue));
                        constant.ConstantValue = toggleState.ToString();

                        if(toggleState){
                            if(isLiteVersion)
                                GA.SettingsGA.SetKeys(liteGameKey, liteSecretKey);
                            else
                                GA.SettingsGA.SetKeys(proGameKey, proSecretKey);
                        }else
                            GA.SettingsGA.SetKeys("", "");
                    break;
                }
            }

            if(GUILayout.Button("Save")){
                Serialize<BuildSettingConstants>(BUILDSETTING_PATH, buildSettingConstants);
            }
        }
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
        // XmlData = (T) obj;
        reader.Close(); 
        return (T) obj;
    }

  // private void Serialize(){
    //     XmlSerializer serializer = new XmlSerializer(typeof(CriticalConstants));
    //     string path = Application.dataPath + "/XML/Resources/Constants/_Critical.xml";

    //     using(TextWriter writer = new StreamWriter(path, false))
    //     {
    //         serializer.Serialize(writer, XmlData);
    //     }  
    //     criticalList = Deserialize();

    //     AssetDatabase.Refresh();
    // }

    // private List<Constant> Deserialize(){
    //     XmlSerializer deserializer = new XmlSerializer(typeof(CriticalConstants));
    //     string path = Application.dataPath + "/XML/Resources/Constants/_Critical.xml";
    //     TextReader reader = new StreamReader(path);
    //     object obj = deserializer.Deserialize(reader);
    //     XmlData = (CriticalConstants)obj;
    //     reader.Close(); 
    //     return XmlData.CriticalConstantList;
    // }
}
#endif