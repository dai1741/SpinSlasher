using System;
using UnityEngine;

public abstract class InputReader
{
	/// <summary>
	/// 移動量を取得する。ベクトルのノルムは最大1
	/// </summary>
	public abstract Vector3 Direction {
		get;
	}
	
	public abstract bool IsTryingToSpin {
		get;
	}
	
	public abstract bool IsTryingToJump {
		get;
	}
	
	public abstract void OnGUI();
}

	
class KeyboardInputReader : InputReader {

	public override Vector3 Direction {
		get {
			var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			var bigger = Mathf.Max(Mathf.Abs(moveDirection.x), Mathf.Abs(moveDirection.z));
			if(bigger > 0) {
				//ななめ移動が速くなるのを補正 
				moveDirection /= (moveDirection / bigger).magnitude;
			}
			return moveDirection;
		}
	}
	
	public override bool IsTryingToSpin {
		get {
			return Input.GetButton("Spin");
		}
	}
	
	public override bool IsTryingToJump {
		get {
			return Input.GetButtonDown("Jump") || Input.GetButton("Jump");
		}
	}
	
	public override void OnGUI() {
	}
}

class PhysicalInputReader : InputReader {
	
	public override Vector3 Direction {
		get {
			// code from: http://unity3d.com/support/documentation/ScriptReference/Input-acceleration.html
			
			// we assume that device is held parallel to the ground
    		// and Home button is in the right hand
			
			// remap device acceleration axis to game coordinates:
			//  1) XY plane of the device is mapped onto XZ plane
			//  2) rotated 90 degrees around Y axis
			var dir = new Vector3(-Input.acceleration.y, 0, Input.acceleration.x);
			
			dir *= 2.75f; // 増幅 
			
			// clamp acceleration vector to unit sphere
			if (dir.sqrMagnitude > 1)
				dir.Normalize();
			// ごく小さいのは無視で 
			else if (dir.sqrMagnitude < 0.02f)
				dir = Vector3.zero;
			
			return dir;
			
		}
	}
	
	public override bool IsTryingToSpin {
		get {
			// 画面をタップしていたらスピン 
			return Input.touchCount - GetJumpCount() > 0;
		}
	}
	
	public override bool IsTryingToJump {
		get {
			// ジャンプボタン押下時か上方向の加速度を感じたらジャンプしたことに 
			return GetJumpCount() > 0 || Input.acceleration.z > 0;
		}
	}
	
	public override void OnGUI() {
		// 色々ひどい 
		GUI.Box (JumpButtonField, "");
		var orig = GUI.skin.label.alignment;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.Label (JumpButtonField, "Jump");
		GUI.skin.label.alignment = orig;
	}
	
	private Rect JumpButtonField {
		get {
			float w = Screen.width / 5;
			float h = Screen.height / 4;
			// 縦幅-20はスコア表示の分 
			return new Rect(Screen.width - w, Screen.height - h - 20, w, h);
		}
	}
	
	private int GetJumpCount() {
		int count = 0;
		var r = JumpButtonField;
		foreach (Touch touch in Input.touches) {
			var pos = touch.position;
			pos.y = Screen.height - pos.y; // なぜか反転している!!!? 
			if (r.Contains(pos)) count++;
		}
		return count;
	}
}
