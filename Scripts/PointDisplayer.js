
public class PointDisplayer extends TransientCameraLooker {
	
	var upSpeed : float;
	
	@System.NonSerialized
	public var point : int; // supplied somewhere
	
	var destroyTimeIncrese : float;
	
	var baseSize : float;
	var sizeIncrese : float;
	var maxSize : float;
	
	var materials : Material[];
	var thresholds : int[]; // materials.length == thresholds.length - 1
	
	function Start() {
		destroyTime += point * destroyTimeIncrese;
		
		var mesh : TextMesh = GetComponent.<TextMesh>();
		mesh.text = point.ToString();
		mesh.characterSize = baseSize + point * sizeIncrese;
		if(maxSize > 0 && maxSize < mesh.characterSize) mesh.characterSize = maxSize;
		
		var matIndex = 0;
		if (thresholds.length > 0) {
			for(; matIndex < thresholds.length; matIndex++) {
				if (point < thresholds[matIndex]) break;
			}
		}
		GetComponent.<Renderer>().material = materials[matIndex];
		
		super();
	}
	function Update () {
		transform.position += Vector3.up * upSpeed * Time.deltaTime;
	}
}
@script RequireComponent(TextMesh)