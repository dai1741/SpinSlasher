using UnityEngine;
using System.Collections;

public class SettingsGUIFragment {
	
	public enum MenuState {
		KEEP, CHANGED, BACK, RESUME,
	}
	
	public int changeBits {
		get; protected set;
	}
	public const int CameraChanged = 0x1;
	public const int SoundChanged = 0x2;
	public const int MusicChanged = 0x4;
	
	public bool drawing {
		get; protected set;
	}
	public bool resumeButtonEnabled {
		get; set;
	}
	
	public int initialCamera {
		get; protected set;
	}
	public bool initialSound {
		get; protected set;
	}
	public bool initialMusic {
		get; protected set;
	}
	public int newCamera {
		get; protected set;
	}
	public bool newSound {
		get; protected set;
	}
	public bool newMusic {
		get; protected set;
	}
	
	public bool InitIfNeeded() {
		if(!drawing) {
			drawing = true;
			initialCamera = newCamera = MyPrefs.cameraIndex;
			initialSound = newSound = MyPrefs.soundEnabled;
			initialMusic = newMusic = MyPrefs.musicEnabled;
			return true;
		}
		return false;
	}
	
	public bool End() {
		if(drawing) {
			drawing = false;
			newCamera = MyPrefs.cameraIndex;
			newSound = MyPrefs.soundEnabled;
			newMusic = MyPrefs.musicEnabled;
			return true;
		}
		return false;
	}
	
	
	public MenuState Draw() {
		MenuState ret = MenuState.KEEP;
		changeBits = 0;
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Camera:");
		int index = MyPrefs.cameraIndex;
		newCamera = GUILayout.SelectionGrid (index, MyPrefs.CAMERA_PREF, 1);
		
		if(index != newCamera) {
			MyPrefs.cameraIndex = newCamera;
			ret = MenuState.CHANGED;
			changeBits |= CameraChanged;
		}
		GUILayout.EndHorizontal();
		
		bool soundEnabled = MyPrefs.soundEnabled;
		newSound = GUILayout.Toggle(soundEnabled, "Sound Effect");
		if(soundEnabled != newSound) {
			MyPrefs.soundEnabled = newSound;
			EntireGameManager.instance.PlayClickSound(); //テキトウ
			ret = MenuState.CHANGED;
			changeBits |= SoundChanged;
		}
		bool musicEnabled = MyPrefs.musicEnabled;
		newMusic = GUILayout.Toggle(musicEnabled, "Music");
		if(musicEnabled != newMusic) {
			MyPrefs.musicEnabled = newMusic;
			ret = MenuState.CHANGED;
			changeBits |= MusicChanged;
		}
		
		GUILayout.FlexibleSpace();
		if(GUILayout.Button ("Restore Defaults")) {
			MyPrefs.RestoreDefaults();
			EntireGameManager.instance.PlayClickSound();
			ret = MenuState.CHANGED;
			if(index != (newCamera = MyPrefs.cameraIndex)) changeBits |= CameraChanged;
			if(soundEnabled != (newSound = MyPrefs.soundEnabled)) changeBits |= SoundChanged;
			if(musicEnabled != (newMusic = MyPrefs.musicEnabled)) changeBits |= MusicChanged;
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		if(resumeButtonEnabled && GUILayout.Button ("Resume")) {
			ret = MenuState.RESUME;
		}
		if(GUILayout.Button ("Back")) {
			ret = MenuState.BACK;
		}
		GUILayout.EndHorizontal();
		
		return ret;
	}
	
}
