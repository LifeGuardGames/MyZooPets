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
    Unknown,
    Miss,
    Null, //before pet is hatched 
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
	Sound
}
public enum BadgeType{
    Level,
    RunnerDistance,
    PatientNumber,
    Decoration
}

// NOTE if you add/change these enums make sure to add/change the string key associated with it across ALL string tables!!!
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
    Poster,
    Wallpaper,
    Carpet,
    // FloorTile,
    // TallFurniture,
    // BigFurniture,
    // SmallPlant,
    // BigPlant,
}

// mini game states
public enum MinigameStates {
	Opening,	// scene has just been loaded, opening UI is up
	Playing,	// the game is playing, no UI is up
	Paused,		// the game is paused, paused game UI is up
	GameOver,	// the game has ended, game over UI is up
	Restarting
}

// preferences
public enum Preferences {
	Sound,
	Music
}

// pet anim states
public enum PetAnimStates {
	Idling,
	Walking,
	Transitioning,
	BreathingFire
}

// mood states
// Any must come last, if anyone adds new mood states
public enum PetMoods {
	Happy,
	Sad,
	Any
}

// health states
// Joe: I decided not to map these to the asthma stages because I felt they were more unique, and we might possibly want
// to expand then one day.
public enum PetHealthStates {
	Healthy,
	Sick,
	VerySick
}

// direction used in moving and gating
public enum RoomDirection {
    Left,
    Right
}

// click lock exceptions...uh...I feel like this is setting a bad precedent
public enum ClickLockExceptions{
	None,
	Moving
}

// patterns for the trigger ninja game
public enum NinjaPatterns {
	Separate,
	Clustered,
	Cross,
	Split,
	Meet,
	Sequence
}

// scoring categories for the ninja game
public enum NinjaScoring {
	Start_1,
	Start_2,
	Start_3,
	Med,
	Med_High,
	High
}

// ninja game modes
public enum NinjaModes {
	Classic
}

// minigame UI
public enum MinigameLabels {
	Score,
	Lives
}

// minigame popups
public enum MinigamePopups {
	Opening,
	Pause,
	GameOver
}

public enum HUDElementType{
	Points, 
	Stars, 
	Health, 
	Mood
}

// types of button clicks
public enum ButtonClickTypes {
	Tap,
	Hold
}

// types of NGUI interface anchors
public enum InterfaceAnchors {
	Bottom,
	BottomLeft,
	BottomRight,
	Center,
	Left,
	Right,
	Top
}

// reward types for minigames
public enum MinigameRewardTypes {
	XP,
	Money
}

// reward statuses for wellapad missions
public enum RewardStatuses {
	Unearned,
	Unclaimed,
	Claimed
}

// time frames
public enum TimeFrames {
	Morning,
	Evening
}