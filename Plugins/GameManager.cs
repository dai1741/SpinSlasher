using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance {
		get; protected set;
	}
	
	public float timeScale = 1;
	public float initialHealth = 5;
	public float health;
	public float maxSpinPoint = 10;
	public float spinPoint;
	
	private float _spinPointNeededToRespin;
	public float spinPointNeededToRespin {
		get {
			return _spinPointNeededToRespin;
		}
		set {
			_spinPointNeededToRespin = value;
			//SPバーの設定 
			maxSpinGUISizeRate = GameObject.Find("MaxSpinPointBar").GetComponent<GUITexture>().pixelInset.width / maxSpinPoint;
			var texture = GameObject.Find("DangerSpinPointBar").GetComponent<GUITexture>();
			texture.pixelInset = new Rect(texture.pixelInset.x, texture.pixelInset.y,
					spinPointNeededToRespin * maxSpinGUISizeRate, texture.pixelInset.height);
		}
	}
	
	public Texture2D healthIcon;
	public Texture2D tweetButtonIcon;
	
	public string tweetFormatString;
	public string gameUrl;
	
	//public AudioChain bgm;
	private PlayableAudio bgm; //ファーストパスなのでコンパイラはAudioChainを解決できない
	
	public bool gameover {
		get; protected set;
	} //デバッグ時以外はこれがtrueになったらもうfalseにはならない
	
	public bool paused {
		get; protected set;
	}
	
	private Camera _mainCamera;
	public Camera mainCamera {
		get { return _mainCamera; }
		protected set {
			if(_mainCamera != null) {
				_mainCamera.enabled = false;
				_mainCamera.GetComponent<AudioListener>().enabled = false;
				_mainCamera.light.enabled = false;
			}
			_mainCamera = value;
			_mainCamera.enabled = true;
			_mainCamera.GetComponent<AudioListener>().enabled = true;
			//_mainCamera.backgroundColor = mode.bgColor;
		}
	}
	
	private float maxSpinGUISizeRate;
	
	private SettingsGUIFragment settingsGUI = new SettingsGUIFragment();
	
	[System.NonSerialized]
	public int score = 0;
	
	public bool soundEnabled {
		get; protected set;
	}
	
	void Awake() {
		instance = this;
	}
	
	void Start() {
		if(EntireGameManager.instance == null) { //for debug
			Application.LoadLevel("titleScene");
			return;
		}
		Instantiate(EntireGameManager.instance.currentPlayer.playerHolder);
		MigrateGameInfo();
		
		health = initialHealth;
		spinPoint = maxSpinPoint;
		
		updateCamera();
		
		soundEnabled = MyPrefs.soundEnabled;
		bgm = (PlayableAudio) GetComponent("AudioChain");
		if(MyPrefs.musicEnabled) bgm.StartAudio();
		
		settingsGUI.resumeButtonEnabled = true;
		
		Instantiate(mode.field);
		
		RenderSettings.skybox.SetColor("_Tint", Color.Lerp(mode.bgColor, Color.white, .3f));
		//カメラの背景色用に作った色なのでskyboxではlerpする 
	}
	
	void OnDestroy() {
		RenderSettings.skybox.SetColor("_Tint", new Color(.5f, .5f, .5f, .5f));
	}
	
	private GameMode mode;
	private void MigrateGameInfo() {
		mode = EntireGameManager.instance.currentGameMode;
		Time.timeScale = timeScale = mode.timeScale;
		initialHealth = mode.initialPlayerHealth;
	}
	
	void updateCamera() {
		int i = PlayerPrefs.GetInt("Camera");
		mainCamera = GameObject.Find(MyPrefs.CAMERA_PREF[i] + " Camera").GetComponent<Camera>();
	}
	
	void Update() {
		if(Input.GetButtonDown("Pause")) {
			setPause(!paused);
			showingSettings = false;
		}
		AudioListener.pause = paused;
	}
	
	public void setPause(bool pause) {
		paused = pause;
		Time.timeScale = paused ? 0 : timeScale;
	}
	
	void OnApplicationPause(bool pause) {
		if(pause) setPause(true);
		AudioListener.pause = paused;
	}
	
	private bool showingSettings = false;
	private bool gotHighscore = false;
	
	void OnGUI() {
		var texture = GameObject.Find("SpinPointBar").GetComponent<GUITexture>();
		texture.pixelInset = new Rect(texture.pixelInset.x, texture.pixelInset.y,
				spinPoint * maxSpinGUISizeRate, texture.pixelInset.height);
		
		if(gameover) {
			Time.timeScale = 0;
			
			GUI.Box (new Rect (Screen.width/2 - 120, Screen.height/2 - 120, 240, 240), "Game Over");
			GUILayout.BeginArea (new Rect (Screen.width/2 - 110, Screen.height/2 - 80, 220, 190));
		
			GUILayout.BeginHorizontal();
			GUILayout.Label("Your score: " + score);
			EntireGameManager.instance.DrawStarRatingGUILayout(mode, score);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			if(gotHighscore || mode.score < score) {
				gotHighscore = true;
				mode.score = score;
				mode.scoreSuffix = EntireGameManager.instance.currentPlayer.highscoreSuffix;
				GUILayout.Label("You've got a highscore in " + mode.name + " mode! Well done :)");
			}
			else {
				GUILayout.Label("(Best score: " + mode.score + ")");
			}
			if(GUILayout.Button (new GUIContent(tweetButtonIcon, "Tweet the score!"),
			                     GUIStyle.none, GUILayout.ExpandWidth(false))) {
				//プラグインあるからoauth使おうと思ったけど、面倒なのでやめた… 
				//webplayerならスコア改ざん防止のチェックサム(というかXorShift)付きでjsに投げて
				//ajaxからサーバーサイドからtwitterにアクセスするのが一番楽かな 
				var url = "https://twitter.com/share?text="
						+ WWW.EscapeURL(string.Format(tweetFormatString, mode.name, score))
						+ "&url=" + WWW.EscapeURL(gameUrl);
				if(Application.isWebPlayer) Application.ExternalEval("window.open('" + url + "')");
				else Application.OpenURL(url);
			}
			GUILayout.EndHorizontal();
			
			//ツールチップを表示 
			var orig = GUI.skin.label.alignment;
			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			GUILayout.Label(GUI.tooltip);
			GUI.skin.label.alignment = orig;
			
			
			if(Debug.isDebugBuild && GUILayout.Button ("Retry (Debug)")) {
				gameover = false;
				Time.timeScale = timeScale;
				
				health = initialHealth;
			}
			else if(GUILayout.Button ("Retry")) {
				Application.LoadLevel("playroom");
			}
			else if(GUILayout.Button ("Back to Menu")) {
				Application.LoadLevel("titleScene");
			}
			GUILayout.FlexibleSpace();
		
			GUILayout.EndArea();
		}
		else if(paused) {
			Time.timeScale = 0;
			
			GUI.Box (new Rect (Screen.width/2 - 120, Screen.height/2 - 120, 240, 240),
			         showingSettings ? "Pause - Settings" : "Pause");
			GUILayout.BeginArea (new Rect (Screen.width/2 - 110, Screen.height/2 - 80, 220, 190));
			
			if(!showingSettings) {
				if(GUILayout.Button ("Resume")) {
					paused = false;
				}
				else if(GUILayout.Button ("Retry")) {
					if(mode.score < score) mode.score = score;
					Application.LoadLevel("playroom");
				}
				else if(GUILayout.Button ("Quit")) {
					if(mode.score < score) mode.score = score;
					Application.LoadLevel("titleScene");
				}
				else if(GUILayout.Button ("Settings")) {
					showingSettings = true;
				}
			}
			else {
				settingsGUI.InitIfNeeded();
				SettingsGUIFragment.MenuState state = settingsGUI.Draw();
				//C#のswitch文むずすぎ 
				if(state == SettingsGUIFragment.MenuState.CHANGED) {
					if((settingsGUI.changeBits & SettingsGUIFragment.CameraChanged) != 0) {
						updateCamera();
					}
				}
				else if(state == SettingsGUIFragment.MenuState.RESUME
				        || state == SettingsGUIFragment.MenuState.BACK) {
					paused = state == SettingsGUIFragment.MenuState.BACK;
					showingSettings = false;
					settingsGUI.End();
					soundEnabled = settingsGUI.newSound;
					if(settingsGUI.initialMusic != settingsGUI.newMusic) {
						if(settingsGUI.newMusic) bgm.StartAudio();
						else {
							bgm.StopAudio();
						}
					}
					
				}
			}
		
			GUILayout.EndArea();
			Time.timeScale = paused ? 0 : timeScale;
		}
		
		const float initX = 10, iconSize = 30, intervalX = 10;
		float initY = Screen.height - 15 - iconSize;
		var healthIconArea = new Rect(initX, initY, iconSize, iconSize);
		for(int i = 0; i < health; i++, healthIconArea.x += iconSize + intervalX) {
			GUI.Label(healthIconArea, healthIcon);
		}
		
		float statusWidth = 135 + mode.name.Length * 7;
		GUI.Label(new Rect(Screen.width - statusWidth, Screen.height - 20, statusWidth, 20),
		          "Mode: " +  mode.name + "   Score: " + score);
	}

	public void OnGameover() {
		gameover = true;
	}
	
	//EntireGameManagerにも同じ関数があるけど、こちらの方が速い
	public void PlaySE(AudioSource source) {
		if(soundEnabled) source.Play();
	}
}