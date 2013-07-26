using UnityEngine;
using System.Collections;

public class PetSpeech : Singleton<PetSpeech> {
    public GameObject textMesh;

    public void Talk(string words){
        textMesh.GetComponent<tk2dTextMesh>().text = words;
        textMesh.GetComponent<tk2dTextMesh>().Commit();
        Invoke("StopTalk", 2.0f);
    }

    public void TalkWithImage(TalkImageType type){

    }

    private void StopTalk(){
        textMesh.GetComponent<tk2dTextMesh>().text = "";
        textMesh.GetComponent<tk2dTextMesh>().Commit();
    }

}
