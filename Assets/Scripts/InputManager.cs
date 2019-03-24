using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour {

	string fullSentence = "", signSentence = "", hintSentence = "";
	public Text primaryTextBox, secondaryTextBox, spacesText;
	public CalculateScreen calcScreen;
	public TimerScript timer;
	public ScoreScript score;
	public GameoverScript gameoverScript;
	string[,] contractions;
	string[] contractionInput;
	string[] fullWords;
	List<string> inputWords;
	int currentFullWord = 0, currentInputWord = 0, currentSentenceInBattle = 0;
	List<int> contractionAtInputWord;
	int spaces = 0;
	public enum BattleState {Initialized, Started, Finished, Gameovered};
	public enum SignState {Initialized, Started, Finished};
	bool finishedLevel = false;
	BattleState battleState = BattleState.Initialized;
	SignState signState = SignState.Initialized;
	Encounter currentEncounter;
	Sign currentSign;
	string endingPunctuation = ".";
	public GameInformation gameinfo;
	public int characterLimit = 100;
	// Use this for initialization
	void Start () {
		fullWords = new string[0];
		inputWords = new List<string>();
		contractions = new string[,] {{"i", "am"},{"i", "would"}, {"we", "are"},{"you", "are"},{"he", "is"},{"she", "is"}
		,{"i", "had"}
		,{"i", "have"},{"we", "have"},{"you", "have"},{"he", "has"},{"she", "has"}
		,{"i", "will"},{"we", "will"},{"you", "will"},{"he", "will"},{"she", "will"}
		,{"what", "is"}, {"what", "has"},{"what", "are"},{"why", "is"},{"why", "are"},{"how", "is"},{"how", "are"}
		,{"it", "has"},{"it", "is"},{"it", "will"},{"it", "had"},{"it", "would"}
		,{"how", "did"}
		,{"is", "not"}, {"has", "not"}
		,{"do", "not"}, {"did", "not"}
		,{"can", "not"}
		,{"should", "not"}, {"could", "not"}, {"would", "not"}
		,{"how", "have"}
		,{"let", "us"}, {"here", "is"}};
		contractionInput = new string[] {"i'm", "i'd", "we're", "you're", "he's", "she's"
		,"i'd"
		,"i've", "we've", "you've", "he's", "she's"
		,"i'll", "we'll", "you'll", "he'll", "she'll"
		,"what's", "what's", "what're", "why's", "why're", "how's", "how're"
		,"it's", "it's", "it'll", "it'd", "it'd"
		,"how'd"
		,"isn't", "hasn't"
		,"don't", "didn't"
		,"can't"
		,"shouldn't", "couldn't", "wouldn't"
		,"how've"
		,"let's", "here's"};
		contractionAtInputWord = new List<int>();
		UpdateSpaces();
	}
	
	public void BeginEncounter(Encounter nextEncounter) {
		if(nextEncounter.isFinal()) {
			gameinfo.finalBattleMusic();
		}
		else gameinfo.normalBattleMusic();
		currentEncounter = nextEncounter;
		currentEncounter.enemySprite.gameObject.SetActive(true);
		fullSentence = currentEncounter.GetSentences()[0];
		battleState = BattleState.Started;
		//splitting sentence into words
		fullWords = fullSentence.Split();
		inputWords = new List<string>();
		//adding equivalent words for input
		foreach(string word in fullWords) {
			inputWords.Add("");
		}
		DisplayFullText();
		string lastSentence = fullWords[fullWords.Length - 1];
		endingPunctuation = ""+lastSentence[lastSentence.Length - 1];
	}

	public void BeginSign(Sign nextSign) {
		currentSign = nextSign;
		signSentence = currentSign.GetSentence();
		signState = SignState.Started;
		DisplaySignText();
	}

	public void BeginHint(string hintText) {
		hintSentence = hintText;
		DisplayHintText();
	}

	public void ClearText() {
		fullWords = new string[0];
		inputWords = new List<string>();
		fullSentence = "";
		primaryTextBox.text = "";
		secondaryTextBox.text = "";
		DisplayFullText();
		DisplayInputText();
	}

	// Update is called once per frame
	void Update () {
		if(battleState == BattleState.Started) {
			//grabbing input from this frame;
			string inputThisFrame = Input.inputString;
			if(inputThisFrame.Contains("\b") || inputThisFrame.Contains(" ")) { //filter out backspace and space
				inputThisFrame = "";
			}
			int inputChars = 0;
			foreach(string input in inputWords) {
				inputChars+=input.Length;
			}
			if(inputChars < characterLimit && inputThisFrame != string.Empty) {
				inputWords[currentInputWord] += inputThisFrame;
				gameinfo.key();
			}
			else if(inputChars >= characterLimit && inputThisFrame != string.Empty){
				gameinfo.mistake();
			}
			if(Input.GetButtonDown("Space")) { //space pressed
				if(spaces > 0) {
					gameinfo.checkLowSpaces(spaces, true);
					AdvanceWord();
					spaces--;
					UpdateSpaces();
					gameinfo.space();
				}
				else {
					gameinfo.checkLowSpaces(spaces, false);
					Gameover();
				}
			}
			if(inputThisFrame.Contains(endingPunctuation) && currentFullWord > fullWords.Length - 2) {
				Punctuation();
			}
			else if(Input.GetButtonDown("Backspace")) { //backspace pressed
				if(inputWords[currentInputWord].Length > 1) {
					inputWords[currentInputWord] = inputWords[currentInputWord].Substring(0, inputWords[currentInputWord].Length - 1);
				}
				else if(inputWords[currentInputWord].Length == 1)
				{
					inputWords[currentInputWord] = string.Empty;
				}
				else if(currentInputWord > 0 && currentFullWord > 0) {
					++spaces;
					UpdateSpaces();
					if(contractionAtInputWord.Contains(currentInputWord-1)) { //going back 2 because contractions
						contractionAtInputWord.Remove(currentInputWord-1);
						currentFullWord -=2;
					}
					else {
						currentFullWord--;
					}
					currentInputWord--;
				}
				gameinfo.backspace();
			}
			if(battleState != BattleState.Finished) {
				DisplayInputText();
				DisplayFullText();
			}
		}
		else if(signState == SignState.Started) {
			if(Input.GetButton("Backspace")) { //backspace pressed
				gameinfo.backspace();
				if(""+signSentence[signSentence.Length-1] == " ") { //backspacing a space
					spaces++;
					UpdateSpaces();
				}
				signSentence = signSentence.Substring(0, signSentence.Length - 1);
				if(signSentence == string.Empty) {
					FinishSign();
				}
			}
			DisplaySignText();
		}
	}

	void UpdateSpaces() {
		spacesText.text = ""+spaces;
	}

	void Gameover() {
		ClearText();
		currentFullWord = 0;
		currentInputWord = 0;
		currentSentenceInBattle = 0;
		
		gameoverScript.gameObject.SetActive(true);
		timer.StopAndGetTime();
		score.Restart();
		gameinfo.normalMusic();
		gameinfo.ResetAll();
		battleState = BattleState.Gameovered;
		signState = SignState.Initialized;
		currentEncounter.enemySprite.gameObject.SetActive(false);
	}

	public void GameoverRecover() {
		timer.Restart();
		battleState = BattleState.Initialized;
	}

	void AdvanceWord() {
		if(currentFullWord > fullWords.Length - 2 && battleState != BattleState.Finished) {
			//nothing
		}
		else {
			bool contractionFound = false;
			for(int j = 0; j < contractions.GetLength(0); ++j) {
				if(fullWords[currentFullWord].ToLower() == contractions[j,0] && fullWords[currentFullWord+1].ToLower() == contractions[j,1] 
				&& inputWords[currentInputWord].ToLower() == contractionInput[j]) {
					contractionFound = true;
				}
				else if(fullWords[currentFullWord].ToLower() == "("+contractions[j,0] && fullWords[currentFullWord+1].ToLower() == contractions[j,1] 
				&& inputWords[currentInputWord].ToLower() == "("+contractionInput[j]) {
					contractionFound = true;
				}
				else if(fullWords[currentFullWord].ToLower() == contractions[j,0] && fullWords[currentFullWord+1].ToLower() == contractions[j,1]+")" 
				&& inputWords[currentInputWord].ToLower() == contractionInput[j]+")") {
					contractionFound = true;
				}
			}
			if(contractionFound) {
				contractionAtInputWord.Add(currentInputWord);
				currentFullWord+=2;
				currentInputWord++;
			}
			else {
				currentFullWord++;
				currentInputWord++;
			}
		}
	}

	void Punctuation() {
		FinishSentence();
		if(currentSentenceInBattle < currentEncounter.GetSentences().Length - 1) {
			currentSentenceInBattle++;
			fullSentence = currentEncounter.GetSentences()[currentSentenceInBattle];
			DisplayFullText();
			DisplayInputText();
			fullWords = fullSentence.Split();
			inputWords = new List<string>();
			//adding equivalent words for input
			foreach(string word in fullWords) {
				inputWords.Add("");
			}
			DisplayFullText();
			string lastSentence = fullWords[fullWords.Length - 1];
			endingPunctuation = ""+lastSentence[lastSentence.Length - 1];
		}
		else {
			FinishEncounter();
		}
	}

	void DisplaySignText() {
		primaryTextBox.text = signSentence;
	}

	void DisplayHintText() {
		primaryTextBox.text = hintSentence;
	}

	void DisplayFullText() {
		string sentence = "";
		for(int i = 0; i < fullWords.Length; ++i) {
			if(i == currentFullWord) {
				sentence += "<color=red>"+fullWords[i]+"</color> ";
			}
			else {
				sentence += fullWords[i]+" ";
			}
		}
		primaryTextBox.text = sentence;
	}

	void DisplayInputText() {
		string sentence = "";
		for(int i = 0; i < inputWords.Count; ++i) {
			if(i == currentInputWord) {
				sentence += "<color=green>"+inputWords[i]+"</color> ";
			}
			else {
				sentence += inputWords[i]+" ";
			}
		}
		secondaryTextBox.text = sentence;
	}

	void FinishSign() {
		currentSign.Finish();
		signState = SignState.Initialized;
	}

	void FinishSentence() {
		gameinfo.attack(currentEncounter.enemySprite.transform);
		int full_compare = 0, input_compare = 0;
		int missedWords = 0;
		for(full_compare = 0; full_compare < fullWords.Length; ++full_compare) {
			bool missed = true;
			if(full_compare != fullWords.Length - 1) { //still a word after this
				bool contractionFound = false;
				for(int j = 0; j < contractions.GetLength(0) - 1; ++j) {
				if(fullWords[full_compare].ToLower() == contractions[j,0] && fullWords[full_compare+1].ToLower() == contractions[j,1] 
				&& inputWords[input_compare].ToLower() == contractionInput[j]) {
					contractionFound = true;
				}
				else if(fullWords[full_compare].ToLower() == "("+contractions[j,0] && fullWords[full_compare+1].ToLower() == contractions[j,1] 
				&& inputWords[input_compare].ToLower() == "("+contractionInput[j]) {
					contractionFound = true;
				}
				else if(fullWords[full_compare].ToLower() == contractions[j,0] && fullWords[full_compare+1].ToLower() == contractions[j,1]+")" 
				&& inputWords[input_compare].ToLower() == contractionInput[j]+")") {
					contractionFound = true;
				}
				}
				if(contractionFound) { //contraction found (skips a word, so +3)
					full_compare++;
					missed = false;
				}
			}
			if(fullWords[full_compare] == inputWords[input_compare]) { //no contraction found
				missed = false;
			}
			++input_compare;
			if(missed) {
				++missedWords;
			}
		}
		score.Miss(missedWords);
		fullWords = new string[0];
		inputWords = new List<string>();
		contractionAtInputWord = new List<int>();
		currentFullWord = 0;
		currentInputWord = 0;
		DisplayFullText();
		DisplayInputText();	
	}

	void FinishEncounter() {
		gameinfo.explode(currentEncounter.enemySprite.transform);
		gameinfo.checkLowSpaces(spaces, false);
		battleState = BattleState.Initialized;
		currentEncounter.Finish();
		currentEncounter.enemySprite.gameObject.SetActive(false);
		currentSentenceInBattle = 0;
		if(currentEncounter.isFinal()) {
			battleState = BattleState.Started;
			Invoke("Credits", 2.0f);
		}
		else {			
			gameinfo.normalMusic();
		}
	}

	public void Credits() {
		SceneManager.LoadScene("Credits");
	}

	public BattleState GetBattleState() {
		return battleState;
	}

	public SignState GetSignState() {
		return signState;
	}

	public bool levelFinished() {
		return finishedLevel;
	}

	public void CalcTotal() {
		finishedLevel = true;
		int time = timer.StopAndGetTime();
		calcScreen.gameObject.SetActive(true);
		calcScreen.Calc(spaces, time, score.GetMissedWords());
	}

	public void AdvanceCalc() {
		finishedLevel = false;
		timer.Restart();
		score.Restart();
		spaces = 0;
		UpdateSpaces();
		calcScreen.gameObject.SetActive(false);
	}

}
