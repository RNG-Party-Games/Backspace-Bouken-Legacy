using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CalculateScreen : MonoBehaviour {

	public float valuePerSpace, valuePerSecond, valuePerMiss;
	public Text spaceTotal, timeTotal, wordsTotal, total;
	public string nextFloor;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Calc(int spaces, int time, int misses) {
		int spaceValue = Mathf.FloorToInt(spaces * valuePerSpace);
		int timeValue = Mathf.FloorToInt(time * valuePerSecond);
		int missValue = Mathf.FloorToInt(misses * valuePerMiss);
		int totalValue = spaceValue - timeValue- missValue;
		spaceTotal.text = "" + spaceValue;
		timeTotal.text = "" + timeValue;
		wordsTotal.text = "" + missValue;
		total.text = "" + totalValue;
	}
}
