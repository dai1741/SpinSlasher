#pragma strict

public class EnemySphereMoves extends EnemyMoves {
	var destroyDist = Mathf.Infinity;
	private static var sqrDestroyDist : float;
	
	function Start() {
		super();
		sqrDestroyDist = destroyDist*destroyDist;
		
		// 動作速度向上のためライトを消す
		// 割と劇的に速くなる 
		if (EntireGameManager.Instance.IsMobile) {
			transform.GetComponentInChildren.<Light>().enabled = false;
		}
	}
	function Update () {
		if(transform.position.sqrMagnitude > sqrDestroyDist) Destroy(gameObject);
	}
}