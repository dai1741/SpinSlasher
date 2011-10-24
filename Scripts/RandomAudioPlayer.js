
var audios : AudioClip[];

function GetAnAudio ():AudioClip {
	return audios[Mathf.FloorToInt(Random.Range(0.0, audios.Length - 0.00001f))];
}

function Play() {
	audio.clip = GetAnAudio();
	GameManager.instance.PlaySE(audio);
}
@script RequireComponent(AudioSource)