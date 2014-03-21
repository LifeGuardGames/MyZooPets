using UnityEngine;
using System.Collections;

public class VersionManager{

    public static bool IsLite(){
        return Constants.GetConstant<bool>("IsLiteVersion");
    }
}
