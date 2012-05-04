#pragma strict

public class EnemySphereMoves extends EnemyMoves {
	
	function Start() {
		super();
		
		// 動作速度向上のためライトを消す
		// 割と劇的に速くなる 
		if (EntireGameManager.Instance.IsMobile) {
			transform.GetComponentInChildren.<Light>().enabled = false;
		}
	}
}