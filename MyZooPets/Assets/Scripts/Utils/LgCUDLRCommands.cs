using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using fastJSON;

/**
 * Example console commands for getting information about GameObjects
 */
public static class LgCUDLRCommands{

	[CUDLR.Command("active object list", "lists all the game objects in the scene")]
	public static void ListActiveGameObjects(){
		UnityEngine.Object[] objects = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		foreach(UnityEngine.Object obj in objects){
			CUDLR.Console.Log(obj.name);
		}
	}
	
	[CUDLR.Command("active object list positions", "all the game objects in the scene: relative + absolute position")]
	public static void ListActiveGameObjectsPositions(){
		UnityEngine.Object[] objects = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		foreach(UnityEngine.Object obj in objects){
			UnityEngine.GameObject go = obj as UnityEngine.GameObject;
			CUDLR.Console.Log(go.name + " || relPos:" + go.transform.localPosition + "  || absPos:" + go.transform.position + "  || relScale:" + go.transform.localScale);
		}
	}

	[CUDLR.Command("object list", "lists all the game objects in the scene")]
	public static void ListGameObjects(){
		object[] gameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));

		foreach(GameObject go in gameObjects)
			CUDLR.Console.Log(go.name);

		CUDLR.Console.Log("==============Game Objects====================");
		CUDLR.Console.Log("Count: " + gameObjects.Length);
	}

	[CUDLR.Command("texture list", "lists all the textures in the scene")]
	public static void ListTextures(){
		object[] textures = Resources.FindObjectsOfTypeAll(typeof(Texture));

		foreach(Texture tex in textures)
			CUDLR.Console.Log(tex.name + "--" + tex.width + "x" + tex.height);

		CUDLR.Console.Log("=================Textures=====================");
		CUDLR.Console.Log("Count: " + textures.Length);
	}

	[CUDLR.Command("material list", "lists all the materials in the scene")]
	public static void ListMaterial(){
		object[] materials = Resources.FindObjectsOfTypeAll(typeof(Material));

		foreach(Material mat in materials)
			CUDLR.Console.Log(mat.name);

		CUDLR.Console.Log("==================Materials===================");
		CUDLR.Console.Log("Count: " + materials.Length);
	}

	[CUDLR.Command("unload unused assets", "do what it does")]
	public static void UnLoadAssets(){
		Resources.UnloadUnusedAssets();
	}

	[CUDLR.Command("gc", "garbage collect")]
	public static void GarbageCollect(){
		System.GC.Collect();
	}

	[CUDLR.Command("total memory", "total memory from gc")]
	public static void GetMemory(){
		CUDLR.Console.Log("Total Memory: " + System.GC.GetTotalMemory(false));
	}

	[CUDLR.Command("object print", "lists properties of the object")]
	public static void PrintGameObject(string[] args){
		if(args.Length < 1){
			CUDLR.Console.Log("expected : object print <Object Name>");
			return;
		}

		GameObject obj = GameObject.Find(args[0]);
		if(obj == null){
			CUDLR.Console.Log("GameObject not found : " + args[0]);
		}
		else{
			CUDLR.Console.Log("Game Object : " + obj.name);
			foreach(Component component in obj.GetComponents(typeof(Component))){
				CUDLR.Console.Log("  Component : " + component.GetType());
				foreach(FieldInfo f in component.GetType().GetFields()){
					CUDLR.Console.Log("    " + f.Name + " : " + f.GetValue(component));
				}
			}
		}
	}

	[CUDLR.Command("Screen Resolution", "display screen resolution")]
	public static void ShowScreenWidthAndHeight(){
		CUDLR.Console.Log("Screen Width: " + Screen.width + " Screen Height: " + Screen.height);
	}

	[CUDLR.Command("DateMissionsCreated", "display date mission created")]
	public static void GetDateMissionsCreated(){
		CUDLR.Console.Log("Date Mission are Created: " + DataManager.Instance.GameData.Wellapad.DateMissionsCreated);
	}

	public static void GetWellapadCurrentTasks(){
	}

	[CUDLR.Command("LgDateTimeNow", "get LgDateTime.GetTimeNow()")]
	public static void TimeNow(){
		CUDLR.Console.Log("Time Now: " + LgDateTime.GetTimeNow());
	}

	[CUDLR.Command("NextPP", "get next play period")]
	public static void NextPP(){
		CUDLR.Console.Log("Next play period: " + PlayPeriodLogic.Instance.NextPlayPeriod);
	}

	[CUDLR.Command("GameDataJson", "get data of pet in json")]
	public static void GameDataJson(){
		string jsonString = PlayerPrefs.GetString("GameData", "");
		CUDLR.Console.Log(jsonString);
	}

	[CUDLR.Command("GameData", "get real time data")]
	public static void GameData(){
		try{
			string jsonString = JSON.Instance.ToJSON(DataManager.Instance.GameData);
			CUDLR.Console.Log(jsonString);
		} catch(NullReferenceException e){
			Debug.LogException(e);
		}
	}

	[CUDLR.Command("GetPlayerPrefString", "")]
	public static void GetPlayerPrefString(string[] args){
		CUDLR.Console.Log(PlayerPrefs.GetString(args[0], ""));
	}

	[CUDLR.Command("GetPlayerPrefInt", "")]
	public static void GetPlayerPrefInt(string[] args){
		CUDLR.Console.Log(PlayerPrefs.GetInt(args[0]) + "");
	}

	[CUDLR.Command("GetFireBreaths", "")]
	public static void GetFireBreaths(string[] args){
		int numOfFireBreaths = DataManager.Instance.GameData.PetInfo.FireBreaths;
		CUDLR.Console.Log("Fire: " + numOfFireBreaths);
	}
}

/**
 * Example console route for getting information about GameObjects
 *
 */
// public static class GameObjectRoutes {
//   [CUDLR.Route("^/object/list.json$", @"(GET|HEAD)", true)]
//   public static void ListGameObjects(CUDLR.RequestContext context) {
//     string json = "[";
//     UnityEngine.Object[] objects = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
//     foreach (UnityEngine.Object obj in objects) {
//       // FIXME object names need to be escaped.. use minijson or similar
//       json += string.Format("\"{0}\", ", obj.name);
//     }
//     json = json.TrimEnd(new char[]{',', ' '}) + "]";

//     context.Response.WriteString(json, "application/json");
//   }
// }
