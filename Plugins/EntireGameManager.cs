using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/*
 * シーンをまたいで存在するScript。
 * 
 */
[RequireComponent (typeof (AudioSource))]
public class EntireGameManager : MonoBehaviour {
	
	public static EntireGameManager instance {
		get; protected set;
	}
	
	public Texture starOnIcon;
	public Texture starOffIcon;
	public GameMode[] gameModes;
	public PlayerInfo[] characters;
	
	public readonly Queue<GameMode> modeUnlockNotificationQueue = new Queue<GameMode>();
	public readonly Queue<PlayerInfo> charaUnlockNotificationQueue = new Queue<PlayerInfo>();
	
	public GameMode currentGameMode { get; set;}
	
	public PlayerInfo currentPlayer;
	
	public int numMinStars { get; protected set;}
	
	void Awake() {
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(this); //gameObject??
			MyPrefs.InitDefaults();
			
			currentPlayer = characters[0];
		}
		else if(instance != this) {
			Destroy(gameObject); //HAHAHA
		}
	}
	
	public void OnApplicationQuit () {
		instance = null;
	}
	
	public static void PlaySE(AudioSource source) {
		if(MyPrefs.soundEnabled) source.Play();
	}
	
	public void PlayClickSound() {
		PlaySE(audio);
	}
	
	public void UnlockIfAny() {
		UnlockGameModes();
		UnlockCharacters();
	}
	
	public void UnlockGameModes() {
		gameModes[0].unlocked = true; //最初は常にunlocked
		foreach(var mode in from m in gameModes
				where !m.unlocked && m.unlockScore <= gameModes[m.unlockModeIndex].score
				select m) {
			mode.unlocked = true;
			modeUnlockNotificationQueue.Enqueue(mode);
		}
	}
	
	public void UnlockCharacters() {
		characters[0].unlocked = true; //最初は常にunlocked
		
		numMinStars = (from n in Enumerable.Range(0, GameMode.RATING_SCORES_COUNT)
				where gameModes.All(m => m.ratingScores[n] <= m.score) select n).Count();
		foreach(var chara in from c in characters
				where !c.unlocked && c.requiredStars <= numMinStars
				select c) {
			chara.unlocked = true;
			charaUnlockNotificationQueue.Enqueue(chara);
		}
	}
	
	public void DrawStarRatingGUILayout(GameMode mode, int score/*, Vector2 size*/) {
		GUILayout.BeginHorizontal();
		
		foreach(int threshold in mode.ratingScores) {
			GUILayout.Label(threshold <= score
					? starOnIcon
			        : starOffIcon, GUILayout.Width(18), GUILayout.Height(18.5f));
			GUILayout.Space(-11); //決め打ちがおおい 
		}
		GUILayout.EndHorizontal();
	}
	
	[ContextMenu ("Init sp score")]
	public void InitScore() {
		gameModes[2].score = -1;
	}
	
	[ContextMenu ("Init chara")]
	public void InitChara() {
		foreach(var chara in characters) chara.unlocked = false;
	}
	
}
