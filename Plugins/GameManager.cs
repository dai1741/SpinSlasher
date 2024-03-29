using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager Instance {
		get; protected set;
	}
	
	public float timeScale = 1;
	public float initialHealth = 5;
	public float health;
	public float maxSpinPoint = 10;
	public float spinPoint;
	
	private float _spinPointNeededToRespin;
	public float SpinPointNeededToRespin {
		get {
			return _spinPointNeededToRespin;
		}
		set {
			_spinPointNeededToRespin = value;
			//SPバーの設定 
			maxSpinGUISizeRate = GameObject.Find("MaxSpinPointBar").GetComponent<GUITexture>().pixelInset.width / maxSpinPoint;
			var texture = GameObject.Find("DangerSpinPointBar").GetComponent<GUITexture>();
			texture.pixelInset = new Rect(texture.pixelInset.x, texture.pixelInset.y,
					SpinPointNeededToRespin * maxSpinGUISizeRate, texture.pixelInset.height);
		}
	}
	
	public Texture2D healthIcon;
	public Texture2D tweetButtonIcon;
	
	public string tweetFormatString;
	public string gameUrl;
	
	//public AudioChain bgm;
	private PlayableAudio bgm; //ファーストパスなのでコンパイラはAudioChainを解決できない
	
	public bool Gameover {
		get; protected set;
	} //デバッグ時以外はこれがtrueになったらもうfalseにはならない
	
	public bool Paused {
		get; protected set;
	}
	
	private Camera _mainCamera;
	public Camera MainCamera {
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
	void UpdateCamera() {
		MainCamera = MyPrefs.GetCameraObject().GetComponent<Camera>();
	}
	
	private InputReader _inputReader;
	public InputReader InputReader {
		get {
			if(_inputReader == null) UpdateInputReader();
			return _inputReader;
		}
	}
	public void UpdateInputReader() {
		_inputReader = MyPrefs.GetInputReader();
	}
	
	private float maxSpinGUISizeRate;
	
	private SettingsGUIFragment settingsGUI = new SettingsGUIFragment();
	
	[System.NonSerialized]
	public int score = 0;
	
	public bool SoundEnabled {
		get; protected set;
	}
	
	void Awake() {
		Instance = this;
	}
	
	void Start() {
		if(EntireGameManager.Instance == null) { //for debug
			Application.LoadLevel("titleScene");
			return;
		}
		Instantiate(EntireGameManager.Instance.currentPlayer.playerHolder);
		MigrateGameInfo();
		
		health = initialHealth;
		spinPoint = maxSpinPoint;
		
		UpdateCamera();
		UpdateInputReader();
		spinPointBar = GameObject.Find("SpinPointBar");
		
		SoundEnabled = MyPrefs.SoundEnabled;
		bgm = (PlayableAudio) GetComponent("AudioChain");
		if(MyPrefs.MusicEnabled) bgm.StartAudio();
		
		settingsGUI.ResumeButtonEnabled = true;
		
		Instantiate(mode.field);
		
		RenderSettings.skybox.SetColor("_Tint", Color.Lerp(mode.bgColor, Color.white, .3f));
		//カメラの背景色用に作った色なのでskyboxではlerpする 
	}
	
	void OnDestroy() {
		RenderSettings.skybox.SetColor("_Tint", new Color(.5f, .5f, .5f, .5f));
	}
	
	private GameMode mode;
	private void MigrateGameInfo() {
		mode = EntireGameManager.Instance.CurrentGameMode;
		Time.timeScale = timeScale = mode.timeScale;
		initialHealth = mode.initialPlayerHealth;
	}
	
	void Update() {
		if(Input.GetButtonDown("Pause")) {
			SetPause(!Paused);
			showingSettings = false;
		}
		AudioListener.pause = Paused;
	}
	
	public void SetPause(bool pause) {
		Paused = pause;
		Time.timeScale = Paused ? 0 : timeScale;
	}
	
	void OnApplicationPause(bool pause) {
		if(pause) SetPause(true);
		AudioListener.pause = Paused;
	}
	
	private bool showingSettings = false;
	private bool gotHighscore = false;
	private GameObject spinPointBar;

	private Rect menuArea = new Rect (Screen.width/2 - 140, Screen.height/2 - 140, 280, 280);
	private Rect menuAreaInner = new Rect (Screen.width/2 - 130, Screen.height/2 - 100, 260, 230);
	
	void OnGUI() {
		var texture = spinPointBar.GetComponent<GUITexture>();
		texture.pixelInset = new Rect(texture.pixelInset.x, texture.pixelInset.y,
				spinPoint * maxSpinGUISizeRate, texture.pixelInset.height);
		
		if(Gameover) {
			Time.timeScale = 0;
			
			GUI.Box (menuArea, "Game Over");
			GUILayout.BeginArea (menuAreaInner);
		
			GUILayout.BeginHorizontal();
			GUILayout.Label("Your score: " + score);
			EntireGameManager.Instance.DrawStarRatingGUILayout(mode, score);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			if(gotHighscore || mode.Score < score) {
				gotHighscore = true;
				mode.Score = score;
				mode.ScoreSuffix = EntireGameManager.Instance.currentPlayer.highscoreSuffix;
				GUILayout.Label("You've got a highscore in " + mode.name + " mode! Well done :)");
			}
			else {
				GUILayout.Label("(Best score: " + mode.Score + ")");
			}
			if(GUILayout.Button (new GUIContent(tweetButtonIcon, "Tweet the score!"),
			                     GUIStyle.none, GUILayout.ExpandWidth(false))) {
				//プラグインあるからoauth使おうと思ったけど、面倒なのでやめた… 
				//webplayerならスコア改ざん防止のチェックサム(というかXorShift)付きでjsに投げて
				//ajaxからサーバーサイドからtwitterにアクセスするのが一番楽かな 
				var url = "https://twitter.com/share?text="
						+ WWW.EscapeURL(string.Format(tweetFormatString, mode.name, score,
					                              EntireGameManager.Instance.currentPlayer.name))
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
				Gameover = false;
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
		else if(Paused) {
			Time.timeScale = 0;
			
			GUI.Box (menuArea, showingSettings ? "Pause - Settings" : "Pause");
			GUILayout.BeginArea (menuAreaInner);
			
			if(!showingSettings) {
				if(GUILayout.Button ("Resume")) {
					Paused = false;
				}
				else if(GUILayout.Button ("Retry")) {
					if(mode.Score < score) mode.Score = score;
					Application.LoadLevel("playroom");
				}
				else if(GUILayout.Button ("Quit")) {
					if(mode.Score < score) mode.Score = score;
					Application.LoadLevel("titleScene");
				}
				else if(GUILayout.Button ("Settings")) {
					showingSettings = true;
				}
			}
			else {
				settingsGUI.InitIfNeeded();
				SettingsGUIFragment.MenuState state = settingsGUI.Draw(menuAreaInner);
				//C#のswitch文むずすぎ 
				if(state == SettingsGUIFragment.MenuState.CHANGED) {
					if((settingsGUI.ChangeBits & SettingsGUIFragment.CameraChanged) != 0) {
						UpdateCamera();
					}
					UpdateInputReader();
				}
				else if(state == SettingsGUIFragment.MenuState.RESUME
				        || state == SettingsGUIFragment.MenuState.BACK) {
					Paused = state == SettingsGUIFragment.MenuState.BACK;
					showingSettings = false;
					settingsGUI.End();
					SoundEnabled = settingsGUI.NewSound;
					if(settingsGUI.InitialMusic != settingsGUI.NewMusic) {
						if(settingsGUI.NewMusic) bgm.StartAudio();
						else {
							bgm.StopAudio();
						}
					}
					
				}
			}
		
			GUILayout.EndArea();
			Time.timeScale = Paused ? 0 : timeScale;
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
		
		// ポーズボタン
		if (GUILayout.Button("P")) {
			SetPause(!Paused);
		}

		InputReader.OnGUI();
	}

	public void OnGameover() {
		Gameover = true;
	}
	
	//EntireGameManagerにも同じ関数があるけど、こちらの方が速い
	public void PlaySE(AudioSource source) {
		if(SoundEnabled) source.Play();
	}
}