using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum Videos { TAPE_READ };

public class VideoTextureController : MonoBehaviour {

	public MovieTexture[] videos;
	public AudioClip[] videoAudio;

	public RawImage textureTarget;
	public AudioSource audioTarget;

	// Use this for initialization
	void Start () {
		if (videos.Length > 0 && videoAudio.Length > 0) {
			textureTarget.texture = videos[0];
			audioTarget.clip = videoAudio[0];
			((MovieTexture)textureTarget.mainTexture).Play();
			audioTarget.Play ();	
		}
	}

	public void SetVideo(Videos target, bool play = true) {
		textureTarget.texture = videos[(int)target];
		audioTarget.clip = videoAudio[(int)target];
		if (play) {
			((MovieTexture)textureTarget.mainTexture).Play();
			audioTarget.Play ();	
		}
	}
}
