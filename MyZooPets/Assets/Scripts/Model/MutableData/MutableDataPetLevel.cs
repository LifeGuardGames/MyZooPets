public class MutableDataPetLevel{
    public Level CurrentLevel {get; set;} //pets current level 

    //================Initialization============
    public MutableDataPetLevel(){
        Init();
    }

    private void Init(){
 //      CurrentLevel = Level.Level1;
    	CurrentLevel = Level.Level8;
    } 
}
