using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ComicPage : MonoBehaviour{
    public string audioFile;

    public void PlaySound(){
        if(!String.IsNullOrEmpty(audioFile))
            AudioManager.Instance.PlayClip(audioFile);
    }
}
