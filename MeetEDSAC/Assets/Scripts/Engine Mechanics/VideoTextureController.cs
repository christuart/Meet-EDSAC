using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum Videos { 	EDSAC_FILM = 0,
						NONE };

public class VideoTextureController : MonoBehaviour {

	public MovieTexture[] videos;
	public AudioClip[] videoAudio;

	public RawImage textureTarget;
	public AudioSource audioTarget;

	private bool mute;

	// Use this for initialization
	void Start () {
		if (videos.Length > 0 && videoAudio.Length > 0) {
			SetVideo (0,true);
		}
	}
	
	public void SetVideo(Videos target, bool play = true) {
		SetVideo ((int)target,play);
	}
	public void SetVideo(int target, bool play = true) {
		Stop();
		textureTarget.texture = videos[target];
		audioTarget.clip = videoAudio[target];
		if (play) 
			Play();
	}

	public void Play() {
		if (textureTarget.mainTexture != null) {
			if (textureTarget.mainTexture.GetType() == typeof(MovieTexture)) {
				((MovieTexture)textureTarget.mainTexture).Play();
				audioTarget.Play ();	
			}
		}
	}
		
	public void Stop() {
		if (textureTarget.mainTexture != null) {
			if (textureTarget.mainTexture.GetType() == typeof(MovieTexture)) {
				((MovieTexture)textureTarget.mainTexture).Stop();
			}
		}
		if (audioTarget != null) audioTarget.Stop();
	}

	public void Pause() {
		if (textureTarget.mainTexture != null) {
			if (textureTarget.mainTexture.GetType() == typeof(MovieTexture)) ((MovieTexture)textureTarget.mainTexture).Pause();
			audioTarget.Pause ();
		}
	}

	public bool IsPlaying() {
		if (textureTarget.mainTexture != null) {
			if (textureTarget.mainTexture.GetType() == typeof(MovieTexture)) {
				return ((MovieTexture)textureTarget.mainTexture).isPlaying;
			}
		}
		return false;
	}
	
	public IEnumerator Mute() {
		mute = true;
		for (float v = audioTarget.volume; v > 0f; v -= 0.1f) {
			if (!mute) yield break;
			if (audioTarget.volume < v) yield break;
			audioTarget.volume = v;
			yield return null;
		}
		audioTarget.volume = 0f;
	}
	
	public IEnumerator Unmute() {
		mute = false;
		for (float v = audioTarget.volume; v < 1f; v += 0.1f) {
			if (mute) yield break;
			if (audioTarget.volume > v) yield break;
			audioTarget.volume = v;
			yield return null;
		}
		audioTarget.volume = 1f;
	}
}
