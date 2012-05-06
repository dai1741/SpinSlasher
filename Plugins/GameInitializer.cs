using UnityEngine;

public class GameInitializer : MonoBehaviour {
	private bool guiInitialized = false;
	
	void Start() {
		Destroy (this, 5);
	}
	
	void OnGUI() {
		if (guiInitialized) return;
		
		guiInitialized = true;
		
		// なぜか相対指定するとバグる
		// GUI.skin.button.padding.top *= 2;
		// GUI.skin.button.padding.bottom *= 2;
		
		GUI.skin.button.padding.top = 5;
		GUI.skin.button.padding.bottom = 5;
		Debug.Log(GUI.skin.button.padding);
	}
}

