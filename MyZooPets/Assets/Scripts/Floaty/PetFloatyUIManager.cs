using UnityEngine;
using System.Collections;

public class PetFloatyUIManager : Singleton<PetFloatyUIManager> {
    public GameObject petFloatyPosition;
     
    public void MoodFloaty(string deltaMood){
        Hashtable option = new Hashtable();
        option.Add("parent", petFloatyPosition);
        option.Add("text", deltaMood);
        option.Add("spriteName", "iconHunger");
        FloatyUtil.SpawnStatsFloatyImageText(option);
    }

}
