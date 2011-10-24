
var destroyTime : float;
var xFactor : float;

function Start() {
	Destroy(gameObject, destroyTime);
	var look = transform.position - Camera.main.transform.position;
	look.x *= xFactor;
	transform.rotation = Quaternion.LookRotation(look);
}