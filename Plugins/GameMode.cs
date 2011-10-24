using System;
using UnityEngine;

[System.Serializable]
public /*struct*/ class GameMode
{
	public string name;
	public string internalName; //pref用。nameは変わることもあるので 
	public GameObject[] creatures;
	public float[] appearOnceIn; // must be creatures.length - 1 == appearOnceIn.length
	public float initialSpeed;
	public float initialSpeedRate = 1;
	public float minSpeedIncrease;
	public float maxSpeedIncrease;
	public float creationInterval;
	public float creationIntervalDecrease;
	public float minCreationInterval;
	
	public float minSpeed;
	public float maxSpeed;
	
	public float timeScale = 1;
	public float initialPlayerHealth = 5;
	public Color bgColor = Color.gray;
	public GameObject field;
	
	public int unlockModeIndex = 0; //unlockkに必要なmode 
	public int unlockScore = 0;
	public int[] ratingScores = new int[]{0, 1000, 2000, 3000, 4000};
	
	public const string HIGHSCORE_PREF_PREFIX = "Highscore_";
	public const string SCORE_SUFFIX_PREF_PREFIX = "ScoreSuffix_";
	public const string UNLOCKED_PREF_PREFIX = "Unlocked_";
	public const int RATING_SCORES_COUNT = 5;
	
	public int score {
		get {
			return PlayerPrefs.GetInt(HIGHSCORE_PREF_PREFIX + internalName, -1); 
		}
		set {
			PlayerPrefs.SetInt(HIGHSCORE_PREF_PREFIX + internalName, value); 
		}
	}
	public string scoreSuffix {
		get {
			return PlayerPrefs.GetString(SCORE_SUFFIX_PREF_PREFIX + internalName, ""); 
		}
		set {
			PlayerPrefs.SetString(SCORE_SUFFIX_PREF_PREFIX + internalName, value); 
		}
	}
	public bool unlocked {
		get {
			return PlayerPrefs.GetInt(UNLOCKED_PREF_PREFIX + internalName, 0) != 0; 
		}
		set {
			PlayerPrefs.SetInt(UNLOCKED_PREF_PREFIX + internalName, value ? 1 : 0); 
		}
	}
}
