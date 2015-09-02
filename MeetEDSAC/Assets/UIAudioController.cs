using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIAudioController : MonoBehaviour {

	public enum AudioEvent { VERTEX_CHANGED, ZOOM_CHANGED, HINGE_OUT, HINGE_AWAY, INSPECTION_POINT_CLICKED };

	public AudioSource source;
	public Dictionary<AudioEvent,AudioClip> audioClips;
	public AudioClip changeVertexClip;
	public AudioClip changeZoomClip;
	public AudioClip hingeOutClip;
	public AudioClip hingeAwayClip;
	public AudioClip inspectionPointClip;

	public bool disabled = false;

	public void DisableKinectAudio() {
		foreach (AudioSource s in gameObject.GetComponents<AudioSource>()) {
			s.Stop();
			Destroy (s);
		}
		disabled = true;
	}

	void Awake() {
		audioClips = new Dictionary<AudioEvent,AudioClip>();
		audioClips.Add(AudioEvent.VERTEX_CHANGED,changeVertexClip);
		audioClips.Add(AudioEvent.ZOOM_CHANGED,changeZoomClip);
		audioClips.Add(AudioEvent.HINGE_OUT,hingeOutClip);
		audioClips.Add(AudioEvent.HINGE_AWAY,hingeAwayClip);
		audioClips.Add(AudioEvent.INSPECTION_POINT_CLICKED,inspectionPointClip);
	}

	public void RunAudioEvent(AudioEvent ae) {
		if (!disabled) {
			source.Stop();
			source.clip = audioClips[ae];
			source.volume = (ae == AudioEvent.ZOOM_CHANGED) ? 0.1f : 1f;
			source.Play();
		}
	}

}
