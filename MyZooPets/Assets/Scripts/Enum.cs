using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////////////////////
// WARNING: ONLY ADD NEW ENTRIES INTO THE LIST OR IT WILL BREAK ALL ENTRIES!!!
// 			DO NOT DELETE ANY ENTRIES!!!
///////////////////////////////////////////////////////////////////////////////////////////////

public enum Level{
    Level1 = 1,
    Level2,
    Level3,
    Level4,
    Level5,
    Level6,
    Level7,
    Level8
}

public enum PetColor{
	None,
	OrangeYellow,
	PurpleLime,
	BlueYellow,
	YellowPink,
	PinkBlue,
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
	None,
	Foods,
	Usables,
	Decorations,
	Accessories,
}

public enum StatType{
	Xp,
	Coin,
	Health,
	Hunger,
	Fire
}

public enum ChallengeType{
    Weekly,
    Daily,
}

public enum NotificationPopupType{
	TipWithImage,
	LevelUp,
    FireLevelUp,
	SuperWellaInhaler,
	SuperWellaSickReminder,
	ZeroHealth,
	NeedFoodTutorial
}

public enum NotificationPopupData{
	PrefabName,
	Title,
	Message,
	SpecialButtonCallback,
	ExtraInfo
}

public enum TutorialPopupFields{
    Message,
    SpriteAtlas, 
    SpriteName, //requires SpriteAtlas
    Button1Callback,
    ShrinkBgToFitText //T: background will shrink to fit the text size
}

public enum BadgeType{
    Level,
	Inhaler,
	Return, 	// How many times in a row the user returns.
	Coin,
	Decoration,
	Accessory,
	Ninja,
	Memory,
	DoctorMatch,
    Runner,
	Shooter,
	Retention,
}

// NOTE if you add/change these enums make sure to add/change the string key associated with it across ALL string tables!!!
public enum TalkImageType{
    Heart,
}

// various type of modes the UI could be in
// ONLY add enums to the end. Inserting enum will break existing LgButtonMessageClass
public enum UIModeTypes {
	NotInited,	// the variable was not initialized; should throw an error
	None,		// means there is no lock on the click manager
	Generic,	// used by most things that will lock the click manager; this is just a generic lock
	Store,
	EditDecos,
    Tutorial,
    CustomizePet,
    IntroComic,
	MenuSettings,
	GatingSystem,
	MiniPet,
	FireBreathing,
	Accessory,
	Cutscene,
	ParentPortal,
	Friends,
	MembershipCheck,
	Badge,
	FireCrystal,
	Wellapad
}

// decoration node anchor types
public enum DecorationTypes {
    Poster,
    Wallpaper,
    Carpet,
	SmallPlant,
	BigFurniture,
    // FloorTile,
    // TallFurniture,
    // BigPlant,
}

public enum AccessoryTypes{
	Hat,
	Glasses
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
	Sequence,
	Swarms
}

// scoring categories for the ninja game
public enum NinjaScoring {
	Start_1,
	Start_2,
	Start_3,
	Med,
	Med_High,
	High,
	Bonus
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
	Top,
	TopLeft,
	TopRight
}

// reward types for minigames
public enum MinigameRewardTypes {
	XP,
	Money,
	Shard
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

public enum PetSpeechTransitions{
    HappySad,
    SadHappy,
}

// status of checkmarks on the wellapad
public enum WellapadTaskCompletionStates {
	Uncompleted,		// not done
	Completed,			// done
	RecentlyCompleted	// done, but not yet seen on the wellapad
}

// states of dropped items on the ground
public enum DroppedItemStates {
	UnInit,
	Dropped,
	PickedUp,
	Awarded
}

public enum MiniPetTypes{
    None,
	Retention,
	GameMaster,
	Merchant,
}

public enum CurrencyTypes{
	WellaCoin
}

public enum MinigameTypes{
	None,	// Default value
	TriggerNinja,
	Shooter,
	Memory,
	Clinic,
	Runner
}

public enum ZoneTypes{
	None,	// Default value
	Bedroom,
	Yard
}

public enum MiniGameCategory {
	None,
	Critical,
	Regular,
}
