//
//  JLocalCacheWindow.cs
//  Fast Platform Switch
//
//  Copyright (c) 2013-2014 jemast software.
//

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using LocalCache = Jemast.LocalCache;
using System.IO;

public class JLocalCacheWindow : EditorWindow {
	
	static string[] buildTargetOptions = new string[] {
		"Web Player",
		"PC, Mac and Linux Standalone",
		"iOS",
		"Android",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		"Blackberry",
		"Windows Store Apps",
		"Windows Phone 8",
#endif
#if !UNITY_3_4 && !UNITY_3_5 && (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
		"Google Native Client",
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		"Flash Player",
#endif
		"PS3",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
		"PS4",
		"PS Vita",
		"Playstation®Mobile",
#endif
		"Xbox 360",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
		"Xbox One",
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		"Wii"
#endif
	};
	
	static string[][] buildTargetWithSubtargetsOptions = new string[][] {
		new string[] { "Web Player" },
		new string[] { "PC, Mac and Linux Standalone" },
		new string[] { "iOS" },
		new string[] { 
			"Android (GENERIC)",
			"Android (DXT)",
			"Android (PVRTC)",
			"Android (ATC)",
			"Android (ETC1)",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			"Android (ETC2)",
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
			"Android (ASTC)"
#endif
		},
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		new string[] {
			"Blackberry (GENERIC)",
			"Blackberry (PVRTC)",
			"Blackberry (ATC)",
			"Blackberry (ETC1)",
		},
		new string[] { "Windows Store Apps" },
		new string[] { "Windows Phone 8" },
#endif
#if !UNITY_3_4 && !UNITY_3_5 && (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
		new string[] { "Google Native Client" },
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		new string[] { "Flash Player" },
#endif
		new string[] { "PS3" },
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
		new string[] { "PS4" },
		new string[] { "PS Vita" },
		new string[] { "Playstation®Mobile" },
#endif
		new string[] { "Xbox 360" },
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
		new string[] { "Xbox One" },
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		new string[] { "Wii" }
#endif
	};
	
	static string[] androidTextureCompressionOptions = new string[] {
		"Don't override",
		"DXT (Tegra)",
		"PVRTC (PowerVR)",
		"ATC (Adreno)",
		"ETC1 or RGBA16 (GLES 2.0)",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		"ETC2 (GLES 3.0)",
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
		"ASTC"
#endif
	};
	
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
	static string[] blackberryTextureCompressionOptions = new string[] {
		"Don't override",
		"PVRTC (PowerVR)",
		"ATC (Adreno)",
		"ETC1 or RGBA16 (GLES 2.0)"
	};
#endif
	
	Texture2D[] buildTargetTextures = null;
	
	Texture2D[] androidSubtargetTextures = null;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
	Texture2D[] blackberrySubtargetTextures = null;
#endif
	
	Texture2D selectedBuildTargetTexture = null;
	Texture2D trashBuildTargetTexture = null;
	Texture2D zipBuildTargetTexture = null;
	Texture2D unzipBuildTargetTexture = null;
	
	Texture2D shadedBackgroundTexture = null;
	
	LocalCache.Shared.CacheTarget? wantedCacheTarget;
	LocalCache.Shared.CacheSubtarget? wantedCacheSubtarget;
	
	int toolbarIndex = 0;
	string[] toolbarOptions = new string[] { "PLATFORM", "STATUS", "SETTINGS" };
	Vector2 scrollPos1 = Vector2.zero;
	Vector2 scrollPos2 = Vector2.zero;
	Vector2 scrollPos3 = Vector2.zero;
	Rect scrollViewRect;
	Rect[] listViewRects = null;
	
	struct CacheData {
		public bool CacheStatus;
		public bool CompressionStatus;
		public System.DateTime Date;
		public string Size;
	}
	
	CacheData[][] cacheData = null;
	long totalCacheSize = 0;
	
	// Styles
	GUIStyle listFontStyle = new GUIStyle();
	
	GUIStyle imageStyle = new GUIStyle();
	GUIStyle areaStyle = new GUIStyle();
	GUIStyle areaStyleAlt = new GUIStyle();
	GUIStyle areaStyleActive = new GUIStyle();
	
	GUIStyle statusFontStyle = new GUIStyle();
	GUIStyle statusFontCachedStyle = new GUIStyle();
	GUIStyle statusFontCompressedStyle = new GUIStyle();
	
	GUIStyle headerGUIStyle;
	GUIStyle toolbarGUIStyle;
	GUIStyle pipelineButtonGUIStyle;

	[MenuItem("Window/Fast Platform Switch")]
	public static void ShowWindow()
	{
		JLocalCacheWindow w = EditorWindow.GetWindow(typeof(JLocalCacheWindow), false, "Platform") as JLocalCacheWindow;
		w.Show ();
	}
	
