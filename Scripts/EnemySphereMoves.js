#pragma strict

public class EnemySphereMoves extends EnemyMoves {
	var destroyDist = Mathf.Infinity;
	private static var sqrDestroyDist : float;
	
	function Start() {
		super();
		sqrDestroyDist = destroyDist*destroyDist;
	}
	function Update () {
		if(transform.position.sqrMagnitude > sqrDestroyDist) Destroy(gameObject);
	}
}