using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour {

	Animator anim;
	Text text;
	int timeSinceStart = 0;
	// Use this for initialization
	void Start () {
		timeSinceStart = 0;
		text = GetComponent<Text>();
		anim = GetComponent<Animator>();
		InvokeRepeating("Tick", 0, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Tick() {
		anim.Play("Tick");
		timeSinceStart++;
		text.text = "" + (int) Mathf.Floor(timeSinceStart);
	}

	public void Restart() {
		timeSinceStart = 0;
		CancelInvoke();
		InvokeRepeating("Tick", 0, 1.0f);
	}

	public int StopAndGetTime() {
		CancelInvoke();
		return timeSinceStart;
	}
}
