﻿using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------
// StringUtils
// Utility functions for formatting/localizing strings.
// Jason: A better way to do this is by using System.String.Format ....too late to change now
//---------------------------------------------------

public class StringUtils {

	// public static string NUM = "NUM";
	// public static string NUM_2 = "NUM2";
	// public static string NUM_3 = "NUM3";
	// public static string TIME = "TIME";
	// // public static string PLURAL_S = "$PLURAL_S$";
	// // public static string PLURAL_ES = "$PLURAL_ES$";
	
	// //---------------------------------------------------
	// // Replace()
	// // Replaces a tag in a string with a value.
	// //---------------------------------------------------	
	// public static string Replace( string i_str, string i_strKey, string i_strVal) {
	// 	string str = i_str;
	// 	string key = "$" + i_strKey + "$";	
	// 	string strResult = str.Replace(key, i_strVal);

	// 		// int amount = Int32.Parse(i_strVal);
	// 		// if(amount > 1){
	// 		// 	strResult = strResult.Replace(PLURAL_S, "s");
	// 		// 	strResult = strResult.Replace(PLURAL_ES, "es");
	// 		// }
		
	// 	return strResult;
	// }
	
	// public static string Replace( string i_str, string i_strKey, int i_nVal ) {
	// 	string strVal = FormatNumber( i_nVal );
	// 	return StringUtils.Replace( i_str, i_strKey, strVal );
	// }
	
	/*
	static string Replace( string i_str, string i_strKey, float i_fVal ) {
		string strVal = FormatNumber( i_fVal );
		return StringUtils.Replace( i_str, i_strKey, strVal );
	}
	*/
	
	public static string FormatNumber( int i_nVal ) {
		string strDelim = Localization.Localize( "NUMBER_DELIMETER" );
		string strVal = i_nVal.ToString("n0");
		
		strVal = strVal.Replace(",", strDelim);
	
		return strVal;
	}
}