	void OnEnable() {
		// Setup current wanted target
		wantedCacheTarget = LocalCache.CacheManager.CurrentCacheTarget;
		wantedCacheSubtarget = LocalCache.CacheManager.CurrentCacheSubtarget;
	}
	
	void OnGUI()
	{
		SetupGUIStyles(GUI.skin);
		
		bool isDirty = false;

		if (buildTargetTextures == null || areaStyle.normal.background == null) {
			SetupWindow();
			isDirty = true;
    	}
		
		toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarOptions, toolbarGUIStyle);
		
		GUILayout.Space(10);
		
		if (toolbarIndex == 0) {
			scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos1, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(40f*(float)buildTargetOptions.Length));
			for (int i = 0; i < buildTargetOptions.Length; i++) {
				if (wantedCacheTarget.HasValue && i == (int)wantedCacheTarget.Value)
					EditorGUILayout.BeginHorizontal(areaStyleActive, GUILayout.Height(40), GUILayout.ExpandWidth(true));
				else
					EditorGUILayout.BeginHorizontal((i % 2 == 0) ? areaStyle : areaStyleAlt, GUILayout.Height(40), GUILayout.ExpandWidth(true));
				
				GUILayout.Space(5);
				
				// Image
				EditorGUILayout.BeginVertical(GUILayout.Height(40));
				GUILayout.FlexibleSpace();
				GUILayout.Box(buildTargetTextures[i], imageStyle);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndVertical();
				
				GUILayout.Space(5);
				
				// Text
				EditorGUILayout.BeginVertical(GUILayout.Height(40));
				GUILayout.FlexibleSpace();
				GUILayout.Label(buildTargetOptions[i], listFontStyle);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndVertical();
				
				GUILayout.FlexibleSpace();
				
				// Selected platform
				if (i == (int)LocalCache.CacheManager.CurrentCacheTarget) {
					EditorGUILayout.BeginVertical(GUILayout.Height(40));
					GUILayout.FlexibleSpace();
					GUILayout.Box(selectedBuildTargetTexture, imageStyle);
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndVertical();
					
					GUILayout.Space(5);
				}
				
				EditorGUILayout.EndHorizontal();
				
				if (Event.current.type == EventType.Repaint) {
					listViewRects[i] = GUILayoutUtility.GetLastRect();
				}
			}
			EditorGUILayout.EndScrollView();
			
			if(Event.current.type == EventType.Repaint) {
				scrollViewRect = GUILayoutUtility.GetLastRect();
				for (int i = 0; i < listViewRects.Length; i++)
					listViewRects[i].y += scrollViewRect.y;
			}
			
			if (wantedCacheTarget == LocalCache.Shared.CacheTarget.Android) {
				GUILayout.Space(10);
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Texture Compression");
				GUILayout.FlexibleSpace();
		
				int androidCompressionOption = CacheSubtargetToAndroidCompressionOption(wantedCacheSubtarget ?? LocalCache.CacheManager.CurrentCacheSubtarget);
				androidCompressionOption = EditorGUILayout.Popup(androidCompressionOption, androidTextureCompressionOptions);
				wantedCacheSubtarget = AndroidCompressionOptionToCacheSubtarget(androidCompressionOption);
				GUILayout.EndHorizontal();
			}
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			else if (wantedCacheTarget == LocalCache.Shared.CacheTarget.BlackBerry) {
				GUILayout.Space(10);
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Texture Compression");
				GUILayout.FlexibleSpace();
		
				int blackberryCompressionOption = CacheSubtargetToBlackBerryCompressionOption(wantedCacheSubtarget ?? LocalCache.CacheManager.CurrentCacheSubtarget);
				blackberryCompressionOption = EditorGUILayout.Popup(blackberryCompressionOption, blackberryTextureCompressionOptions);
				wantedCacheSubtarget = BlackBerryCompressionOptionToCacheSubtarget(blackberryCompressionOption);
				GUILayout.EndHorizontal();
			}
#endif
			else {
				wantedCacheSubtarget = null;
			}
			
			GUILayout.Space(10);
			
			GUI.enabled = !LocalCache.CacheManager.BackgroundCacheCompressionInProgress && !LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling && !EditorApplication.isUpdating;
			if (GUILayout.Button("SWITCH PLATFORM", pipelineButtonGUIStyle))
			{
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
				if (EditorSettings.externalVersionControl == ExternalVersionControl.Disabled) {
					if (EditorUtility.DisplayDialog("External Version Control Required", "You need to enable external version control for Fast Platform Switch proper operation.", "Enable Meta Files", "Cancel")) {
						EditorSettings.externalVersionControl = ExternalVersionControl.Generic;
					}
				}
#endif

				AssetDatabase.Refresh();
				
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
				if (EditorSettings.externalVersionControl != ExternalVersionControl.Disabled) {
#endif
					LocalCache.CacheManager.SwitchPlatform(wantedCacheTarget, wantedCacheSubtarget, false);
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
				}
#endif
			}
			GUI.enabled = true;
			
