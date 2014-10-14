using UnityEngine;
using System.Collections;

public class MutableDataPetMenuInfo{
    public string PetName {get; set;}
    public string PetColor {get; set;}
    public string PetSpecies {get; set;} 

    public MutableDataPetMenuInfo(){}

    public MutableDataPetMenuInfo(string petName, string petColor, string petSpecies){
        PetName = petName;
        PetColor = petColor;
        PetSpecies = petSpecies;
    }
}
