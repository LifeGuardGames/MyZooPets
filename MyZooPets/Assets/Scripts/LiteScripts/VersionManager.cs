using UnityEngine;
using System.Collections;

public class VersionManager : MonoBehaviour {
    private static VersionManager instance;

    public static VersionManager Instance{
        get{
            if(instance == null){
                instance = (VersionManager) FindObjectOfType(typeof(VersionManager));

                if(instance == null){
                    GameObject go = new GameObject("_Version");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<VersionManager>();
                }
            }
            return instance;
        }
    } 

    void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }

    public bool IsLite(){
        return Constants.GetConstant<bool>("IsLiteVersion");
    }
}
