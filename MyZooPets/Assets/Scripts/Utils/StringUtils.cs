using UnityEngine;
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
	public static string FormatStringPossession(string oldString) {
		if(string.IsNullOrEmpty(oldString)) {
			Debug.LogWarning("Formatting an empty string");
			return "";
		}

		string subString = oldString.Substring(oldString.Length - 1);
		string newString = "";

		if(subString == "s") {
			newString = oldString + "'";
		}
		else {
			newString = oldString + "'s";
		}
		return newString;
	}

	public static string FormatNumber(int i_nVal) {
		string strDelim = Localization.Localize("NUMBER_DELIMETER");
		string strVal = i_nVal.ToString("n0");
		strVal = strVal.Replace(",", strDelim);
		return strVal;
	}

	/// <summary>
	/// Probably need type information for each binding how to handle plural, and possession cases.
	/// </summary>
	public static string FormatString(Type classType, IXMLNode stringBindings) {
		List<PropertyInfo> propertyInfo = new List<PropertyInfo>();

		Hashtable hashAttr = XMLUtils.GetAttributes(stringBindings);
		string stringFormat = "";

		if(hashAttr.ContainsKey("StringFormat")) {
			stringFormat = (string)hashAttr["StringFormat"];
		}
		else {
			Debug.LogError("string to be formatted can't be found for class " + classType);
		}

		//Localize the string	
		string baseString = Localization.Localize(stringFormat);

		//Get all the bindings
		List<IXMLNode> childrenList = XMLUtils.GetChildrenList(stringBindings);

		foreach(IXMLNode node in childrenList) {
			Hashtable bindingAttr = XMLUtils.GetAttributes(node);
			string binding = (string)bindingAttr["Path"];
			propertyInfo.Add(classType.GetProperty(binding));
		}

		//format the base string with the format items
		return string.Format(baseString, propertyInfo);
	}

	public static Vector3 ParseVector3(string vectorString) {
		Vector3 vector = new Vector3(0, 0, 0);
		string[] arrayVector3;

		try {
			arrayVector3 = vectorString.Split(","[0]);
			if(arrayVector3.Length == 3) {
				vector = new Vector3(
					float.Parse(arrayVector3[0].Trim(new char[] { '(' })),
					float.Parse(arrayVector3[1]),
					float.Parse(arrayVector3[2].Trim(new char[] { ')' })));
			}
			else
				Debug.LogError("Illegal vector3 parsing, reverting to 0,0,0");
		}
		catch(NullReferenceException e) {
			Debug.LogError("Vector3 parsing. string cannot be null. error message: " + e.Message);
		}
		return vector;
	}

	/// <summary>
	/// Formats the int to double digit string.
	/// Useful for making integer counts into string for XML use
	/// </summary>
	/// <returns>Double digit string representation of the number</returns>
	/// <param name="number">Number to convert</param>
	public static string FormatIntToDoubleDigitString(int number) {
		if(number > 99 || number < 0) {
			Debug.LogError("Unsupported input for number detected " + number);
			return "00";
		}
		else {
			if(number > 9) {
				return number.ToString();
			}
			else {
				return "0" + number.ToString();
			}
		}
	}

	/// <summary>
	/// Formats a TimeSpan into a xH:xM:xS format
	/// </summary>
	/// <returns>The time left.</returns>
	/// <param name="timeLeft">Time left</param>
	public static string FormatTimeLeft(TimeSpan timeLeft) {
		if(timeLeft.Hours > 0) {
			return string.Format("{0}h {1}m {2}s", timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
		}
		else if(timeLeft.Minutes > 0) {
			return string.Format("{0}m {1}s", timeLeft.Minutes, timeLeft.Seconds);
		}
		else {
			return string.Format("{0}s", timeLeft.Seconds);
		}
	}
}
