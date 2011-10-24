
private var showingAbout = false;

function OnGUI () {
	var area : Rect = new Rect(Screen.width - 220, Screen.height - 30, 220, 30);
	
	GUILayout.BeginArea (area);
	
	GUILayout.BeginHorizontal();
	if(GUILayout.Button (showingAbout ? "Instructions" : "Credits", GUILayout.Height(30))) {
		GetComponent.<SineShake>().PlayAndStopAtAngle(showingAbout ? -90 : 90);
		showingAbout = !showingAbout;
	}
	if(GUILayout.Button ("Back", GUILayout.Height(30))) {
		Application.LoadLevel("titleScene");
	}
	GUILayout.EndHorizontal();
	GUILayout.EndArea();
}