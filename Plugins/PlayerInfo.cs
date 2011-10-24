using System;
using UnityEngine;

//Javaのenumのような感じでUnityのInspectorを使いたい。 
[System.Serializable]
public class PlayerInfo {
	public string name;
	public string internalName;
	public string longName;
	public int requiredStars;
	public GameObject playerHolder;
	
	public const string UNLOCKED_PREF_PREFIX = "PlayerUnlocked_";
	public bool unlocked {
		get {
			return PlayerPrefs.GetInt(UNLOCKED_PREF_PREFIX + internalName, 0) != 0; 
		}
		set {
			PlayerPrefs.SetInt(UNLOCKED_PREF_PREFIX + internalName, value ? 1 : 0); 
		}
	}
		
}

