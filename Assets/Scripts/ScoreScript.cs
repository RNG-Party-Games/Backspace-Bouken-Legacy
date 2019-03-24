using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

	int missedWords;
	Text text;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int GetMissedWords() {
		return missedWords;
	}

	public void Restart() {
		missedWords = 0;
		text.text = "" + missedWords;
	}

	public void Miss(int missed) {
		missedWords += missed;
		text.text = "" + missedWords;	
	}
}
