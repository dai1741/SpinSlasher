using System;
using UnityEngine;

public class MyPrefs {
	
	//public static const MyPrefs instance = new MyPrefs();
	public static readonly string[] CAMERA_PREF = new string[] {"Ue", "Naname", "Yoko"};
	
	public static int CameraIndex {
		get {
			return PlayerPrefs.GetInt("Camera"); 
		}
		set {
			if(value < 0 || CAMERA_PREF.Length <= value) throw new ArgumentOutOfRangeException();
			PlayerPrefs.SetInt("Camera", value); 
		}
	}
	public static /*float*/ bool SoundEnabled {
		get {
			return PlayerPrefs.GetInt("SoundEffect") != 0; 
		}
		set {
			PlayerPrefs.SetInt("SoundEffect", value ? 1 : 0); 
		}
	}
	public static /*float*/ bool MusicEnabled {
		get {
			return PlayerPrefs.GetInt("Music") != 0; 
		}
		set {
			PlayerPrefs.SetInt("Music", value ? 1 : 0); 
		}
	}
	
	public static void RestoreDefaults() {
		CameraIndex = 1;
		SoundEnabled = MusicEnabled = true;
	}
	
	public static void InitDefaults() {
		if(PlayerPrefs.GetInt("Camera", -1) == -1) {
			RestoreDefaults();
		}
	}
}
