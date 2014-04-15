using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Callbacks;

public class CustomPostprocesScript : MonoBehaviour {

    [PostProcessBuild]
    public static void OnPostprocessBuildPlayer(BuildTarget target, string buildPath){
        UnityEngine.Debug.Log(target);
        UnityEngine.Debug.Log(buildPath);
        UnityEngine.Debug.Log(Application.dataPath);

        UnityEngine.Debug.Log("----Custome Script---Executing post process build phase.");      
        // string objCPath = Application.dataPath + "/ObjC";
        Process myCustomProcess = new Process();        

        myCustomProcess.StartInfo.FileName = "python";

        myCustomProcess.StartInfo.Arguments = string.Format("Assets/Editor/post_process.py \"{0}\"", buildPath);
        myCustomProcess.StartInfo.UseShellExecute = false;
        myCustomProcess.StartInfo.RedirectStandardOutput = false;
        myCustomProcess.Start(); 
        myCustomProcess.WaitForExit();
        UnityEngine.Debug.Log("----Custome Script--- Finished executing post process build phase.");  
    }
}
