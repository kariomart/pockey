﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshake : MonoBehaviour {

	public Vector3 defaultCameraPos;
	Vector3 weightedDirection;
	float screenshakeTimer = 0;
	float thisMagnitude = 0;
	bool shaking;

    Camera cam;
	CameraController camControl;

	// Use this for initialization
	void Start () {
		defaultCameraPos = transform.position;
        cam = Camera.main;
		camControl = GetComponent<CameraController>();
	}
			
	// Update is called once per frame
	void Update () {

		if (screenshakeTimer > 0) {
			shaking = true;
			Vector3 shakeDirection = ((Vector3)Random.insideUnitCircle + weightedDirection).normalized * thisMagnitude * Mathf.Clamp01(screenshakeTimer);
            float zoomFactor = cam.orthographicSize / 15f;
            shakeDirection *= zoomFactor;
			camControl.shakeChange = shakeDirection;
			Vector3 result = defaultCameraPos + shakeDirection;
			result.z = -10;
			//transform.position = result;
			screenshakeTimer -= Time.deltaTime;
		} 

		else {
			camControl.shakeChange = Vector3.zero;
			if (shaking) {
				shaking = false;
			}
		}
	}

	public void SetScreenshake(float magnitude, float duration, Vector3 direction) {
        defaultCameraPos = transform.position;
		thisMagnitude = magnitude;
		screenshakeTimer = duration;
		weightedDirection = direction;

	}

	public void SetScreenshake(float magnitude, float duration, Vector3 origin, Vector3 direction) {
		defaultCameraPos = origin;
		thisMagnitude = magnitude;
		screenshakeTimer = duration;
		weightedDirection = direction;

	}

	public void SetScreenshake(float magnitude, float duration) {
		SetScreenshake (magnitude, duration, Vector3.zero);

	}

	public void SetScreenshake(float magnitude, float duration, PlayerController player) {
		SetScreenshake (magnitude, duration, Vector3.zero);
	}

	


}