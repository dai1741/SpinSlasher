using System;
using UnityEngine;

public class MyPrefs {
	
	//public static const MyPrefs instance = new MyPrefs();
	public static readonly string[] CAMERA_PREF = new string[] {"Ue", "Naname", "Yoko"};
	
	public static int cameraIndex {
		get {
			return PlayerPrefs.GetInt("Camera"); 
		}
		set {
			if(value < 0 || CAMERA_PREF.Length <= value) throw new ArgumentOutOfRangeException();
			PlayerPrefs.SetInt("Camera", value); 
		}
	}
	public static /*float*/ bool soundEnabled {
		get {
			return PlayerPrefs.GetInt("SoundEffect") != 0; 
		}
		set {
			PlayerPrefs.SetInt("SoundEffect", value ? 1 : 0); 
		}
	}
	public static /*float*/ bool musicEnabled {
		get {
			return PlayerPrefs.GetInt("Music") != 0; 
		}
		set {
			PlayerPrefs.SetInt("Music", value ? 1 : 0); 
		}
	}
	public static bool droidUnlocked {
		get {
			return PlayerPrefs.GetInt("DroidUnlocked") != 0; 
		}
		set {
			PlayerPrefs.SetInt("DroidUnlocked", value ? 1 : 0); 
		}
	}
	/*public bool this[string key] {
		get {
			return PlayerPrefs.GetInt(key) != 0; 
		}
		set {
			PlayerPrefs.SetInt(key, value ? 1 : 0); 
		}
	}*/
	
	public static void RestoreDefaults() {
		cameraIndex = 1;
		soundEnabled = musicEnabled = true;
	}
	
	public static void InitDefaults() {
		if(PlayerPrefs.GetInt("Camera", -1) == -1) {
			RestoreDefaults();
			droidUnlocked = false;
		}
	}
}

