﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

	public static SoundController me;
	public GameObject audSource;
	public AudioSource[] audSources;
	public float panAmount = 1f;

	[Header("SFX")]
	public List<AudioClip> sfx_wallHits;
	public List<AudioClip> sfx_shots;
	public List<AudioClip> sfx_pinball;


	void Update() {

		//FixSoundSpeeds ();

	}

	void Awake(){
		me = this;
	}
		


	void Start () {
		audSources = new AudioSource[128];

		for (int i = 0; i < audSources.Length; i++) {

			AudioSource aSource = (Instantiate (audSource, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<AudioSource>();
			audSources [i] = aSource;
			aSource.gameObject.transform.parent = this.transform;


		}

	}

	public static SoundController Get() {
		if (me == null) {
			me = (SoundController)FindObjectOfType(typeof(SoundController));
		}

		return me;
	}

	public void PlayRandomSound(List<AudioClip> sfxList) {
		AudioClip a = sfxList[Random.Range(0, sfxList.Count)];
		PlaySound(a, 1f);
	}


	public void PlaySound(AudioClip snd, float vol)

	{
		//		Debug.Log (snd);
		int sNum = GetSourceNum ();
		audSources [sNum].clip = snd;
		audSources [sNum].volume = vol;
		audSources [sNum].pitch = Time.timeScale;
		audSources [sNum].panStereo = 0;
		audSources [sNum].Play ();
	}

	public void PlaySound(AudioClip snd, float vol, float pitch)

	{
		int sNum = GetSourceNum ();
		audSources [sNum].clip = snd;
		audSources [sNum].volume = vol;
		audSources [sNum].pitch = pitch * Time.timeScale;
		audSources [sNum].panStereo = 0;
		audSources [sNum].Play ();
	}

	public void PlaySound(AudioClip snd, float vol, float pitch, float xPos)

	{
		//Debug.Log (snd);
		int sNum = GetSourceNum ();
		audSources [sNum].clip = snd;
		audSources [sNum].volume = vol;
		audSources [sNum].pitch = pitch * Time.timeScale;

		if (Mathf.Abs(xPos) > 2) {
			audSources [sNum].panStereo = Geo.Remap(xPos, -11, 11, -panAmount, panAmount);
		} else {
			audSources [sNum].panStereo = 0;
		}
		audSources [sNum].Play ();
	}

	public void PlaySoundAtNormalPitch(AudioClip snd, float vol)

	{
		//Debug.Log (snd);
		int sNum = GetSourceNum ();
		audSources [sNum].clip = snd;
		audSources [sNum].volume = vol;
		audSources [sNum].panStereo = 0;
		audSources [sNum].pitch = 1;
		audSources [sNum].Play ();
	}

	public void PlaySoundAtNormalPitch(AudioClip snd, float vol, float xPos)

	{
		//Debug.Log (snd);
		int sNum = GetSourceNum ();
		audSources [sNum].clip = snd;
		audSources [sNum].volume = vol;
		audSources [sNum].pitch = 1;

		if (Mathf.Abs(xPos) > 2) {
			audSources [sNum].panStereo = Geo.Remap(xPos, -10, 10, -panAmount, panAmount);
		} else {
			audSources [sNum].panStereo = 0;
		}

		audSources [sNum].Play ();
	}

	public void PlaySoundAtPitch(AudioClip snd, float vol, float pitch)

	{
		//Debug.Log (snd);
		int sNum = GetSourceNum ();
		audSources [sNum].clip = snd;
		audSources [sNum].volume = vol;
		audSources [sNum].panStereo = 0;
		audSources [sNum].pitch = pitch;
		audSources [sNum].Play ();
	}


	// Update is called once per frame
	public int GetSourceNum()
	{

		for (int i = 0; i < audSources.Length; i++)
		{
			if(!audSources[i].isPlaying)
				return i;
		}
		return 0;
	}

	public void FixSoundSpeeds() {

		for (int i = 0; i < audSources.Length; i++) {

			if (audSources [i].isPlaying) {

				audSources [i].pitch = Time.timeScale;
			}


		}




	}




}
