var deleteAudioSource : AudioSource;
var completeStar : Texture2D;
private var availableCharas : int[];
private var characterNames : String[];


function Start() {
	Time.timeScale = 1;
	guiState = EntireGameManager.Instance.CurrentGameMode != null
			? GUIState.selectingGameModes
			: GUIState.normal;
	EntireGameManager.Instance.UnlockIfAny();
	SupplyNewUnlocked();
	
	//Linqつかいたい 
	var availableCharasTemp = new Array();
	var characterNamesTemp = new Array();
	var characters : PlayerInfo[] = EntireGameManager.Instance.characters;
	for(var i = 0; i < characters.Length; i++) {
		if(characters[i].Unlocked) {
			availableCharasTemp.Push(i);
			characterNamesTemp.Push(characters[i].name);
		}
	}
	availableCharas = availableCharasTemp.ToBuiltin(int);
	characterNames = characterNamesTemp.ToBuiltin(String);
	
	charaIndex = System.Array.IndexOf(EntireGameManager.Instance.characters, EntireGameManager.Instance.currentPlayer);
}

enum GUIState { normal, selectingGameModes, watchingRecords, deletingRecords,
		settings, modeUnlocked, charaUnlocked }
private var guiState = GUIState.normal;
private var settingsGUI : SettingsGUIFragment = new SettingsGUIFragment();

private var newUnlockedMode : GameMode;
private var newUnlockedChara : PlayerInfo;

private function SupplyNewUnlocked() {
	if(EntireGameManager.Instance.modeUnlockNotificationQueue.Count > 0) {
		newUnlockedMode = EntireGameManager.Instance.modeUnlockNotificationQueue.Dequeue();
		guiState = GUIState.modeUnlocked;
	}
	else if(EntireGameManager.Instance.charaUnlockNotificationQueue.Count > 0) {
		newUnlockedChara = EntireGameManager.Instance.charaUnlockNotificationQueue.Dequeue();
		guiState = GUIState.charaUnlocked;
	}
}

private var backButtonPressed : boolean = false;

