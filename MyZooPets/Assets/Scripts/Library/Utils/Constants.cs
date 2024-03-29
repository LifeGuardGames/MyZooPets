﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Constants. Loads constant from xml file;[
/// </summary>

public class Constants{
	private static Hashtable hashConstants;
	
	public static T GetConstant<T>(string strKey){
		if(hashConstants == null)
			SetUpData();
		
		T data = default(T);
		if(hashConstants.ContainsKey(strKey))
			data = (T)hashConstants[strKey];
		else
			Debug.LogError("No such constant for key " + strKey);
		
		return data;
	}
	
	public static Vector3 ParseVector3(string vectorString){
		Vector3 vector = new Vector3(0, 0, 0);
		String[] arrayVector3;

		try{
			arrayVector3 = vectorString.Split(","[0]);

			if(arrayVector3.Length == 3){
				vector = new Vector3(
					float.Parse(arrayVector3[0]), 
					float.Parse(arrayVector3[1]), 
					float.Parse(arrayVector3[2]));
			}
			else
				Debug.LogError("Illegal vector3 parsing, reverting to 0,0,0");
		}
		catch(NullReferenceException e){
			Debug.LogError("Vector3 parsing. string cannot be null. error message: " + e.Message);
		}

		return vector;
	}
	
	private static Color ParseColor(string colorString){
		Color color = Color.white;
		String[] arrayColor = colorString.Split(","[0]);
		if(arrayColor.Length == 4){
			color = new Color(
				float.Parse(arrayColor[0]) / 255, 
				float.Parse(arrayColor[1]) / 255, 
				float.Parse(arrayColor[2]) / 255, 
				float.Parse(arrayColor[3]) / 255);
		}
		else
			Debug.LogError("Illegal color parsing...reverting to white");
			
		return color;
	}
	
	private static void SetUpData(){
		hashConstants = new Hashtable();
		
		//Load all item xml files
		UnityEngine.Object[] files = Resources.LoadAll("Constants", typeof(TextAsset));
		foreach(TextAsset file in files){
			string xmlString = file.text;

			//Create XMLParser instance
			XMLParser xmlParser = new XMLParser(xmlString);

			//Call the parser to build the IXMLNode objects
			XMLElement xmlElement = xmlParser.Parse();
			
			string strError = "Error in file " + file.name + ": ";

			//Go through all child node of xmlElement (the parent of the file)
			for(int i=0; i<xmlElement.Children.Count; i++){
				IXMLNode childNode = xmlElement.Children[i];
				Hashtable hashAttr = XMLUtils.GetAttributes(childNode);
				
				AddData(strError, hashAttr);	
			}
		}				
	}
	
	private static void AddData(string i_strError, Hashtable hashAttributes){
		if(!hashAttributes.Contains("Name") || !hashAttributes.Contains("Type") || !hashAttributes.Contains("Value"))
			Debug.LogError("Illegal constant in Constants.xml");
		else{
			string strKey = (string)hashAttributes["Name"];
			string strType = (string)hashAttributes["Type"];
			string strValue = (string)hashAttributes["Value"];
			
			if(hashConstants.Contains(strKey))
				Debug.LogError(i_strError + "Repeat constant: " + strKey);
			
			// get the type and switch on it so we know what kind of value the constant is
			switch(strType){
			case "String":
				hashConstants[strKey] = strValue;
				break;
			case "Int":
				hashConstants[strKey] = int.Parse(strValue);
				break;
			case "Bool":
				hashConstants[strKey] = bool.Parse(strValue);
				break;
			case "Float":
				hashConstants[strKey] = float.Parse(strValue);
				break;
			case "Color":
				hashConstants[strKey] = ParseColor(strValue);
				break;
			case "Vector3":
				hashConstants[strKey] = ParseVector3(strValue);
				break;
			default:
				Debug.LogError("Illegal constant type for " + strKey);
				break;
			}
		}	
	}
}

