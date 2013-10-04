using UnityEngine;
using System.Collections;

//---------------------------------------------------
// PetInfoData 
// Save data for PetInfo. Mutable data 
//---------------------------------------------------
public class PetInfoData{
    public string PetID {get; set;}
    public string PetName {get; set;}
    public string PetColor {get; set;}
    public bool IsHatched {get; set;}

    public PetInfoData(){}

    public void Init(){
        PetID = "";
        PetName = "LazyWinkle";
        PetColor = "whiteBlue";
        IsHatched = false;
    }
}