using UnityEngine;
using System.Collections;

public class PetFloatyUIManager : Singleton<PetFloatyUIManager> {
    public GameObject petFloatyPosition;

    public void CreateMoodFloaty(int deltaMood){
        CreateFloaty(deltaMood, "iconHunger");
    }

    public void CreateHealthFloaty(int deltaHealth){
        CreateFloaty(deltaHealth, "iconHeart");
    }

    public void CreatePointsFloaty(int deltaPoints){
        CreateFloaty(deltaPoints, "iconStar");
    }

    //-------------------------------------------------------
    // CreateFloaty()
    // Use the FloatyUtil class to spawn the floaty image text
    // on top of the pet's head
    //-------------------------------------------------------
    private void CreateFloaty(int deltaValue, string spriteName){
        string strDeltaValue = "";

        if(deltaValue > 0)
            strDeltaValue = "+" + deltaValue;
        else
            strDeltaValue = "" + deltaValue;

        Hashtable option = new Hashtable();
        option.Add("parent", petFloatyPosition);
        option.Add("text", strDeltaValue);
        option.Add("spriteName", spriteName);

        FloatyUtil.SpawnStatsFloatyImageText(option);
    }

}
