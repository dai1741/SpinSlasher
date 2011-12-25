
var speed = 6.0;
var jumpSpeed = 1.0;
var jumpIntervalSecs = 0.5;
var keepableJumpTime = 0.0;
var rotateSpeed = 10.0;
var rotateDamp = 20.0;
var gravity = 20.0;
var accelerate = 3.0;
var brake = 3.0;
//var aerialMoveFactor = 0.3; //broken
//var baseTransForm : Transform;

var detonator : GameObject;
var damagedDetonator : GameObject;
var extremeDetonator : GameObject;
private static final var explosionLife : float = 6.5;

var pointDisplayer : GameObject;
var mutekiTime = 1.0;
var invisibleMutekiTime = 0.1;

var spinEmitter : ParticleEmitter; //少ないときは変える
var spinAudioSource : AudioSource;
var spinningXScale = 1.0;
var spinningMinYSpeed = Mathf.NegativeInfinity;
var fallDownAudioSource : AudioSource;
var killedAudio : RandomAudioPlayer;

var mutekiMaterial : Material;
var mutekiRendererHolderName : String;
var mutekiMaterialIndex = 0;

var deadY = -100;

private var meshTransform : Transform;

private var controller : CharacterController;
private var isAerial : boolean = false; //変数名おかし 
private var moveDirection : Vector3 = Vector3.zero;
private var curRotateSpeed : float = 0;

private var isSpinning : boolean = false;
private var secsToJump : float = 0;

private var origMaterial : Material;

function Start()
{
	Time.timeScale = GameManager.Instance.timeScale;
	controller = GetComponent(CharacterController);
	playerParts = GameObject.FindGameObjectsWithTag("PlayerPart");
	for(var part in playerParts) part.active = false;
	
	
	for(var child : Transform in transform.Find("Mesh")) {
		meshTransform = child; break;
	}
	origMaterial = GetMutekiRenderer().materials[mutekiMaterialIndex];
	
	GameManager.Instance.SpinPointNeededToRespin = spinPointNeededToRespin;
}

private var playerParts : GameObject[];
private var remainedMutekiTime : float = 0;
private var remaindKeepableJumpTime : float = 0;
private var jumpButtonClicked : boolean;

function Update() 
{
	if(Input.GetButtonDown("Jump") || Input.GetButton("Jump")) jumpButtonClicked = true;
}
function FixedUpdate() 
{
	if(GameManager.Instance == null) return; // has to wait for his awake.
	if(transform.position.y < deadY) {
		OnGameover();
		return;
	}
	if(Input.GetButton("Spin")
			&& GameManager.Instance.spinPoint > (isSpinning ? 0 : GameManager.Instance.SpinPointNeededToRespin)) {
		if(!isSpinning) {
			isSpinning = true;
			combo = 0;
			lastSpinStartTime = Time.time;
			curRotateSpeed = rotateSpeed;
			if(spinEmitter != null) spinEmitter.emit = true;
			for(var part in playerParts) part.active = true;
			meshTransform.localScale.x = spinningXScale;
			GameManager.Instance.PlaySE(spinAudioSource);
		}
	}
	else {
		curRotateSpeed = Mathf.Max(curRotateSpeed - rotateDamp * Time.deltaTime, 0);
		if(isSpinning) {
			isSpinning = false;
			//combo = 0;
			if(spinEmitter != null) spinEmitter.emit = false;
			for(var part in playerParts) part.active = false;
			meshTransform.localScale.x = 1.0;
			spinAudioSource.Stop();
		}
	}
	if(isSpinning || Time.time - lastSpinStartTime < spinPointComsumeMinTime) {
		if(remainedMutekiTime < invisibleMutekiTime) GameManager.Instance.spinPoint -= 
				Mathf.Min(spinPointComsumePerSec * Time.deltaTime
						* (isAerial ? spinPointAerialComsumeRate : 1),
						GameManager.Instance.spinPoint);
		transform.Rotate(Vector3.up * Time.deltaTime * curRotateSpeed);
	}
	else {
		GameManager.Instance.spinPoint = Mathf.Min(GameManager.Instance.spinPoint + spinPointRepairPerSec
				* (isAerial ? spinPointAerialRepairRate : 1) * Time.deltaTime,
				GameManager.Instance.maxSpinPoint);
	}
	
	if(remainedMutekiTime > 0) {
		remainedMutekiTime = Mathf.Max(remainedMutekiTime - Time.deltaTime, 0);
	}
	
	var oldDirection = moveDirection;
	var oldFlat = oldDirection; oldFlat.y = 0;
	moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	var bigger = Mathf.Max(Mathf.Abs(moveDirection.x), Mathf.Abs(moveDirection.z));
	if(bigger > 0) {
		//ななめ移動が速くなるのを補正 
		moveDirection /= (moveDirection / bigger).magnitude;
	}
	//正面固定で.
	//moveDirection = baseTransForm.TransformDirection(moveDirection);
	//var moveFactor = isAerial ? aerialMoveFactor : 1.0;
	moveDirection *= speed /* * moveFactor */;
	if(moveDirection != oldFlat) {
		moveDirection = Vector3.MoveTowards(oldFlat, moveDirection,
				(moveDirection == Vector3.zero || 
						Vector3.Dot(oldFlat, moveDirection) < 0 ? brake : accelerate)
						 * Time.deltaTime);
	}
	
	if(!IsSpinningSticky()) {
		if(moveDirection != Vector3.zero) transform.rotation = Quaternion.LookRotation(moveDirection);
	}
	
	//ここからジャンプ処理。カオス 
	jumpButtonClicked = jumpButtonClicked || Input.GetButton("Jump");
	
	if(jumpButtonClicked && !isAerial && secsToJump <= 0) {
		moveDirection.y = jumpSpeed;
		secsToJump = jumpIntervalSecs;
		remaindKeepableJumpTime = keepableJumpTime;
	}
	else if(isAerial) moveDirection.y = oldDirection.y;
	
	if(!jumpButtonClicked) remaindKeepableJumpTime = 0;
	else if(isAerial && 0 < remaindKeepableJumpTime) {
		remaindKeepableJumpTime = Mathf.Max(remaindKeepableJumpTime - Time.deltaTime, 0);
		if(remaindKeepableJumpTime == 0) moveDirection.y += gravity * keepableJumpTime / 2;
	}
	
	if(isAerial || secsToJump <= 0) jumpButtonClicked = false;
	else secsToJump -= Time.deltaTime;
	
	moveDirection.y -= gravity * Time.deltaTime;
	if(isSpinning && moveDirection.y < spinningMinYSpeed) moveDirection.y = spinningMinYSpeed;
	
	var flagBits = controller.Move(moveDirection * Time.deltaTime);
	var lastAerial = isAerial;
	isAerial = (flagBits & CollisionFlags.CollidedBelow) == 0;
	
	if(lastAerial && !isAerial && secsToJump > 0) {
		GameManager.Instance.PlaySE(fallDownAudioSource);
	}
}

