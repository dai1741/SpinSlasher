

	
@script RequireComponent(AudioSource);
public class AudioChain extends MonoBehaviour implements PlayableAudio {
	
	var audioClips : AudioClip[];
	
	/**
	 * Ranges describing what audios should be looped.
	 * First element of a vector (i.e. x) means start index.
	 * Second one (i.e. y) means end index. inclusive.
	 * Each index must be less than audioClips.length and must be x <= y.
	 * This array can be empty.
	 */
	var loopRanges : Vector2[];
	
	/*
	 * Loop counts corresponding to loop ranges.
	 * zero describes the loop lasts eternally.
	 * One describes no loop performed.
	 * So when you set this var to n then the audios played n times.
	 * Each count must be non-negative. Must be loopRanges.length == loopCounts.length.
	 */
	var loopCounts: int[];
	
	var loopEntireAudio : boolean = false;
	
	var playOnStart : boolean = true;
	
	/*
	 * Example data set:
	 * audioClips = [audio0, audio1, audio2, audio3]
	 * loopRanges = [[0,1], [2,2]]
	 * loopCounts = [2    , 3    ]
	 * loopEntireAudio = false
	 *
	 * In this case AudioChain will play the following audio sequence:
	 * 		audio0, audio1, audio0, audio1, audio2, audio2, audio02, audio3
	 */
	
	private var currentIndex = -1;
	private var currentRangesIndex = -1;
	private var currentRemainedLoop = -1;
	private var isPlaying = false;
	
	function Start() {
		if(playOnStart && audioClips.Length > 0) StartCoroutine("ManageAudio");
	}
	
	protected function ManageAudio() {
		while(true) {
		
			if(loopRanges.Length > 0 && currentRangesIndex < loopRanges.Length) {
				if(currentRangesIndex == -1) {
					currentRangesIndex++;
					currentRemainedLoop = loopCounts[currentRangesIndex];
					//先にセットされてしまうが特に問題はない
				}
				else if(loopRanges[currentRangesIndex].y == currentIndex) {
					if(currentRemainedLoop == 1) {
						currentRangesIndex++;
						if(currentRangesIndex < loopRanges.Length) currentRemainedLoop = loopCounts[currentRangesIndex];
					}
					else {
						currentIndex = loopRanges[currentRangesIndex].x - 1;
						//直後にインクリメントされるので1引く
						Debug.Log("rewind to :" + currentIndex);
						if(currentRemainedLoop > 1) currentRemainedLoop--;
					}
				}
			}
			
			if(!loopEntireAudio && currentIndex + 1 >= audioClips.Length) {
				isPlaying = false;
				break;
			}
			
			currentIndex++;
			if(loopEntireAudio && currentIndex >= audioClips.Length) {
				currentIndex = currentRangesIndex = currentRemainedLoop = -1;
				continue;
			}
			
			if(audio.isPlaying) audio.Stop();
			audio.clip = audioClips[currentIndex];
			audio.Play();
			isPlaying = true;
			
			yield WaitForSeconds(audio.clip.length);yield WaitForFixedUpdate();
		}
	}
	
	public function StopAudio() {
		StopCoroutine("ManageAudio");
		/*if(audio.isPlaying)*/ audio.Stop();
		currentIndex = currentRangesIndex = currentRemainedLoop = -1;
		isPlaying = false;
		Debug.Log("BGM STOP");
	}
	
	public function StartAudio() {
		Debug.Log("BGM START");
		if(isPlaying) StopAudio();
		if(audioClips.Length > 0) StartCoroutine("ManageAudio");
	}

}