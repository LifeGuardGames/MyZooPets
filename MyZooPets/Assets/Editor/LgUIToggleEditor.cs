using UnityEditor;

[CustomEditor(typeof(LgUIToggle))]
public class LgUIToggleEditor : UnityEditor.UI.ToggleEditor {

	public override void OnInspectorGUI() {
		LgUIToggle component = (LgUIToggle)target;
		base.OnInspectorGUI();

		component.modeTypeSize = EditorGUILayout.IntSlider("UI Modes", component.modeTypeSize, 1, 3);
		if(component.modeTypeSize == 1) {
			component.modeType1 = (UIModeTypes)EditorGUILayout.EnumPopup("Mode Type 1", component.modeType1);
		}
		else if(component.modeTypeSize == 2) {
			component.modeType1 = (UIModeTypes)EditorGUILayout.EnumPopup("Mode Type 1", component.modeType1);
			component.modeType2 = (UIModeTypes)EditorGUILayout.EnumPopup("Mode Type 2", component.modeType2);
		}
		else if(component.modeTypeSize == 3) {
			component.modeType1 = (UIModeTypes)EditorGUILayout.EnumPopup("Mode Type 1", component.modeType1);
			component.modeType2 = (UIModeTypes)EditorGUILayout.EnumPopup("Mode Type 2", component.modeType2);
			component.modeType3 = (UIModeTypes)EditorGUILayout.EnumPopup("Mode Type 3", component.modeType3);
		}
	}
}
