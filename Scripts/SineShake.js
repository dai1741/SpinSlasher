#pragma strict

var anglarVelocityDeg = 3.0;
var initialAngleDeg = 0.0;
var radius = 10.0;
var direction : Vector3 = Vector3.forward;
var directionIsLocal = false;

var playOnAwake = true;

private var angle : float;
private var anglarVelocity : float;
private var playing : boolean;
private var remainedDuration : float;
private var stopAngle : float = Mathf.Infinity;
private static final var epsAngle = Mathf.PI / 20;

function Start() {
	angle = CropRadians(initialAngleDeg * Mathf.Deg2Rad);
	anglarVelocity = CropRadians(anglarVelocityDeg * Mathf.Deg2Rad);
	if(directionIsLocal) direction = transform.rotation * direction;
	direction = direction.normalized;
	if(playOnAwake) Play();
}

function FixedUpdate () {
	if(!playing) return;
	if(remainedDuration <= 0 || Mathf.Abs(angle - stopAngle) < epsAngle
			|| Mathf.PI * 2 - angle + stopAngle < epsAngle) Stop();
	var distance = Mathf.Cos(angle) * radius * Time.deltaTime * anglarVelocity;
	transform.position += direction * distance;
	angle = CropRadians(angle + anglarVelocity * Time.deltaTime);
	remainedDuration -= Time.deltaTime;
}

function Play() {
	Play(Mathf.Infinity);
}
function Play(duration : float) {
	remainedDuration = duration;
	playing = true;
}

function PlayAndStopAtAngle(stopAngleDeg : float) {
	stopAngle = CropRadians(stopAngleDeg * Mathf.Deg2Rad);
	Play();
}

function Stop() {
	stopAngle = Mathf.Infinity;
	playing = false;
}

static function CropRadians(rad : float) {
	return (rad + Mathf.PI * 2) % (Mathf.PI * 2);
}