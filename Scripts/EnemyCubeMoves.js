#pragma strict

public class EnemyCubeMoves extends EnemyMoves {
	var maxRotationDegreesDelta : float;
	var maxVelocityDegreesDelta : float; //per speed
	var aimingOffset : Vector3;
	private var myTransform : Transform;
	private var entity : Transform;
	private var chasee : Transform;
	private var maxVelocityRadiansDelta : float;
	
	function Start() {
		super();
		myTransform = transform;
		entity = myTransform.Find("CubeWrapper");
		chasee = GameObject.FindWithTag("Player").transform; //playerは1人と仮定 
		maxVelocityRadiansDelta = maxVelocityDegreesDelta * Mathf.Deg2Rad;
	}
	function FixedUpdate () {
		var diff = chasee.position - myTransform.position + aimingOffset;
		entity.rotation = Quaternion.RotateTowards(entity.rotation,
				Quaternion.LookRotation(diff), maxRotationDegreesDelta * Time.deltaTime);
		rigidbody.velocity = Vector3.RotateTowards(rigidbody.velocity,
				diff.normalized * enemyManager.minSpeed,
				maxVelocityRadiansDelta * enemyManager.minSpeed * Time.deltaTime, 1);
	}
}