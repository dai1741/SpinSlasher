using System;
using UnityEngine;

public abstract class InputReader
{
	
	private static InputReader _instance;
	public static InputReader Instance {
		get {
			if(_instance == null) {
				_instance = new KeyboardInputReader();
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

	public override Vector3 Direction {
		get {
			return Vector3.zero;
		}
	}
	
	public override bool IsTryingToSpin {
		get {
			return false;
		}
	}
	
	public override bool IsTryingToJump {
		get {
			return false;
		}
	}
}
