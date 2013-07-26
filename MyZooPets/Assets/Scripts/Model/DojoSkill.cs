using UnityEngine;
using System.Collections;

public class DojoSkill{

    private string skillName;
    private string desc;
    private string iconName;
    private string petAnimationName;
    private string gestureAnimationName;
    private int costStars;

    public string SkillName{
        get{return skillName;}
    }
    public string Desc{
        get{return desc;}
    }
    public string IconName{
        get{return iconName;}
    }
    public string PetAnimationName{
        get{return petAnimationName;}
    }
    public string GestureAnimationName{
        get{return gestureAnimationName;}
    }
    public int CostStars{
        get{return costStars;}
    }

    public DojoSkill(string skillName, string desc, string iconName, string petAnimationName, string gestureAnimationName, int costStars){
        this.skillName = skillName;
        this.desc = desc;
        this.iconName = iconName;
        this.petAnimationName = petAnimationName;
        this.gestureAnimationName = gestureAnimationName;
        this.costStars = costStars;
    }
}
