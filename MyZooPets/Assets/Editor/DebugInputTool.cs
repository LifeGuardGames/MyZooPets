using UnityEngine;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Xml.Serialization;

// Test time format: 2014-09-20 3:00:00
public class DebugInputTool : EditorWindow {
	#region constant values
	private const string CRITICAL_PATH = "/XML/Resources/Constants/_Critical.xml";
	private const string BUILDSETTING_PATH = "/XML/Resources/Constants/_BuildSetting.xml";
	#endregion

	#region private values
	private List<Constant> criticalList;
	private List<Constant> buildSettingList;
	private List<Constant> buildInfoList;

	private CriticalConstants criticalConstants;
	private BuildSettingConstants buildSettingConstants;
	private BuildInfoConstants buildInfoConstants;

	private Vector2 scrollPos;
	#endregion

	// Add menu item named "My Window" to the Window menu
	[MenuItem("LGG/DebugInputTool")]
	public static void ShowWindow() {
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(DebugInputTool));
	}

	void OnFocus() {
		criticalConstants = Deserialize<CriticalConstants>(CRITICAL_PATH);
		criticalList = criticalConstants.CriticalConstantList;

		buildSettingConstants = Deserialize<BuildSettingConstants>(BUILDSETTING_PATH);
		buildSettingList = buildSettingConstants.BuildSettingConstantList;
	}

	void OnGUI() {
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		GUILayout.Label("Plist Editor", EditorStyles.boldLabel);
		if(GUILayout.Button("Delete Plist")) {
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
		}

		GUILayout.Label("Critical Constants Editor", EditorStyles.boldLabel);

		if(criticalList != null) {
			foreach(Constant constant in criticalList) {
				switch(constant.ConstantType) {
					case "Bool":
						bool toggleState = EditorGUILayout.Toggle(constant.Name, bool.Parse(constant.ConstantValue));
						constant.ConstantValue = toggleState.ToString();
						break;
					case "String":
						constant.ConstantValue = EditorGUILayout.TextField(constant.Name, constant.ConstantValue);
						break;
				}
			}
			if(GUILayout.Button("Save")) {
				Serialize<CriticalConstants>(CRITICAL_PATH, criticalConstants);
			}
		}

		GUILayout.Label("Build Setting Editor", EditorStyles.boldLabel);
		if(buildSettingList != null) {
			foreach(Constant constant in buildSettingList) {
				switch(constant.Name) {
					case "AnalyticsEnabled":
						bool toggleState = EditorGUILayout.Toggle(new GUIContent("Is Analytics Enabled"), bool.Parse(constant.ConstantValue));
						constant.ConstantValue = toggleState.ToString();
						break;
					case "IsLiveAnalytics":
						bool toggleState3 = EditorGUILayout.Toggle(new GUIContent("Is Live Analytics"), bool.Parse(constant.ConstantValue));
						constant.ConstantValue = toggleState3.ToString();
						break;
					case "AdsEnabled":
						bool toggleState2 = EditorGUILayout.Toggle(new GUIContent("Is Ads Enabled"), bool.Parse(constant.ConstantValue));
						constant.ConstantValue = toggleState2.ToString();
						break;
				}
			}
			if(GUILayout.Button("Save")) {
				Serialize<BuildSettingConstants>(BUILDSETTING_PATH, buildSettingConstants);
			}
		}
		EditorGUILayout.EndScrollView();
	}

	private void Serialize<T>(string filePath, object xmlData) {
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		string path = Application.dataPath + filePath;

		using(TextWriter writer = new StreamWriter(path, false)) {
			serializer.Serialize(writer, (T)xmlData);
		}
		AssetDatabase.Refresh();
	}

	private T Deserialize<T>(string filePath) {
		XmlSerializer deserializer = new XmlSerializer(typeof(T));
		string path = Application.dataPath + filePath;
		TextReader reader = new StreamReader(path);
		object obj = deserializer.Deserialize(reader);
		reader.Close();
		return (T)obj;
	}
}
#endif