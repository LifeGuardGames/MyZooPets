using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//----------------------------------------------
// Loading constant Loading scene data from xml file
//----------------------------------------------
public class DataLoadScene : MonoBehaviour {
    private static List<string> loadingScreenNames;

    //----------------------------------------------
    // GetRandomLoadScreen()
    // Very basic way of getting random load screen.
    // could add random by category in the future
    //----------------------------------------------
    public static string GetRandomLoadScreen(){
        if(loadingScreenNames == null){
            loadingScreenNames = new List<string>();
            SetupData();
        }

        int randomIndex = Random.Range(0, loadingScreenNames.Count);

        return loadingScreenNames[randomIndex];
    }

    private static void SetupData(){
        //Load from xml
        TextAsset file = (TextAsset) Resources.Load("LoadingScreens", typeof(TextAsset));
        string xmlString = file.text;

        //Create XMLParser instance
        XMLParser xmlParser = new XMLParser(xmlString);

        //Call the parser to build the IXMLNode objects
        XMLElement xmlElement = xmlParser.Parse();

        //Go through all child node of xmlElement (the parent of the file)
        for(int i=0; i<xmlElement.Children.Count; i++){
            IXMLNode childNode = xmlElement.Children[i];

            Hashtable hashAttr = XMLUtils.GetAttributes(childNode);
            string loadingScreenName = (string)hashAttr["Name"];

            loadingScreenNames.Add(loadingScreenName);
        }
    }
}
