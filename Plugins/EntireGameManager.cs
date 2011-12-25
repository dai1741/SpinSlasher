using System.Collections;
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
	
	public readonly LimitedQueue modeUnlockNotificationQueue = new LimitedQueue(8);
	public readonly LimitedQueue charaUnlockNotificationQueue = new LimitedQueue(8);
	
	public GameMode CurrentGameMode { get; set;}
	
	public PlayerInfo currentPlayer;
	
	public int NumMinStars { get; protected set;}
	
	void Awake() {
		if(Instance == null) {
			Instance = this;
			DontDestroyOnLoad(this); //gameObject??
			MyPrefs.InitDefaults();
			
			currentPlayer = characters[0];
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
		foreach(var mode in gameModes) {
			if (!mode.Unlocked && mode.unlockScore <= gameModes[mode.unlockModeIndex].Score) {
				mode.Unlocked = true;
				modeUnlockNotificationQueue.Enqueue(mode);
			}
		}
	}
	
	public void UnlockCharacters() {
		characters[0].Unlocked = true; //最初は常にunlocked
		
		int n = GameMode.RATING_SCORES_COUNT - 1;
		for(; n >= 0; n--) {
			bool isOk = true;
			foreach(var mode in gameModes) {
				if (mode.Score < mode.ratingScores[n]) {
					isOk = false;
					break;
				}
			}
			if (isOk) break; // ラベルどうやるんだ？ 
		}
		NumMinStars = n + 1;
		foreach(var chara in characters) {
			if (!chara.Unlocked && chara.requiredStars <= NumMinStars) {
				chara.Unlocked = true;
				charaUnlockNotificationQueue.Enqueue(chara);
			}
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
		gameModes[2].Score = -1;
	}
	
	[ContextMenu ("Init chara")]
	public void InitChara() {
		foreach(var chara in characters) chara.Unlocked = false;
	}
	
}
