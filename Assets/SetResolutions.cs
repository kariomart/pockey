using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SetResolutions : MonoBehaviour {

	public Resolution[] resolutions;
	public  TextMeshProUGUI resText;
	public TextMeshProUGUI fullscreenText;
	int resValue;
	bool isFullscreen;
	float musicVol;
	public TextMeshProUGUI musicVal;
	AudioSource menuMusic;
	float sfxVol;
	public AudioMixerGroup sfxMixer;
	public TextMeshProUGUI sfxVal;
    public int periodVal;
    public int[] periodVals = {60, 120, 180, 300, 30};
    public int periodIndex;
    public TextMeshProUGUI periodValText;
	public AudioSource musicSource;
	public float sfxMaxVol;
	
    
	// Use this for initialization
	void Start () {


		resolutions = Screen.resolutions;
		sfxVol = 100;
		musicVol = 100;
		
	}

	public void SetResolution(int resolutionIndex) {

		Resolution resolution = resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
		resText.text = "RESOLUTION\t" + resolution.width + "\t\t\t" + resolution.height + "\t\t\tSM2093";
		//resText.text = "" + resolution.width;

	}

	void OnEnable() {
		//resolutionDropdown.Select(); 
	}

	public void ChangeController1() {
		Master.me.NextController(0);
	}

	public void ChangeController2() {
		Master.me.NextController(1);
	}

	public void ChangeResolution() {
		resValue ++;
		resValue %= resolutions.Length;
		SetResolution(resValue);
	}

    public void QuitGame() {
        Application.Quit();
    }

	public void RestartGame() {
		Master.me.NewPeriod(1);
	}

	public void toggleFullscreen() {
		if (isFullscreen) {
			isFullscreen = false;
			fullscreenText.text = "FULLSCREEN\t" + "ON" + "\t\t\t53124" + "\t\t\tSM2095";
		} else {
			isFullscreen = true;
			fullscreenText.text = "FULLSCREEN\t" + "OFF" + "\t\t\t53124" + "\t\t\tSM2095";
		}

		Screen.fullScreen = isFullscreen;
	}

    public void changePeriodLength() {

        periodIndex++;
        periodIndex%=periodVals.Length;
		periodVal = periodVals[periodIndex];
		Master.me.periodLength = periodVal;
        periodValText.text = "PERIOD\t" + periodVal + "\t\t\t53524" + "\t\t\tSM2097";

	}

	public void changeMusicVolume() {

		musicVol += 10f;
		musicVol %= 110f;
		setMusicVolume(); 

	}

	void setMusicVolume() {
        //Debug.Log("changed music vol");
		musicVal.text = "" + musicVol + "%";
        musicVal.text = "MUSIC\t\t" + musicVol + "%\t\t\t53124" + "\t\t\tSM2096";
		musicSource.volume = (musicVol/100f) * .5f;
	}

	public void changeSFXVolume() {

		sfxVol += 10f;
		sfxVol %= 110f;
		setSFXVolume(); 

	}

	void setSFXVolume() {
		Debug.Log("changed music vol");
		sfxVal.text = "" + sfxVol + "%";
        sfxVal.text = "SFX\t\t" + sfxVol + "%\t\t\t53124" + "\t\t\tSM2097";
		if (sfxVol == 0) {
			sfxMixer.audioMixer.SetFloat("Volume", -80);
		} else {
			sfxMixer.audioMixer.SetFloat("Volume", sfxMaxVol + 50 * (sfxVol/100f));
		}

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
