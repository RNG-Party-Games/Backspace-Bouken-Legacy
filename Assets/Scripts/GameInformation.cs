using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInformation : MonoBehaviour {

	public List<Encounter> encounters;
	public List<Map> maps;
	public List<Sign> signs;
	public Color ending;
	int endingX, endingY;
	public AudioClip dungeon, dungeonBattle, finalDungeon, finalDungeonBattle;
	public AudioClip spaceSFX, backspaceSFX, mistakeSFX, keySFX, doorSFX, attackSFX, explosionSFX;
	public float spaceVol, backspaceVol, mistakeVol, keyVol, doorVol, attackVol, explosionVol;
	public float moveVol;
	public AudioClip[] moveSFX;
	public AudioSource music, sfx, lowSpaces;
	public bool isFinalDungeon = false;
	public Animator attackSprite, explosionSprite;
	public Sprite[] dungeonSprites, finalDungeonSprites;
	public int currentFloor = 0;
	public Screenshake shake;
	public ParticleSystem normalParticles, finalParticles;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Map GetMap() {
		return maps[currentFloor];
	}

	public void AdvanceFloor() {
		++currentFloor;
		isFinalDungeon = maps[currentFloor].isFinal;
		finalParticles.gameObject.SetActive(isFinalDungeon);
		normalParticles.gameObject.SetActive(!isFinalDungeon);
		normalMusic();
	}
	public void normalMusic() {
		if(isFinalDungeon) music.clip = finalDungeon;
		else music.clip = dungeon;
		music.Play();
	}

	public Sprite[] getSpriteset() {
		isFinalDungeon = maps[currentFloor].isFinal;
		if(isFinalDungeon) return finalDungeonSprites;
		else return dungeonSprites;
	}

	public void normalBattleMusic() {
		music.clip = dungeonBattle;
		music.Play();
	}
	public void finalBattleMusic() {
		music.clip = finalDungeonBattle;
		music.Play();
	}

	public void space() {
		sfx.volume = spaceVol;
		sfx.clip = spaceSFX;
		sfx.Play();
	}

	public void backspace() {
		sfx.volume = backspaceVol;
		sfx.clip = backspaceSFX;
		sfx.Play();
	}

	public void key() {
		sfx.volume = keyVol;
		sfx.clip = keySFX;
		sfx.Play();
	}

	public void door() {
		sfx.volume = doorVol;
		sfx.clip = doorSFX;
		sfx.Play();
	}
	public void move() {
		sfx.volume = moveVol;
		int rand = Random.Range(0, 2);
		sfx.clip = moveSFX[rand];
		sfx.Play();
	}

	public void mistake() {
		shake.Shake(0.3f, 0.3f, 1.0f);
		sfx.volume = mistakeVol;
		sfx.clip = mistakeSFX;
		sfx.Play();
	}

	public void checkLowSpaces(int spaces, bool inBattle) {
		if(inBattle) {
			if(spaces <= 5 && !lowSpaces.isPlaying) {
				lowSpaces.Play();
			}
			else if(spaces > 5) {
				lowSpaces.Stop();
			}
		}
		else lowSpaces.Stop();
	}

	public void attack(Transform t) {
		sfx.volume = attackVol;
		sfx.clip = attackSFX;
		sfx.Play();
		attackSprite.transform.position = new Vector3(t.position.x, t.position.y, t.position.z - 2);
		attackSprite.gameObject.SetActive(true);
		attackSprite.Play("Attack");
		shake.Shake(0.5f, 0.3f, 2.0f);
	}
	public void explode(Transform t) {
		sfx.volume = explosionVol;
		sfx.clip = explosionSFX;
		sfx.Play();
		explosionSprite.transform.position = new Vector3(t.position.x, t.position.y, t.position.z - 1);
		explosionSprite.Play("Explosion");
		shake.Shake(1.0f, 0.5f, 1.0f);
	}

	public List<Encounter> GetEncounters() {
		return encounters;
	}

	public List<Sign> GetSigns() {
		return signs;
	}

	public List<Color> GetEncounterColors() {
		List<Color> colors = new List<Color>();
		foreach(Encounter e in encounters) {
			colors.Add(e.GetColor());
		}
		return colors;
	}

	public List<Color> GetSignColors() {
		List<Color> colors = new List<Color>();
		foreach(Sign s in signs) {
			colors.Add(s.GetColor());
		}
		return colors;
	}

	public Color GetEndingColor() {
		return ending;
	}

	public void SetEnding(int x, int y) {
		endingX = x;
		endingY = y;
	}

	public bool isEnding(int x, int y) {
		return (x == endingX && y == endingY);
	}

	public void FinishEncounter(int encounterIndex) {
		encounters.RemoveAt(encounterIndex);
	}

	public Encounter encounterAt(int x, int y) {
		for(int i = 0; i < encounters.Count; ++i) {
			if(encounters[i].GetX() == x && encounters[i].GetY() == y && encounters[i].floor == currentFloor) {
				return encounters[i];
			}
		}
		return null;
	}

	public Sign signAt(int x, int y) {
		for(int i = 0; i < signs.Count; ++i) {
			if(signs[i].GetX() == x && signs[i].GetY() == y && signs[i].floor == currentFloor) {
				return signs[i];
			}
		}
		return null;
	}

	public void ResetAll() {
		foreach(Encounter e in encounters) {
			e.Reset();
		}
		foreach(Sign s in signs) {
			s.Reset();
		}
	}
}

[System.Serializable]
public class Encounter {
	public Color color;
	public string[] sentences;
	public SpriteRenderer enemySprite;
	public string hint;
	public bool final = false;
	int x = 0, y = 0;
	bool finished = false;
	public int floor = 0;

	public Color GetColor() {
		return color;
	}

	public bool isFinal() {
		return final;
	}

	public void SetCoords(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public void Finish() {
		finished = true;
	}

	public bool isFinished() {
		return finished;
	}

	public string[] GetSentences() {
		return sentences;
	}
	
	public int GetX() {
		return x;
	}
	public int GetY() {
		return y;
	}

	public void Reset() {
		finished = false;
	}
}

[System.Serializable]
public class Map {
	public Movement.Direction startingDir;
	public Texture2D map;
	public bool isFinal = false;
}

[System.Serializable]
public class Sign {
	public string sentence;
	public Color color;
	int x = 0, y = 0;
	bool finished = false;
	public int floor = 0;

	public Color GetColor() {
		return color;
	}

	public void SetCoords(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public void Finish() {
		finished = true;
	}

	public bool isFinished() {
		return finished;
	}

	public string GetSentence() {
		return sentence;
	}
	
	public int GetX() {
		return x;
	}
	public int GetY() {
		return y;
	}

	public void Reset() {
		finished = false;
	}
}