var spinPointRepairPerSec = 2.5;
var spinPointComsumePerSec = 4.0;
var spinPointAerialRepairRate = 0.3;
var spinPointAerialComsumeRate = 0.8;
var spinPointComsumeMinTime = 0.3;
var spinPointNeededToRespin = 3.0;
private var lastSpinStartTime = -10.0;

public function IsSpinningSticky() {
	return curRotateSpeed > 0;
}


function OnControllerColliderHit (hit : ControllerColliderHit) {
	var other = hit.gameObject;
	if(other.tag == "Enemy" && (IsSpinningSticky() || hit.normal.y <= 0)) {
		OnCollideEnemy(other);
	}
}

public function OnCollideEnemy(enemy : GameObject) : void {
	var enemyMoves = enemy.GetComponent.<EnemyMoves>();
	if(enemyMoves.isDead) {
		return;
	}
	enemyMoves.isDead = true;
		
	var explosionPrefab : GameObject;
	var soundEnabled = GameManager.Instance.SoundEnabled;
	if(!IsSpinningSticky()) {
		if(remainedMutekiTime == 0) {
			GameManager.Instance.health--;
			if(GameManager.Instance.health <= 0) {
				OnGameover();
			}
			else {
				remainedMutekiTime = mutekiTime + invisibleMutekiTime;
				SendMessage("BlinkPlayer");
			}
		}
		else {
			soundEnabled = false;
		}
		explosionPrefab = damagedDetonator;
	}
	else {
		var gainPoint = enemyMoves.GetPoint(combo++);
		GameManager.Instance.score += gainPoint;
		var pointDisplayerObj : GameObject = Instantiate (pointDisplayer);
		pointDisplayerObj.transform.position = enemy.transform.position + Vector3.up;
		pointDisplayerObj.GetComponent.<PointDisplayer>().point = gainPoint;
		explosionPrefab = combo >= 5 ? extremeDetonator : detonator;
	}
	if(explosionPrefab != null) {
		var explosion : GameObject = Instantiate (explosionPrefab,
				enemy.transform.position, Quaternion.identity);
		/*explosion.GetComponent("Detonator").detail = 0.5;
		explosion.GetComponent("DetonatorSound").maxVolume = 
				soundEnabled ? 1 : 0;*/
		Destroy(explosion, explosionLife); 
	}
	
	Destroy (enemy);
}

private function GetMutekiRenderer(): Renderer {
	return (mutekiRendererHolderName != ""
			? meshTransform.Find(mutekiRendererHolderName)
			: meshTransform)
			.GetComponent.<Renderer>();
}

private function BlinkPlayer() {
	var mesh : Renderer = GetMutekiRenderer();
	var materials : Material[] = mesh.materials;
	do {
		materials[mutekiMaterialIndex] = mutekiMaterial;
		mesh.materials = materials;
		yield WaitForSeconds(0.1);
		materials[mutekiMaterialIndex] = origMaterial;
		mesh.materials = materials;
		yield WaitForSeconds(0.1);
	} while(remainedMutekiTime > invisibleMutekiTime);
}

private var combo: int = 0;

private function OnGameover() {
	if(!GameManager.Instance.Gameover) {
		GameManager.Instance.GetComponent.<AudioChain>().StopAudio();
		killedAudio.Play();
		
		GameManager.Instance.OnGameover();
	}
}


@script RequireComponent(CharacterController)