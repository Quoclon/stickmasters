using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	#region Singleton
	// Singleton instance.
	public static SoundManager Inst = null;

	// Initialize the singleton instance.
	private void Awake()
	{
		// If there is not already an instance of SoundManager, set it to this.
		if (Inst == null)
		{
			Inst = this;
		}
		//If an instance already exists, destroy whatever this object is to enforce the singleton.
		else if (Inst != this)
		{
			Destroy(gameObject);
		}

		//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
		DontDestroyOnLoad(gameObject);
	}
	#endregion

	// https://www.daggerhartlab.com/unity-audio-and-sound-manager-singleton-script/
	[Header("Last Audio Source Played")]
	public AudioSource lastAudioSourcePlayed;

	[Header("Audio Sources")]
	public AudioSource[] swordClashes;
	public AudioSource[] swordSwings;
	public AudioSource[] severedLimbs;
	public AudioSource playerHit;

	// Play a single clip via passing in the public SoundManager singleton AudioSource
	public void Play(AudioSource audioSource)
	{
		audioSource.Play();
		lastAudioSourcePlayed = audioSource;
	}

	public void PlayRandomFromArray(AudioSource[] audioSources)
	{
		AudioSource audioSource = audioSources[Random.Range(0, audioSources.Length-1)];

		/*
		if (lastAudioSourcePlayed != null)
			if (lastAudioSourcePlayed.isPlaying)
				return;
		*/

		audioSource.Play();
		lastAudioSourcePlayed = audioSource;
	}
}
