using System;
using UnityEngine;

public abstract class InputReader
{
	
	private static InputReader _instance;
	public static InputReader Instance {
		get {
			if(_instance == null) {
				_instance = EntireGameManager.Instance.IsMobile
					? (InputReader) new PhysicalInputReader()
					: (InputReader) new KeyboardInputReader();
			}
			return _instance;
		}
	}
	
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
}

class PhysicalInputReader : InputReader {
	
	InputReader proxy = new KeyboardInputReader();
	
	public override Vector3 Direction {
		get {
			var dir = proxy.Direction;
			if (dir != Vector3.zero) return dir;
			
			// code from: http://unity3d.com/support/documentation/ScriptReference/Input-acceleration.html
			
			// we assume that device is held parallel to the ground
    		// and Home button is in the right hand
			
			// remap device acceleration axis to game coordinates:
			//  1) XY plane of the device is mapped onto XZ plane
			//  2) rotated 90 degrees around Y axis
			dir = new Vector3(-Input.acceleration.y, 0, Input.acceleration.x);
			
			dir *= 2.5f; // 増幅 
			
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
			return proxy.IsTryingToSpin || Input.touches.Length > 0;
		}
	}
	
	public override bool IsTryingToJump {
		get {
			// 上方向の加速度を感じたらジャンプしたことに 
			return proxy.IsTryingToJump || Input.acceleration.z > 0.075f;
		}
	}
}
