using UnityEngine;
using System.Collections;

public class MutableData_PetMenuInfo{
    public string PetName {get; set;}
    public string PetColor {get; set;}
    public string PetSpecies {get; set;} 

    public MutableData_PetMenuInfo(){}

    public MutableData_PetMenuInfo(string petName, string petColor, 
        string petSpecies){
        PetName = petName;
        PetColor = petColor;
        PetSpecies = petSpecies;
    }
}
