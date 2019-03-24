using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshake : MonoBehaviour {

	// How long the object should shake for.
	public float shakeDuration = 0f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;
	public Vector3 originalPos;

	void Start () {
		originalPos = transform.position;
	}

	public void Shake(float duration, float shakeAmount, float decreaseFactor) {
		shakeDuration = duration;
		originalPos = transform.position;
	}
	
	void Update () {
		if (shakeDuration > 0)
		{
			transform.position = originalPos + Random.insideUnitSphere * shakeAmount;
			
			shakeDuration -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shakeDuration = 0f;
			transform.position = originalPos;
		}
		
	}
}
