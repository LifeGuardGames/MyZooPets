using UnityEngine;
using System.Collections;

public enum Level{
    Level0,
    Level1,
    Level2,
    Level3,
    Level4,
    Level5,
    Level6,
    Level7,
    Level8,
    Level9,
    Level10,
    Level11,
    Level12,
    Level13,
    Level14,
    Level15,
    Level16,
    Level17,
    Level18,
    Level19,
    Level20,
}

public enum DosageRecord{
    Hit,
    Null,
    Miss,
    LeaveBlank, // for those parts that should be
}

public enum InhalerType{
    Advair,
    Rescue,
}

public enum AsthmaStage{
    OK,
    Sick,
    Attack,
}
public enum ItemType{
	Foods,
	Usables,
	Decorations,
}
public enum StatType{
    Mood,
    Health,
}
public enum BadgeTier{
	Null,
	Bronze,
	Silver,
	Gold,
}
public enum ChallengeType{
    Weekly,
    Daily,
}

public enum NotificationPopupType{
	OneButton,
	TwoButtons,
	GameOverRewardOneButton,
	GameOverRewardTwoButton,
	TutorialLeft,
	TipWithImage,
	LevelUp,
}

public enum NotificationPopupFields{
	Type,
	Message,
	Button1Label,
	Button1Callback,
	Button2Label,
	Button2Callback,
	DeltaStars,
	DeltaPoints,
	SpriteName,
	StartsHidden,
	HideImmediately,
	Badge,
	TutorialImageType,
}

public enum NotificationType{
    YesNo,
    OK,
}

public enum NotificationTitle{
    Great,
    NiceTry,
}

public enum ItemReceiver{
	Floor,
	Wall,
	Pet,
}

public enum ZoomItem{
    Pet,
    TrophyShelf,
    SlotMachine,
    RealInhaler,
    PracticeInhaler,
    BadgeBoard,
    Dojo
}
public enum BadgeType{
    Level,
    Other
}

// NOTE if you add/change these enums make sure to add/change the string key associated with it across ALL string tables!!!
public enum DojoSkillName{
    Sit,
    Wave,
    Backflip,
    HighFive,
    DiscoMove,
    SadFace,
    Konfu,
    Chilling,
    HighJump,
}

public enum TalkImageType{
    Heart,
}

public enum TutorialImageType{
    CalendarIntro,
    CalendarGreenStamp,
    CalendarRedStamp,
    CalendarBonus
}

// various type of modes the UI could be in
public enum UIModeTypes {
	None,
	Store,
	EditDecos
}

// decoration node anchor types
public enum DecorationTypes {
	Wall,
	Floor,
	Wallpaper,
	Carpet
}

// mini game states
public enum MinigameStates {
	Opening,	// scene has just been loaded, opening UI is up
	Playing,	// the game is playing, no UI is up
	Paused,		// the game is paused, paused game UI is up
	GameOver	// the game has ended, game over UI is up
}

// preferences
public enum Preferences {
	Sound,
	Music
}
