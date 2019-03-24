using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameoverScript : MonoBehaviour {

	public Movement render;
	public Image frame;
	public Sprite gameoverSprite, normalSprite;
	// Use this for initialization
	void Start () {
		frame.sprite = gameoverSprite;		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Space")) {
			frame.sprite = normalSprite;
			render.ResetGameover();
			gameObject.SetActive(false);
		}
	}
}
