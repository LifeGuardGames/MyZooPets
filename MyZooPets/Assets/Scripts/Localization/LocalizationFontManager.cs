using UnityEngine;
using System.Collections;

public class LocalizationFontManager : Singleton<LocalizationFontManager> {
	// list of font overrides.  Make sure the index of the override matches with the language in Localization.cs!  On the same game object.
	public string[] FontOverrides;
}
