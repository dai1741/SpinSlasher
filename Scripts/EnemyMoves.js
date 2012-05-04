#pragma strict

public class EnemyMoves extends MonoBehaviour {

	var basePoint : int;
	var incresePoint : int; //変数名誤字ったけどリファクタリングの仕方がわからない… 
	var canDieByDistance = false;
	
	function OnCollisionEnter(collisionInfo : Collision) {
		var other = collisionInfo.collider;
		if(other.tag.StartsWith("Player")) {
			var script : PlayerMoves = (other.tag == "PlayerPart"
				? GameObject.FindWithTag("Player").GetComponent.<PlayerMoves>()
				: other.GetComponent.<PlayerMoves>());
			script.OnCollideEnemy(gameObject);
		}
	}
	
	@System.NonSerialized
	public var isDead : boolean = false;
	
	protected static var enemyManager: EnemyManager;
	
	function Start () {
		enemyManager = EnemyManager.GetInstance();
	}
	
	function OnCollisionExit(collisionInfo : Collision) {
		var sqrMagnitude = rigidbody.velocity.sqrMagnitude;
		if(sqrMagnitude < enemyManager.sqrMinSpeed) {
			rigidbody.velocity = rigidbody.velocity.normalized * enemyManager.minSpeed;
		}
		else if(sqrMagnitude > enemyManager.sqrMaxSpeed) {
			rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, enemyManager.maxSpeed);
		}
	}
	
	public function GetPoint(combo : int): int {
		return Mathf.Min(basePoint + incresePoint * combo, 300);
	}

}