function OnGUI() {

	// とても不安定 
	if (!EntireGameManager.Instance.IsMobile && Event.current.type == EventType.Repaint) {
		backButtonPressed = Input.GetButton("Back");
	}

	var width = Screen.width <= 600 ? Screen.width/9 * 4 : Screen.width/3; //なんとか押し込む
	var area = new Rect (Screen.width/2 - width/2, Screen.height/2 - 5, width, Screen.height/2 + 5);
	GUILayout.BeginArea (area);
	GUILayout.FlexibleSpace();
	
	if(guiState == GUIState.normal) SupplyNewUnlocked();
	
	switch(guiState) {
	case GUIState.modeUnlocked:
		WriteLabelInCenter("New Mode Unlocked");
		
		var neededMode = EntireGameManager.Instance.gameModes[newUnlockedMode.unlockModeIndex];
		GUILayout.Label("You scored " + neededMode.Score
				+ " pts in " + neededMode.name + " mode and\n"
				+ newUnlockedMode.name + " mode has been unlocked!");
		GUILayout.FlexibleSpace();
		if(GUILayout.Button ("Dismiss"/*, GUILayout.MaxWidth(area.width / 5)*/)) {
			EntireGameManager.Instance.PlayClickSound();
			newUnlockedMode = null;
			guiState = GUIState.normal;
		}
		break;
		
	case GUIState.charaUnlocked:
		WriteLabelInCenter("New Character Unlocked");
		//TODO: 中小化 
		GUILayout.Label("You got " + newUnlockedChara.requiredStars
				+ " stars on all modes and\n"
				+ newUnlockedChara.longName + " has been unlocked!");
		GUILayout.FlexibleSpace();
		if(GUILayout.Button ("Dismiss")) {
			EntireGameManager.Instance.PlayClickSound();
			newUnlockedChara = null;
			guiState = GUIState.normal;
		}
		break;
		
	case GUIState.normal:
		if(GUILayout.Button ("Start Game", GUILayout.MinHeight(Screen.height/8))) {
			EntireGameManager.Instance.PlayClickSound();
			guiState = GUIState.selectingGameModes;
		}
		GUILayout.FlexibleSpace();
		if(GUILayout.Button ("Instructions")) {
			EntireGameManager.Instance.PlayClickSound();
			Application.LoadLevel("instruction");
		}
		GUILayout.BeginHorizontal();
		if(GUILayout.Button ("Highscores")) {
			EntireGameManager.Instance.PlayClickSound();
			guiState = GUIState.watchingRecords;
		}
		if(GUILayout.Button ("Options")) {
			EntireGameManager.Instance.PlayClickSound();
			guiState = GUIState.settings;
		}
		GUILayout.EndHorizontal();
		
		if(!Application.isWebPlayer) {
			GUILayout.FlexibleSpace();
			if(GUILayout.Button ("Quit Game") || (EntireGameManager.Instance.IsMobile && Input.GetButton("Back"))) {
				Application.Quit();
			}
		}
		break;
		
	case GUIState.selectingGameModes:
		WriteLabelInCenter("Select Game Mode");
		var gameModes : GameMode[] = EntireGameManager.Instance.gameModes;
		for(var mode in gameModes) {
			if(mode.Unlocked) {
				if(GUILayout.Button (mode.name/*, GUILayout.Height(Screen.height/8)*/)) {
					EntireGameManager.Instance.PlayClickSound();
					EntireGameManager.Instance.CurrentGameMode = mode;
					Application.LoadLevel("playroom");
				}
			}
			else {
				GUI.enabled = false;
				neededMode = gameModes[mode.unlockModeIndex];
				GUILayout.Button (neededMode.Unlocked
						? "Get " + mode.unlockScore + " pts in " + neededMode.name
						: "Locked");
				GUI.enabled = true;
			}
		}
		GUILayout.FlexibleSpace();
		if(GUILayout.Button ("Back to Main Menu") || backButtonPressed) {
			guiState = GUIState.normal;
			EntireGameManager.Instance.CurrentGameMode = null;
		}
		break;
		
	case GUIState.watchingRecords:
		WriteLabelInCenter("Highscore");
		for(var mode in EntireGameManager.Instance.gameModes) {
			GUILayout.BeginHorizontal();
			GUILayout.Label(mode.Unlocked ? mode.name : "???", GUILayout.Width(area.width / 9 * 4));
			if(mode.Score >= 0) {
				GUILayout.Label(mode.Score + " " + mode.ScoreSuffix, GUILayout.MinWidth(50)); //数字5個分+initials
				EntireGameManager.Instance.DrawStarRatingGUILayout(mode, mode.Score);
			}
			else GUILayout.Label("Not played");
			GUILayout.EndHorizontal();
		}
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		if(GUILayout.Button ("Delete All")) {
			EntireGameManager.Instance.PlayClickSound();
			guiState = GUIState.deletingRecords;
		}
		if(GUILayout.Button ("Back to Main Menu", GUILayout.MinWidth(area.width / 2 + 20)) || backButtonPressed) {
			guiState = GUIState.normal;
		}
		GUILayout.EndHorizontal();
		break;
		
	case GUIState.deletingRecords:
		GUILayout.Label("Are you sure you want to delete all highscores?\n\n"
				+ "Cautions:\n"
				+ " - This cannot be undone!\n"
				+ " - Unlocked games will be locked again!");
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		if(GUILayout.Button ("Yes", GUILayout.MaxWidth(Mathf.Min(area.width / 5, 50)))) {
			for(var mode in EntireGameManager.Instance.gameModes) {
				mode.Score = -1;
				mode.Unlocked = false;
			}
			EntireGameManager.Instance.gameModes[0].Unlocked = true;
			EntireGameManager.PlaySE(deleteAudioSource);
			guiState = GUIState.watchingRecords;
		}
		if(GUILayout.Button ("No") || GUILayout.Button ("Cancel") || backButtonPressed) {
			guiState = GUIState.watchingRecords;
		}
		
		GUILayout.EndHorizontal();
		break;
		
	case GUIState.settings:
		WriteLabelInCenter("Options");
		settingsGUI.InitIfNeeded();
		var state = settingsGUI.Draw();
		if(state == SettingsGUIFragment.MenuState.BACK || backButtonPressed) {
			settingsGUI.End();
			guiState = GUIState.normal;
		}
		break;
	}
	GUILayout.FlexibleSpace();
	GUILayout.EndArea();
	
	if(availableCharas.length > 1) {
		area = new Rect(Screen.width - 200, 0, 200, 30);
		
		GUILayout.BeginArea (area);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Chara: "); //テクスチャのがよさげ 
		charaIndex = GUILayout.SelectionGrid(charaIndex, characterNames, 3);
		EntireGameManager.Instance.currentPlayer = EntireGameManager.Instance.characters[charaIndex];
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	if(EntireGameManager.Instance.NumMinStars >= GameMode.RATING_SCORES_COUNT) {
		GUI.Label(Rect(0, 0, 20, 19), completeStar);
	}
}
private var charaIndex = 0;

private function WriteLabelInCenter(str : String) {
	var orig = GUI.skin.label.alignment;
	var origColor = GUI.color;
	GUI.skin.label.alignment = TextAnchor.MiddleCenter;
	GUI.color = new Color(1, .9, .6, 1);
	GUILayout.Label(str);
	GUI.color = origColor;
	GUI.skin.label.alignment = orig;
}