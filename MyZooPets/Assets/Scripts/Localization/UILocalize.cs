using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;

/// <summary>
/// Modified NGUI localize script to work for uGUI instead
/// </summary>
[RequireComponent(typeof(Text))]
public class UILocalize : MonoBehaviour {
	/// <summary>
	/// Localization key.
	/// </summary>
	public string key;

	string mLanguage;
	bool mStarted = false;

	/// <summary>
	/// This function is called by the Localization manager via a broadcast SendMessage.
	/// </summary>
	void OnLocalize(Localization loc) {
		if(mLanguage != loc.currentLanguage) Localize();
	}

	/// <summary>
	/// Localize the widget on enable, but only if it has been started already.
	/// </summary>
	void OnEnable() {
		if(mStarted && Localization.instance != null) Localize();
	}

	/// <summary>
	/// Localize the widget on start.
	/// </summary>
	void Start() {
		mStarted = true;
		if(Localization.instance != null) {
			Localize();
		}
	}

	/// <summary>
	/// Force-localize the widget.
	/// </summary>
	public void Localize() {
		Localization loc = Localization.instance;
		Text textComponent = GetComponent<Text>();

		// If no localization key has been specified, use the label's text as the key
		if(string.IsNullOrEmpty(mLanguage) && string.IsNullOrEmpty(key) && textComponent != null) {
			key = textComponent.text;
		}

	
		// If we still don't have a key, leave the value as blank
		string val = string.IsNullOrEmpty(key) ? "" : loc.Get(key);

		if(textComponent != null) {
			if(key == "INHALER_FLOATY_HOLD_BREATH") {
				string final = "";
				val = string.IsNullOrEmpty("INHALER_FLOATY_HOLD_BREATHP1") ? "" : loc.Get("INHALER_FLOATY_HOLD_BREATHP1");
				final = final + ArabicFixer.Fix(val);
				final = final + " 10 ";
				val = string.IsNullOrEmpty("INHALER_FLOATY_HOLD_BREATHP3") ? "" : loc.Get("INHALER_FLOATY_HOLD_BREATHP3");
				final = final + ArabicFixer.Fix(val);
				val = string.IsNullOrEmpty("INHALER_FLOATY_HOLD_BREATHP4") ? "" : loc.Get("INHALER_FLOATY_HOLD_BREATHP4");
				final = final + " " + val;
				textComponent.text = final;
			}
			else if(LoadLevelManager.Instance.GetCurrentSceneName() == SceneUtils.INHALERGAME) { 
				textComponent.text = ArabicFixer.Fix(val);
			}
			else {
				textComponent.text = val;
			}
		}
		mLanguage = loc.currentLanguage;
	}
}
