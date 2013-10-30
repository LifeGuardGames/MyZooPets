using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Net;

/**
 * Example console commands for getting information about GameObjects
 */
public static class GameObjectCommands {

  [CUDLR.Command("active object list", "lists all the game objects in the scene")]
  public static void ListActiveGameObjects() {
    UnityEngine.Object[] objects = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
    foreach (UnityEngine.Object obj in objects) {
      CUDLR.Console.Log(obj.name);
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
  public static void PrintGameObject(string[] args) {
    if (args.Length < 1) {
      CUDLR.Console.Log( "expected : object print <Object Name>" );
      return;
    }

    GameObject obj = GameObject.Find( args[0] );
    if (obj == null) {
      CUDLR.Console.Log("GameObject not found : "+args[0]);
    } else {
      CUDLR.Console.Log("Game Object : "+obj.name);
      foreach (Component component in obj.GetComponents(typeof(Component))) {
       CUDLR.Console.Log("  Component : "+component.GetType());
        foreach (FieldInfo f in component.GetType().GetFields()) {
          CUDLR.Console.Log("    "+f.Name+" : "+f.GetValue(component));
        }
      }
    }
  }

  [CUDLR.Command("load level", "load a new level")]
  public static void LoadNewLevel(string[] args){
    if(args.Length < 1){
      CUDLR.Console.Log("expect a level name");
      return;
    }
    Application.LoadLevel(args[0]);
  }
}



/**
 * Example console route for getting information about GameObjects
 *
 */
public static class GameObjectRoutes {

  [CUDLR.Route("^/object/list.json$", @"(GET|HEAD)", true)]
  public static void ListGameObjects(CUDLR.RequestContext context) {
    string json = "[";
    UnityEngine.Object[] objects = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
    foreach (UnityEngine.Object obj in objects) {
      // FIXME object names need to be escaped.. use minijson or similar
      json += string.Format("\"{0}\", ", obj.name);
    }
    json = json.TrimEnd(new char[]{',', ' '}) + "]";

    context.Response.WriteString(json, "application/json");
  }
}