			GUILayout.Space(10);
			
			if (LocalCache.CacheManager.BackgroundCacheCompressionInProgress) {
				GUILayout.Label("Please wait for background compression to end...");
				GUILayout.Space(10);
			}
			
			if (LocalCache.CacheManager.PlatformRefreshInProgress) {
				GUILayout.Label("Please wait for platform refresh to end...");
				GUILayout.Space(10);
			} else if (EditorApplication.isCompiling || EditorApplication.isUpdating) {
				GUILayout.Label("Please wait for script compilation to end...");
				GUILayout.Space(10);
			} else if (EditorApplication.isPlayingOrWillChangePlaymode) {
				GUILayout.Label("Cannot switch platform during play mode...");
				GUILayout.Space(10);
			}
				
			
			// Handle list view events at the end
			Vector2 mousePosition = Event.current.mousePosition;
			if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && scrollViewRect.Contains(mousePosition))
	        {
				mousePosition.y += scrollPos1.y;
				for (int i = 0; i < listViewRects.Length; i++) {
					if (listViewRects[i].Contains(mousePosition)) {
						wantedCacheTarget = (LocalCache.Shared.CacheTarget)i;
						isDirty = true;
						break;
					}
				}
	        }
		} else if (toolbarIndex == 1) {
			int totalOptionsLength = 0;
			for (int i = 0; i < buildTargetWithSubtargetsOptions.Length; i++)
				totalOptionsLength += buildTargetWithSubtargetsOptions[i].Length;
			
			bool hasCacheData = cacheData != null;
			bool cacheDataIsInvalid = false;
			if (!hasCacheData) {
				cacheData = new CacheData[buildTargetWithSubtargetsOptions.Length][];
				totalCacheSize = 0;
			}
			
			scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(40f*(float)totalOptionsLength));
			var altStyle = false;
			for (int i = 0; i < buildTargetWithSubtargetsOptions.Length; i++) {
				if (!hasCacheData)
					cacheData[i] = new CacheData[buildTargetWithSubtargetsOptions[i].Length];
				
				for (int j = 0; j < buildTargetWithSubtargetsOptions[i].Length; j++) {
					LocalCache.Shared.CacheTarget cacheTarget = (LocalCache.Shared.CacheTarget)i;
					LocalCache.Shared.CacheSubtarget? cacheSubtarget = null;
					if (cacheTarget == LocalCache.Shared.CacheTarget.Android)
						cacheSubtarget = (LocalCache.Shared.CacheSubtarget)j + (int)LocalCache.Shared.CacheSubtarget.Android_First + 1;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
					else if (cacheTarget == LocalCache.Shared.CacheTarget.BlackBerry)
						cacheSubtarget = (LocalCache.Shared.CacheSubtarget)j + (int)LocalCache.Shared.CacheSubtarget.BlackBerry_First + 1;
#endif
					
					// Cacheception
					if (!hasCacheData) {
						cacheData[i][j].CacheStatus = LocalCache.CacheManager.GetCacheStatus (cacheTarget, cacheSubtarget);
						cacheData[i][j].CompressionStatus = LocalCache.CacheManager.GetCacheCompression(cacheTarget, cacheSubtarget);
					}
					
					EditorGUILayout.BeginHorizontal(altStyle == false ? areaStyle : areaStyleAlt, GUILayout.Height(40), GUILayout.ExpandWidth(true));
					altStyle = !altStyle;
					
					GUILayout.Space(5);
					
					// Image
					EditorGUILayout.BeginVertical(GUILayout.Height(40));
					GUILayout.FlexibleSpace();
					
					if (cacheTarget == LocalCache.Shared.CacheTarget.Android)
						GUILayout.Box(androidSubtargetTextures[j], imageStyle);
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
					else if (cacheTarget == LocalCache.Shared.CacheTarget.BlackBerry)
						GUILayout.Box(blackberrySubtargetTextures[j], imageStyle);
#endif
					else
						GUILayout.Box(buildTargetTextures[i], imageStyle);
					
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndVertical();
					
					GUILayout.Space(5);
					
					// Text
					EditorGUILayout.BeginVertical(GUILayout.Height(40));
					GUILayout.FlexibleSpace();
					GUILayout.Label(cacheData[i][j].CacheStatus ? "Cached" : "Not cached", cacheData[i][j].CacheStatus ? statusFontCachedStyle : statusFontStyle);
					GUILayout.Label(cacheData[i][j].CacheStatus ? (cacheData[i][j].CompressionStatus ? "Compressed" : "Uncompressed") : "-", cacheData[i][j].CompressionStatus ? statusFontCompressedStyle : statusFontStyle);
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndVertical();
					
					GUILayout.FlexibleSpace();
					
					if (cacheData[i][j].CacheStatus) {
						if (!hasCacheData) {
							cacheData[i][j].Date = LocalCache.CacheManager.GetCacheDate(cacheTarget, cacheSubtarget);
							long cacheSize = LocalCache.CacheManager.GetCacheSize (cacheTarget, cacheSubtarget);
							totalCacheSize += cacheSize;
							
							if (cacheSize >= 1073741824)
								cacheData[i][j].Size = (cacheSize * 9.31322575 * 1E-10).ToString("F2") + " GB";
							if (cacheSize >= 1048576)
								cacheData[i][j].Size = (cacheSize * 9.53674316 * 1E-7).ToString("F2") + " MB";
							else
								cacheData[i][j].Size = (cacheSize * 0.0009765625).ToString("F2") + " kB";
						}
						
						EditorGUILayout.BeginVertical(GUILayout.Height(40));
						GUILayout.FlexibleSpace();
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label(cacheData[i][j].Date.ToString ("d"), statusFontStyle);
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label(cacheData[i][j].Date.ToString ("t"), statusFontStyle);
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndVertical();
						
						GUILayout.Space(5);
						
						EditorGUILayout.BeginVertical(GUILayout.Height(40));
						GUILayout.FlexibleSpace();
						GUILayout.Label(cacheData[i][j].Size, statusFontStyle);
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndVertical();
						
						GUILayout.Space(5);

						if (!(LocalCache.Preferences.EnableHardLinks && cacheTarget == LocalCache.CacheManager.CurrentCacheTarget && cacheSubtarget == LocalCache.CacheManager.CurrentCacheSubtarget)) {
							EditorGUILayout.BeginVertical(GUILayout.Height(40));
							GUILayout.FlexibleSpace();
							GUI.enabled = !LocalCache.CacheManager.BackgroundCacheCompressionInProgress && !LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling && !EditorApplication.isUpdating;
							if (cacheData[i][j].CompressionStatus) {
								if (GUILayout.Button(unzipBuildTargetTexture)) {
									LocalCache.CacheManager.DecompressCache(cacheTarget, cacheSubtarget);
									cacheDataIsInvalid = true;
									isDirty = true;
								}
							} else {
								if (GUILayout.Button(zipBuildTargetTexture)) {
									LocalCache.CacheManager.CompressCache(cacheTarget, cacheSubtarget);
									cacheDataIsInvalid = true;
									isDirty = true;
								}
							}
							GUI.enabled = true;
							GUILayout.FlexibleSpace();
							EditorGUILayout.EndVertical();
						
							GUILayout.Space(5);
							
							EditorGUILayout.BeginVertical(GUILayout.Height(40));
							GUILayout.FlexibleSpace();
							GUI.enabled = !LocalCache.CacheManager.BackgroundCacheCompressionInProgress && !LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling && !EditorApplication.isUpdating;
							if (GUILayout.Button(trashBuildTargetTexture)) {
								LocalCache.CacheManager.ClearCache(cacheTarget, cacheSubtarget);
								cacheDataIsInvalid = true;
								isDirty = true;
							}
							GUI.enabled = true;
							GUILayout.FlexibleSpace();
							EditorGUILayout.EndVertical();
							
							GUILayout.Space(5);
						} else {
							GUILayout.Space(110);
						}
					}
					
					EditorGUILayout.EndHorizontal();
					
					if (Event.current.type == EventType.Repaint) {
						listViewRects[i] = GUILayoutUtility.GetLastRect();
					}
				}
			}
			EditorGUILayout.EndScrollView();
			
			GUILayout.Space(10);
			if (totalCacheSize >= 1073741824)
				GUILayout.Label("Total cache size: " + (totalCacheSize * 9.31322575 * 1E-10).ToString("F2") + " GB");
			else if (totalCacheSize >= 1048576)
				GUILayout.Label("Total cache size: " + (totalCacheSize * 9.53674316 * 1E-7).ToString("F2") + " MB");
			else
				GUILayout.Label("Total cache size: " + (totalCacheSize * 0.0009765625).ToString("F2") + " kB");
			GUILayout.Space(10);
			
			if (cacheDataIsInvalid)
				cacheData = null;
		} else if (toolbarIndex == 2) {
			scrollPos3 = EditorGUILayout.BeginScrollView(scrollPos3, GUILayout.ExpandHeight(true));
			
			GUILayout.Label("HARD LINKS (EXPERIMENTAL)", headerGUIStyle);
			
			GUILayout.Space(5);

			GUI.skin.label.wordWrap = true;
			GUILayout.Label ("Hard links boost performance by having the cache stay in place and instead change directory links during platform switch. This feature is still experimental and may cause issues on some platforms and under certain conditions.", GUILayout.Width(position.width - 25), GUILayout.Height(110f - position.width * 0.1f));
			GUI.skin.label.wordWrap = false;

			GUI.enabled = !LocalCache.CacheManager.BackgroundCacheCompressionInProgress && !LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling && !EditorApplication.isUpdating;	
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Enable Hard Links");
			GUILayout.FlexibleSpace();

			var enableHardLinks = EditorGUILayout.Toggle("",LocalCache.Preferences.EnableHardLinks, GUILayout.Width(15));
			if (enableHardLinks != LocalCache.Preferences.EnableHardLinks) {
				if (enableHardLinks == true) {
					if (EditorUtility.DisplayDialog("Enable hard links?", "WARNING: Hard links is an expirmental feature. It is recommended you BACKUP YOUR PROJECTS before using it.\n\nHard links can fail under a number of conditions including: unsupported file system, network shares, unsupported platform...\n\nTHIS OPTION IS SYSTEM-WIDE AND WILL BE ACTIVATED ON OPENING ANY OF YOUR FAST PLATFORM SWITCH V1.4+ ENABLED PROJECTS.\n\nIn case of trouble, contact us at contact@jemast.com to resolve issues.", "Ok, I understand!", "Wait... Cancel!")) {
						LocalCache.Preferences.EnableHardLinks = enableHardLinks;
						LocalCache.CacheManager.HasCheckedHardLinkStatus = false;
						isDirty = true;
					}
				} else {
					LocalCache.Preferences.EnableHardLinks = enableHardLinks;
					LocalCache.CacheManager.HasCheckedHardLinkStatus = false;
					isDirty = true;
				}
			}

			EditorGUILayout.EndHorizontal();

			GUILayout.Space(10);

			GUILayout.Label("COMPRESSION", headerGUIStyle);
			
			GUILayout.Space(10);
			
			GUI.enabled = !LocalCache.CacheManager.BackgroundCacheCompressionInProgress && !LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling && !EditorApplication.isUpdating;		
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Automatic Compression");
			GUILayout.FlexibleSpace();
			LocalCache.Preferences.AutoCompress = EditorGUILayout.Toggle("",LocalCache.Preferences.AutoCompress, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Compression Algorithm");
			GUILayout.FlexibleSpace();
			LocalCache.Preferences.CompressionAlgorithm = EditorGUILayout.Popup(LocalCache.Preferences.CompressionAlgorithm, LocalCache.Shared.CompressionAlgorithmOptions);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Compression Quality");
			GUILayout.FlexibleSpace();
			if (LocalCache.Preferences.CompressionAlgorithm == 0)
				LocalCache.Preferences.CompressionQualityLZ4 = EditorGUILayout.Popup(LocalCache.Preferences.CompressionQualityLZ4, LocalCache.Shared.CompressionQualityLZ4Options);
			EditorGUILayout.EndHorizontal();
			
			GUILayout.Space(5);
			if (GUILayout.Button("Compress all cached data immediately")) {
				LocalCache.CacheManager.CompressAllCache(false);
				cacheData = null;
				isDirty = true;
			}
			GUILayout.Space(5);
			if (GUILayout.Button("Decompress all cached data immediately")) {
				LocalCache.CacheManager.DecompressAllCache();
				cacheData = null;
				isDirty = true;
			}
			
			GUI.enabled = true;
			
			GUILayout.Space(10);
			
			GUILayout.Label("CLEAN UP", headerGUIStyle);
			
			GUILayout.Space(10);
			
			GUI.enabled = !LocalCache.CacheManager.BackgroundCacheCompressionInProgress && !LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling && !EditorApplication.isUpdating;
			if (GUILayout.Button("Delete all cached data immediately")) {
				if (EditorUtility.DisplayDialog("Delete all cached data?", "All cached data will be destroyed and switching platform will result in cache reinitialization and full asset reimport for that platform.", "Ok, I understand!", "Wait... Cancel!")) {
					LocalCache.CacheManager.ClearAllCache();
					cacheData = null;
					isDirty = true;
				}
			}
			GUI.enabled = true;
			
			if (GUILayout.Button("Unlock user interface")) {
				if (EditorUtility.DisplayDialog("Unlock user interface?", "If the plugin user interface is stuck in either 'Waiting for platform refresh to end' or 'Waiting for background compression to end' and you are sure that the switch/compression operation halted or crashed and is over, you can use this functionnality to force unlocking the user interface.", "Ok, I understand!", "Wait... Cancel!")) {
					LocalCache.CacheManager.PlatformRefreshInProgress = false;
					LocalCache.CacheManager.SwitchOperationInProgress = false;
					LocalCache.CacheManager.PlatformRefreshCurrentScene = "";
					LocalCache.CacheManager.SwitchOperationIsAPI = false;
					LocalCache.CacheManager.PlatformRefreshShouldBustCache = false;
					File.Delete(LocalCache.Shared.CachePath + "Background.txt");
				}
			}
			
			if (GUILayout.Button("Clear all cache & Reimport everything")) {
				if (EditorUtility.DisplayDialog("Perform full clean up?", "You'll need to accept to Reimport all assets when prompted. You will also be prompted to save your current scene if you haven't done it. Good luck...", "Ok, I understand!", "Wait... Cancel!")) {
					LocalCache.CacheManager.FixIssues();
					cacheData = null;
					isDirty = true;
				}
			}
			
			GUILayout.Space(10);
			
			GUILayout.Label("VERSION CONTROL", headerGUIStyle);
			
			GUILayout.Space(10);
			
			if (GUILayout.Button("Add Git ignore rule")) {
				string vcFilePath = LocalCache.Shared.ProjectPath + "/.gitignore";
				if (!File.Exists(vcFilePath)) {
					var stream = File.Create(vcFilePath);
					stream.Dispose();
				}
				string vcFileContent = File.ReadAllText (vcFilePath);
				if (!vcFileContent.Contains("/LocalCache/")) {
					vcFileContent += "\n\n#Fast Platform Switch\n/LocalCache/";
					File.WriteAllText(vcFilePath, vcFileContent);
				}
			}
			GUILayout.Space(5);
			if (GUILayout.Button("Add Mercurial ignore rule")) {
				string vcFilePath = LocalCache.Shared.ProjectPath + "/.hgignore";
				if (!File.Exists(vcFilePath)) {
					var stream = File.Create(vcFilePath);
					stream.Dispose();
				}
				string vcFileContent = File.ReadAllText (vcFilePath);
				if (!vcFileContent.Contains("^LocalCache/")) {
					vcFileContent += "\n\n#Fast Platform Switch\nsyntax: regexp\n^LocalCache/";
					File.WriteAllText(vcFilePath, vcFileContent);
				}
			}
			GUILayout.Space(5);
			GUI.skin.label.wordWrap = true;
			GUILayout.Label ("For other version control systems, make sure you ignore the \"LocalCache\" directory located in your project folder at the same level as the \"Assets\" directory.", GUILayout.Width(position.width - 25), GUILayout.Height(40));
			GUI.skin.label.wordWrap = false;
			
			GUILayout.Space(10);
			
			GUILayout.Label("DEBUG", headerGUIStyle);
			
			GUILayout.Space(10);
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Enable Log File");
			GUILayout.FlexibleSpace();
			LocalCache.Preferences.EnableLogFile = EditorGUILayout.Toggle("",LocalCache.Preferences.EnableLogFile, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.EndScrollView();
		}
		
		GUILayout.Space(10);
		
		if (isDirty) {
			cacheData = null;
			EditorUtility.SetDirty(this);
			Repaint();
		}
	}
	
	void SetupWindow() {
		if (LocalCache.CacheManager.PlatformRefreshInProgress)
			return;
		
		var assetsPath = LocalCache.Shared.EditorAssetsPath;

		// Load textures
		buildTargetTextures = new Texture2D[] {
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_WEBPLAYER.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_STANDALONE.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_IOS.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID.png", typeof(Texture2D)) as Texture2D,
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_METRO.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_WP8.png", typeof(Texture2D)) as Texture2D,
			#endif
			#if !UNITY_3_4 && !UNITY_3_5 && (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_NACL.png", typeof(Texture2D)) as Texture2D,
			#endif
			#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_FLASH.png", typeof(Texture2D)) as Texture2D,
			#endif
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_PS3.png", typeof(Texture2D)) as Texture2D,
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_PS4.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_VITA.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_PSM.png", typeof(Texture2D)) as Texture2D,
#endif
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_X360.png", typeof(Texture2D)) as Texture2D,
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_XBONE.png", typeof(Texture2D)) as Texture2D,
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_WII.png", typeof(Texture2D)) as Texture2D
#endif
		};
		
		androidSubtargetTextures = new Texture2D[] {
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_DXT.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_PVRTC.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_ATC.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_ETC1.png", typeof(Texture2D)) as Texture2D,
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_ETC2.png", typeof(Texture2D)) as Texture2D,
			#endif
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_ASTC.png", typeof(Texture2D)) as Texture2D,
			#endif
		};
		
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		blackberrySubtargetTextures = new Texture2D[] {
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10_PVRTC.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10_ATC.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10_ETC1.png", typeof(Texture2D)) as Texture2D
		};
#endif
		
		selectedBuildTargetTexture = AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_Selected.png", typeof(Texture2D)) as Texture2D;
		trashBuildTargetTexture = AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_Trash.png", typeof(Texture2D)) as Texture2D;
		zipBuildTargetTexture = AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_Zip.png", typeof(Texture2D)) as Texture2D;
		unzipBuildTargetTexture = AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_Unzip.png", typeof(Texture2D)) as Texture2D;
		
		var sharedPath = LocalCache.Shared.SharedEditorAssetsPath;
		shadedBackgroundTexture = AssetDatabase.LoadAssetAtPath(sharedPath + "Images/shadedBackground.png", typeof(Texture2D)) as Texture2D;
		
		// Setup current wanted target
		wantedCacheTarget = LocalCache.CacheManager.CurrentCacheTarget;
		wantedCacheSubtarget = LocalCache.CacheManager.CurrentCacheSubtarget;
		
		// Setup rects for list view handling
		listViewRects = new Rect[buildTargetOptions.Length];
		
		// Setup font styles
    	listFontStyle.fontSize = 12;
		statusFontStyle.fontSize = 10;
		statusFontCachedStyle.fontSize = 10;
		statusFontCompressedStyle.fontSize = 10;
		if (EditorGUIUtility.isProSkin) {
			listFontStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
			statusFontStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
			statusFontCachedStyle.normal.textColor = new Color(0.0f, 0.6f, 0.0f, 1.0f);
			statusFontCompressedStyle.normal.textColor = new Color(0.0f, 0.5f, 1.0f, 1.0f);
		} else {
			listFontStyle.normal.textColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
			statusFontStyle.normal.textColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
			statusFontCachedStyle.normal.textColor = new Color(0.0f, 0.6f, 0.0f, 1.0f);
			statusFontCompressedStyle.normal.textColor = new Color(0.0f, 0.5f, 1.0f, 1.0f);
		}
		
		// Set up GUI styles
		areaStyle.normal.background = MakeTex(1, 1, new Color(1.0f, 1.0f, 1.0f, 0.025f));
		areaStyleAlt.normal.background = MakeTex(1, 1, new Color(1.0f, 1.0f, 1.0f, 0.05f));
		areaStyleActive.normal.background = MakeTex(1, 1, new Color(0.0f, 0.5f, 1.0f, 0.5f));
		
		// Set up textures
		string[] allTextures = new string[] {
			assetsPath + "Images/Platform_WEBPLAYER.png",
			assetsPath + "Images/Platform_STANDALONE.png",
			assetsPath + "Images/Platform_IOS.png",
			assetsPath + "Images/Platform_ANDROID.png",
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			assetsPath + "Images/Platform_BB10.png",
			assetsPath + "Images/Platform_BB10_PVRTC.png",
			assetsPath + "Images/Platform_BB10_ATC.png",
			assetsPath + "Images/Platform_BB10_ETC1.png",
			assetsPath + "Images/Platform_METRO.png",
			assetsPath + "Images/Platform_WP8.png",
			#endif
			#if !UNITY_3_4 && !UNITY_3_5 && (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
			assetsPath + "Images/Platform_NACL.png",
			#endif
			#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			assetsPath + "Images/Platform_FLASH.png",
			#endif
			assetsPath + "Images/Platform_PS3.png",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
			assetsPath + "Images/Platform_PS4.png",
			assetsPath + "Images/Platform_VITA.png",
			assetsPath + "Images/Platform_PSM.png",
#endif
			assetsPath + "Images/Platform_X360.png",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
			assetsPath + "Images/Platform_XBONE.png",
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			assetsPath + "Images/Platform_WII.png",
#endif
			assetsPath + "Images/Platform_ANDROID_DXT.png",
			assetsPath + "Images/Platform_ANDROID_PVRTC.png",
			assetsPath + "Images/Platform_ANDROID_ATC.png",
			assetsPath + "Images/Platform_ANDROID_ETC1.png",
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			assetsPath + "Images/Platform_ANDROID_ETC2.png",
			#endif
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
			assetsPath + "Images/Platform_ANDROID_ASTC.png",
			#endif
			assetsPath + "Images/Platform_Selected.png",
			assetsPath + "Images/Platform_Trash.png",
			assetsPath + "Images/Platform_Zip.png",
			assetsPath + "Images/Platform_Unzip.png",
			sharedPath + "Images/shadedBackground.png"
		};
		
		foreach (var path in allTextures) {
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			if (textureImporter == null)
				continue;
			
			bool hasChange = false;
			if (textureImporter.textureType != TextureImporterType.GUI) {
				textureImporter.textureType = TextureImporterType.GUI;
				hasChange = true;
			}
			if (textureImporter.filterMode != FilterMode.Bilinear) {
				textureImporter.filterMode = FilterMode.Bilinear;
				hasChange = true;
			}
			if (textureImporter.textureFormat != TextureImporterFormat.AutomaticTruecolor) {
				textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
				hasChange = true;
			}
			
			if (hasChange)
				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		}
	}
	
	void SetupGUIStyles(GUISkin skin) {
		headerGUIStyle = new GUIStyle(skin.GetStyle("Label"));
		headerGUIStyle.fontStyle = FontStyle.Bold;
		headerGUIStyle.fontSize = 14;
		headerGUIStyle.padding = new RectOffset(10,10,5,5);
		headerGUIStyle.margin = new RectOffset(0,0,5,5);
		headerGUIStyle.normal.background = shadedBackgroundTexture;
		
		toolbarGUIStyle = new GUIStyle(skin.GetStyle("toolbarbutton"));
		toolbarGUIStyle.fontStyle = FontStyle.Bold;
		toolbarGUIStyle.fontSize = 14;
		toolbarGUIStyle.fixedHeight = 30;
		
		pipelineButtonGUIStyle = new GUIStyle(skin.GetStyle("Button"));
		pipelineButtonGUIStyle.fontStyle = FontStyle.Bold;
		pipelineButtonGUIStyle.fontSize = 14;
		pipelineButtonGUIStyle.margin = new RectOffset(10,10,10,10);
	}
	
	void CacheManagerRefresh() {
		if (this != null) {
			cacheData = null;
			EditorUtility.SetDirty(this);
			this.Repaint();
		}
	}
	
	// Handy method by andeeeee...
	private static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width*height];

        for(int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
	
	private static int CacheSubtargetToAndroidCompressionOption(LocalCache.Shared.CacheSubtarget? target) {
		switch (target) {
		case LocalCache.Shared.CacheSubtarget.Android_GENERIC:
			return 0;
		case LocalCache.Shared.CacheSubtarget.Android_ETC:
			return 4;
		case LocalCache.Shared.CacheSubtarget.Android_DXT:
			return 1;
		case LocalCache.Shared.CacheSubtarget.Android_PVRTC:
			return 2;
		case LocalCache.Shared.CacheSubtarget.Android_ATC:
			return 3;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		case LocalCache.Shared.CacheSubtarget.Android_ETC2:
			return 5;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
		case LocalCache.Shared.CacheSubtarget.Android_ASTC:
			return 6;
#endif
		default:
			return 0;
		}
	}
	
	private static LocalCache.Shared.CacheSubtarget? AndroidCompressionOptionToCacheSubtarget(int option) {
		switch (option) {
		case 0:
			return LocalCache.Shared.CacheSubtarget.Android_GENERIC;
		case 1:
			return LocalCache.Shared.CacheSubtarget.Android_DXT;
		case 2:
			return LocalCache.Shared.CacheSubtarget.Android_PVRTC;
		case 3:
			return LocalCache.Shared.CacheSubtarget.Android_ATC;
		case 4:
			return LocalCache.Shared.CacheSubtarget.Android_ETC;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		case 5:
			return LocalCache.Shared.CacheSubtarget.Android_ETC2;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
		case 6:
			return LocalCache.Shared.CacheSubtarget.Android_ASTC;
#endif
		default:
			return null;
		}
	}
	
	
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
	private static int CacheSubtargetToBlackBerryCompressionOption(LocalCache.Shared.CacheSubtarget? target) {
		switch (target) {
		case LocalCache.Shared.CacheSubtarget.BlackBerry_GENERIC:
			return 0;
		case LocalCache.Shared.CacheSubtarget.BlackBerry_PVRTC:
			return 1;
		case LocalCache.Shared.CacheSubtarget.BlackBerry_ATC:
			return 2;
		case LocalCache.Shared.CacheSubtarget.BlackBerry_ETC:
			return 3;
		default:
			return 0;
		}
	}
	
	
	private static LocalCache.Shared.CacheSubtarget? BlackBerryCompressionOptionToCacheSubtarget(int option) {
		switch (option) {
		case 0:
			return LocalCache.Shared.CacheSubtarget.BlackBerry_GENERIC;
		case 1:
			return LocalCache.Shared.CacheSubtarget.BlackBerry_PVRTC;
		case 2:
			return LocalCache.Shared.CacheSubtarget.BlackBerry_ATC;
		case 3:
			return LocalCache.Shared.CacheSubtarget.BlackBerry_ETC;
		default:
			return null;
		}
	}
#endif
	
}
