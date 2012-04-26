using UnityEngine;
using System.Collections;

public class SettingsGUIFragment {
	
	public enum MenuState {
		KEEP, CHANGED, BACK, RESUME,
	}
	
	public int ChangeBits {
		get; protected set;
	}
	public const int CameraChanged = 0x1;
	public const int SoundChanged = 0x2;
	public const int MusicChanged = 0x4;
	public const int InputChanged = 0x8;
	
	public bool Drawing {
		get; protected set;
	}
	public bool ResumeButtonEnabled {
		get; set;
	}
	
	public int InitialCamera {
		get; protected set;
	}
	public bool InitialSound {
		get; protected set;
	}
	public bool InitialMusic {
		get; protected set;
	}
	public int InitialInput {
		get; protected set;
	}
	public int NewCamera {
		get; protected set;
	}
	public int NewInput {
		get; protected set;
	}
	public bool NewSound {
		get; protected set;
	}
	public bool NewMusic {
		get; protected set;
	}
	
	public bool InitIfNeeded() {
		if(!Drawing) {
			Drawing = true;
			InitialCamera = NewCamera = MyPrefs.CameraIndex;
			InitialInput = NewInput = MyPrefs.InputIndex;
			InitialSound = NewSound = MyPrefs.SoundEnabled;
			InitialMusic = NewMusic = MyPrefs.MusicEnabled;
			return true;
		}
		return false;
	}
	
	public bool End() {
		if(Drawing) {
			Drawing = false;
			NewCamera = MyPrefs.CameraIndex;
			NewInput = MyPrefs.InputIndex;
			NewSound = MyPrefs.SoundEnabled;
			NewMusic = MyPrefs.MusicEnabled;
			return true;
		}
		return false;
	}
	
	
	public MenuState Draw(Rect area) {
		MenuState ret = MenuState.KEEP;
		ChangeBits = 0;
		
		GUILayoutOption widthOp = GUILayout.Width(area.width / 7 * 3);
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Camera:", widthOp);
		int index = MyPrefs.CameraIndex;
		NewCamera = GUILayout.SelectionGrid (index, MyPrefs.CAMERA_PREF, 1);
		
		if(index != NewCamera) {
			MyPrefs.CameraIndex = NewCamera;
			ret = MenuState.CHANGED;
			ChangeBits |= CameraChanged;
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Input:", widthOp);
		int inputIndex = MyPrefs.InputIndex;
		NewInput = GUILayout.SelectionGrid (inputIndex, MyPrefs.INPUT_PREF, 1);
		
		if(inputIndex != NewInput) {
			MyPrefs.InputIndex = NewInput;
			ret = MenuState.CHANGED;
			ChangeBits |= InputChanged;
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		bool soundEnabled = MyPrefs.SoundEnabled;
		NewSound = GUILayout.Toggle(soundEnabled, "Sound Effect");
		if(soundEnabled != NewSound) {
			MyPrefs.SoundEnabled = NewSound;
			EntireGameManager.Instance.PlayClickSound(); //テキトウ
			ret = MenuState.CHANGED;
			ChangeBits |= SoundChanged;
		}
		bool musicEnabled = MyPrefs.MusicEnabled;
		NewMusic = GUILayout.Toggle(musicEnabled, "Music");
		if(musicEnabled != NewMusic) {
			MyPrefs.MusicEnabled = NewMusic;
			ret = MenuState.CHANGED;
			ChangeBits |= MusicChanged;
		}
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		if(GUILayout.Button ("Restore Defaults")) {
			MyPrefs.RestoreDefaults();
			EntireGameManager.Instance.PlayClickSound();
			ret = MenuState.CHANGED;
			if(index != (NewCamera = MyPrefs.CameraIndex)) ChangeBits |= CameraChanged;
			if(inputIndex != (NewInput = MyPrefs.InputIndex)) ChangeBits |= InputChanged;
			if(soundEnabled != (NewSound = MyPrefs.SoundEnabled)) ChangeBits |= SoundChanged;
			if(musicEnabled != (NewMusic = MyPrefs.MusicEnabled)) ChangeBits |= MusicChanged;
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		if(ResumeButtonEnabled && GUILayout.Button ("Resume")) {
			ret = MenuState.RESUME;
		}
		if(GUILayout.Button ("Back")) {
			ret = MenuState.BACK;
		}
		GUILayout.EndHorizontal();
		
		return ret;
	}
	
}
