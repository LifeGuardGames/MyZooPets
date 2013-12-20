using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////
// ListUtils
// Misc utility functions for lists.
///////////////////////////////////////////

public static class ListUtils {
	
	///////////////////////////////////////////
	// GetRandomElement()
	// Given a list, will return one element
	// at random from the list.
	///////////////////////////////////////////	
	public static T GetRandomElement<T>( List<T> list ) {
		List<T> listOne = GetRandomElements<T>( list, 1 );
		return listOne[0];
	}	
	
	///////////////////////////////////////////
	// GetRandomElements()
	// Given a list, will return another list
	// comprised of random, unique elements of
	// the incoming list.
	// Note that I am not throwing an error if
	// 0 comes in as nCount.  I decided it's up
	// to whomever is calling this function to
	// care about that, if they do.
	///////////////////////////////////////////
	public static List<T> GetRandomElements<T>( List<T> list, int nCount )
	{
		// simple check to make sure we aren't trying to get more elements than are in the array
		if ( nCount < 0 || nCount > list.Count )
		{
			Debug.Log("Attempted to get random # of elements from an array too small!  Shrinking elements to match array length.");
			nCount = list.Count;
		}
		
		List<T> listResults = new List<T>();		// list with results function will return
		List<T> listCopy = new List<T>(list);		// a copy of the list, since we manipulate it
		
		// while we haven't reached our number of results...
		while ( listResults.Count < nCount && nCount > 0 )
		{
			// get a random element from the remaining elements in arrayCopy
			// and then stick it in the results
			int nRandom = UnityEngine.Random.Range(0, listCopy.Count);
			listResults.Add(listCopy[nRandom]);
			
			// remove that object from the array of possible objects so we get unique results
			listCopy.RemoveAt(nRandom);
		}
		
		return listResults;
	}
	
	///////////////////////////////////////////
	// Shuffle()
	// Shuffles a list.  I got this off the
	// interwebs.
	///////////////////////////////////////////	
	public static void Shuffle<T>(this IList<T> list)  
	{  
	    System.Random rng = new System.Random();  
	    int n = list.Count;  
	    while (n > 1) {  
	        n--;  
	        int k = rng.Next(n + 1);  
	        T value = list[k];  
	        list[k] = list[n];  
	        list[n] = value;  
	    }  
	}	

}
