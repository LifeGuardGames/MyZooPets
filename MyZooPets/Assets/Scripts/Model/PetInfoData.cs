using UnityEngine;
using System.Collections;

[DoNotSerializePublic]
public class PetInfoData{
    [SerializeThis]
    private string petID;
    [SerializeThis]
    private string petName;
    [SerializeThis]
    private string petColor;
    [SerializeThis]
    private bool isHatched;

    public string PetID{
        get{return petID;}
        set{petID = value;}
    }

    public string PetName{
        get{return petName;}
        set{petName = value;}
    }

    public string PetColor{
        get{return petColor;}
        set{petColor = value;}
    }

    public bool IsHatched{
        get{return isHatched;}
        set{isHatched = value;}
    }

    public PetInfoData(){}

    public void Init(){
        this.petID = "";
        this.petName = "LazyWinkle";
        this.petColor = "whiteBlue";
        this.isHatched = false;
    }
}