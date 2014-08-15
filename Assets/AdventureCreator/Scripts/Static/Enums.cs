/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Enums.cs"
 * 
 *	This script containers any enum type used by more than one script.
 * 
 */

public enum GameState { Normal, Cutscene, DialogOptions, Paused };

public enum AppearType { Manual, MouseOverInventory, DialogOptions, OnMenuButton, OnHotspot, OnSpeech, Gameplay };
public enum Orientation { Horizontal, Vertical };

public enum InteractionType { Use, Examine, Inventory };
public enum PlayerAction { DoNothing, TurnToFace, WalkTo, WalkToMarker };

public enum ControlStyle { Direct, PointAndClick, FirstPerson };
public enum InputType { MouseAndKeyboard, Controller, TouchScreen };

public enum InteractionIcon { Use, Examine, Talk };
public enum InventoryHandling { ChangeCursor, ChangeHotspotLabel, ChangeCursorAndHotspotLabel };

public enum CharState { Idle, Custom, Move, Decelerate }

public enum FadeType { fadeIn, fadeOut };

public enum CameraLocConstrainType { TargetX, TargetZ, TargetAcrossScreen, TargetIntoScreen };
public enum CameraRotConstrainType { TargetX, TargetZ, TargetAcrossScreen, TargetIntoScreen, LookAtTarget };

public enum MoveMethod { Linear, Smooth, Curved };

public enum AnimLayer {	Base=0, UpperBody=1, LeftArm=2, RightArm=3, Neck=4, Head=5, Face=6, Mouth=7 };
public enum AnimStandard { Idle, Walk, Run };
public enum AnimPlayMode { PlayOnce=0, PlayOnceAndClamp=1, Loop=2 };
public enum AnimPlayModeBase { PlayOnceAndClamp=1, Loop=2 };

public enum PlayerMoveLock { Free=0, AlwaysWalk=1, AlwaysRun=2, NoChange=3 };

public enum TransformType { Translate, Rotate, Scale };

public enum VariableType { Boolean, Integer };
public enum BoolValue { True=1, False=0 };

public enum AC_Direction { None, Up, Down, Left, Right };
public enum ArrowPromptType { KeyOnly, ClickOnly, KeyAndClick };

public enum PathType { Loop, PingPong, ForwardOnly, IsRandom };
public enum PathSpeed { Walk, Run };

public enum SoundType { SFX, Music, Other };