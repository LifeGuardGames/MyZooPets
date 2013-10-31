using UnityEngine;
using System.Collections;

//---------------------------------------------------
// LoadSceneData
// use to store scene data when transitioning to a new scene
//---------------------------------------------------
public class LoadSceneData{
    private string lastScene;
    private Vector3 lastPetPosition;
    private int lastCameraPartition;

    public string LastScene{
        get{return lastScene;}
    }

    public Vector3 LastPetPosition{
        get{return lastPetPosition;}
    }

    public int LastCameraPartition{
        get{return lastCameraPartition;}
    }

    public LoadSceneData(string lastScene, Vector3 lastPetPosition, int lastCameraPartition){
        this.lastScene = lastScene;
        this.lastPetPosition = lastPetPosition;
        this.lastCameraPartition = lastCameraPartition;
    }
}
