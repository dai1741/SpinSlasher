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
	
	public static EntireGameManager Instance {
		get; protected set;
	}
	
	public Texture starOnIcon;
	public Texture starOffIcon;
	public GameMode[] gameModes;
	public PlayerInfo[] characters;
	
	public readonly Queue<GameMode> modeUnlockNotificationQueue = new Queue<GameMode>();
	public readonly Queue<PlayerInfo> charaUnlockNotificationQueue = new Queue<PlayerInfo>();
	
	public GameMode CurrentGameMode { get; set;}
	
	public PlayerInfo currentPlayer;
	
	public int NumMinStars { get; protected set;}
	
	void Awake() {
		if(Instance == null) {
			Instance = this;
			DontDestroyOnLoad(this); //gameObject??
			MyPrefs.InitDefaults();
			
			currentPlayer = characters[0];
			
			gameObject.AddComponent<GameInitializer>();
		}
		else if(Instance != this) {
			Destroy(gameObject); //HAHAHA
		}
	}
	
	public void OnApplicationQuit () {
		Instance = null;
	}
	
	public static void PlaySE(AudioSource source) {
		if(MyPrefs.SoundEnabled) source.Play();
	}
	
	public void PlayClickSound() {
		PlaySE(audio);
	}
	
	public void UnlockIfAny() {
		UnlockGameModes();
		UnlockCharacters();
	}
	
	public void UnlockGameModes() {
		gameModes[0].Unlocked = true; //最初は常にunlocked
		foreach(var mode in from m in gameModes
				where !m.Unlocked && m.unlockScore <= gameModes[m.unlockModeIndex].Score
				select m) {
			mode.Unlocked = true;
			modeUnlockNotificationQueue.Enqueue(mode);
		}
	}
	
	public void UnlockCharacters() {
		characters[0].Unlocked = true; //最初は常にunlocked
		
		NumMinStars = (from n in Enumerable.Range(0, GameMode.RATING_SCORES_COUNT)
				where gameModes.All(m => m.ratingScores[n] <= m.Score) select n).Count();
		foreach(var chara in from c in characters
				where !c.Unlocked && c.requiredStars <= NumMinStars
				select c) {
			chara.Unlocked = true;
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
	
	public bool IsMobile {
		get {
			// 暫定 
			return Application.platform == RuntimePlatform.Android;
		}
	}
	
	[ContextMenu ("Init sp score")]
	public void InitScore() {
		gameModes[2].Score = -1;
	}
	
	[ContextMenu ("Init chara")]
	public void InitChara() {
		foreach(var chara in characters) chara.Unlocked = false;
	}
	
}
