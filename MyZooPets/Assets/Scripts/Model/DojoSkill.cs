using UnityEngine;
using System.Collections;

public class DojoSkill{

    private string skillName;
    private string desc;
    private string iconName;
    private string petAnimationName;
    private string gestureAnimationName;
    private int costStars;
    private bool isUnLocked;

    public string SkillName{ //name of the dojo skill
        get{return skillName;}
    }
    public string Desc{ //description of the dojo skill.
        get{return desc;}
    }
    public string IconName{
        get{return iconName;}
    }
    public string PetAnimationName{ //name of the animation in sprite collection
        get{return petAnimationName;}
    }
    public string GestureAnimationName{ //name of the gesture animation in sprite collection
        get{return gestureAnimationName;}
    }
    public int CostStars{ //how much it costs to buy the dojo skill
        get{return costStars;}
    }
    public bool IsUnlocked{ //has the skill been unlocked
        get{return isUnLocked;}
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
