#pragma strict

public class SpinBehaviour extends MonoBehaviour {

	var direction : Vector3 = Vector3.forward;
	var directionIsLocal : boolean = false;
	
	function Start() {
		if(directionIsLocal) direction = transform.rotation * direction;
		direction = direction.normalized;
	}
	
	var rotateSpeed = 100.0;
	
	function Update () {
		transform.Rotate(direction * Time.deltaTime * rotateSpeed);
	}
}