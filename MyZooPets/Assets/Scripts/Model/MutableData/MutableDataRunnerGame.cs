using UnityEngine;
using System.Collections.Generic;

public class MutableDataRunnerGame{
    public List<string> RunnerItemCollided {get; set;}

    public MutableDataRunnerGame(){
        Init();
    }

    private void Init(){
        RunnerItemCollided = new List<string>();
    }
}