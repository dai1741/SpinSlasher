#pragma strict

public class EnemySphereMoves extends EnemyMoves {
	var destroyDist = Mathf.Infinity;
	private static var sqrDestroyDist : float;
	private var myTransform : Transform;
	
	function Start() {
		super();
		myTransform = transform;
		sqrDestroyDist = destroyDist*destroyDist;
		
		// 動作速度向上のためライトを消す
		// 割と劇的に速くなる 
		if (EntireGameManager.Instance.IsMobile) {
			myTransform.GetComponentInChildren.<Light>().enabled = false;
		}
	}
	function Update () {
		if(myTransform.position.sqrMagnitude > sqrDestroyDist) Destroy(gameObject);
	}
}