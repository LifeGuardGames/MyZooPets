﻿using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

//---------------------------------------------------
// StringUtils
// Utility functions for formatting/localizing strings.
// Jason: A better way to do this is by using System.String.Format ....too late to change now
//---------------------------------------------------

public class StringUtils {

	public static string FormatStringPossession(string oldString){
		if(string.IsNullOrEmpty(oldString)){
			Debug.LogWarning("Formatting an empty string");
			return "";
		}

		string subString = oldString.Substring(oldString.Length - 1);
		string newString = "";

		if(subString == "s")
			newString = oldString + "'";
		else
			newString = oldString + "'s";

		return newString;
	}

	public static string FormatNumber( int i_nVal ) {
		string strDelim = Localization.Localize( "NUMBER_DELIMETER" );
		string strVal = i_nVal.ToString("n0");
		
		strVal = strVal.Replace(",", strDelim);
	
		return strVal;
	}

    // <Description StringFormat="">
    // <Binding Path=""></Binding>
    // </Description>
    // working progress......probably need type information for each binding
    // how to handle plural, and possession cases.
	public static string FormatString(Type classType, IXMLNode stringBindings){
	 	List<PropertyInfo> propertyInfo= new List<PropertyInfo>();
        string retValue = "";

        Hashtable hashAttr = XMLUtils.GetAttributes(stringBindings);
        string stringFormat = "";

        if(hashAttr.ContainsKey("StringFormat"))
        	stringFormat = (string) hashAttr["StringFormat"];
        else
        	Debug.LogError("string to be formatted can't be found for class " + classType);

        //Localize the string	
        string baseString = Localization.Localize(stringFormat);

        //Get all the bindings
        List<IXMLNode> childrenList = XMLUtils.GetChildrenList(stringBindings);

        foreach(IXMLNode node in childrenList){
            Hashtable bindingAttr = XMLUtils.GetAttributes(node);
            string binding = (string) bindingAttr["Path"];

            propertyInfo.Add(classType.GetProperty(binding));
        }

        //format the base string with the format items
        retValue = String.Format(baseString, propertyInfo);

        return retValue;	
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
}
