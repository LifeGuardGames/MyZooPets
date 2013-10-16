using UnityEngine;
using System.Collections.Generic;

public class RunnerGameData{
    public List<string> RunnerItemCollided {get; set;}

    public RunnerGameData(){}

    public void Init(){
        RunnerItemCollided = new List<string>();
    }
}