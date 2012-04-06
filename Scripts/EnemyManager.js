var creatures : GameObject[];
var appearOnceIn : float[];
var initialSpeed = 1.0;
private var initialSpeedRate = 0.0;
var fieldRadius = 1.0;
var minSpeedIncrease = 0.0;
var maxSpeedIncrease = 0.0;
var creationInterval = 1.0;
var creationIntervalDecrease = 0.0;
var minCreationInterval = 1.0;

var firstPlayWait = 1.0;
var firstPlayDescs : GameObject;

public var minSpeed : float = 1;
public var maxSpeed : float = 1;

@System.NonSerialized
public var sqrMinSpeed : float;
@System.NonSerialized
public var sqrMaxSpeed : float;

private var lastTime : float;
private var creationCount : int = 0;

private var isFirstPlay : boolean;

private static var instance : EnemyManager;
public static function GetInstance() : EnemyManager {
	return instance;
}

function Awake() {
	instance = this;
}

function Start() {
	lastTime = Time.time;
	if(EntireGameManager.Instance != null) {
		MigrateGameInfo();
		SendMessage(isFirstPlay ? "CreateEnemiesWithDesc" : "CreateEnemies");
	}
}

private function MigrateGameInfo() {
	var mode = EntireGameManager.Instance.CurrentGameMode;
	
	//力技 
	creatures = mode.creatures;
	appearOnceIn = mode.appearOnceIn;
	initialSpeed = mode.initialSpeed;
	initialSpeedRate = mode.initialSpeedRate;
	minSpeedIncrease = mode.minSpeedIncrease;
	maxSpeedIncrease = mode.maxSpeedIncrease;
	creationInterval = mode.creationInterval;
	creationIntervalDecrease = mode.creationIntervalDecrease;
	minCreationInterval = mode.minCreationInterval;
	minSpeed = mode.minSpeed;
	maxSpeed = mode.maxSpeed;
	
	isFirstPlay = mode.internalName == "Normal" && mode.Score <= 0;
}

private function CreateEnemies() {
	while(true) {
		yield WaitForSeconds(creationInterval);
		
		creationCount++;
		var creature : GameObject = creatures[0];
		for(var i = 1; i < creatures.length; i++) {
			if(creationCount % appearOnceIn[i-1] == 0) creature = creatures[i];
		}
		
		CreateEnemy(creature);
		
		var timeDiff = Time.time - lastTime;
		lastTime = Time.time;
		minSpeed += timeDiff * minSpeedIncrease;
		maxSpeed += timeDiff * maxSpeedIncrease;
		sqrMinSpeed = minSpeed * minSpeed;
		sqrMaxSpeed = maxSpeed * maxSpeed;
		
		if(creationInterval > minCreationInterval)
			creationInterval -= timeDiff * creationIntervalDecrease;
		else
			creationInterval = minCreationInterval;
	}
}

protected function CreateEnemiesWithDesc() {
	Instantiate(firstPlayDescs);
	yield WaitForSeconds(firstPlayWait);
	CreateEnemies();
}

protected function CreateEnemy(enemy : GameObject) {
	var angle = Random.Range(0, Mathf.PI*2);
	var randomVector =  new Vector3(fieldRadius * Mathf.Sin(angle), 0.9, fieldRadius * Mathf.Cos(angle));
	var direction = angle - Mathf.PI + Random.Range(Mathf.PI, Mathf.PI) / 10;
	var velocity = new Vector3(Mathf.Sin(direction), 0, Mathf.Cos(direction)) * Mathf.Max(initialSpeed, minSpeed * initialSpeedRate);
	
	var newCreature : GameObject = Instantiate(enemy, randomVector, Quaternion.identity);
	newCreature.rigidbody.velocity = velocity;
	
}
