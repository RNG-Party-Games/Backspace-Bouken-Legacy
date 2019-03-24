using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Movement : MonoBehaviour {


	public SpriteRenderer[] depth_0, depth_1, depth_2;
	public SpriteRenderer[] depth_0_doors, depth_1_doors, depth_2_doors;
	public SpriteRenderer[] depth_0_signs, depth_1_signs, depth_2_signs;
	public SpriteRenderer[] depth_0_exits, depth_1_exits, depth_2_exits;
	List<List<Color>> mapGrid;
	List<List<BlockType>> blockGrid;
	List<List<Color>> minimap;
	List<List<bool>> minimapViewed;
	public enum Direction {North, East, South, West};
	public string[] directionText = {"N", "E", "S", "W"};
	public Color wall, playerSpawn;
	public int finalDungeonIndex = 2;
	public Direction dir = Direction.North;
	public int cameraX, cameraY, lastCameraX, lastCameraY;
	public GameInformation gameinfo;
	public enum BlockType {Empty, Block, Door, Sign, Exit};
	public InputManager typing;
	Texture2D minimapTex;
	public Image minimapImage;
	public SpriteRenderer floor;
	int floorSprite = 0;
	public Sprite[] floorSprites;
	Color frameBackground;
	public ParticleSystem dust, fog;
	public Text compass;
	
	// Use this for initialization
	void Start () {
		LoadMap(true);
	}

	void LoadMap(bool deleteMinimap) {
		dir = gameinfo.GetMap().startingDir;
		compass.text = directionText[(int) dir];
		LoadSpriteset();
		mapGrid = new List<List<Color>>();
		blockGrid = new List<List<BlockType>>();
		minimap = new List<List<Color>>();
		frameBackground = new Color();
		if(deleteMinimap) {
			minimapViewed = new List<List<bool>>();
		}
		ColorUtility.TryParseHtmlString("#1d2025", out frameBackground);
		for(int i = gameinfo.GetMap().map.height-1; i >= 0; --i) {
			mapGrid.Add(gameinfo.GetMap().map.GetPixels().OfType<Color>().ToList().GetRange(i*gameinfo.GetMap().map.width, gameinfo.GetMap().map.width));
			blockGrid.Add(new List<BlockType>());
			minimap.Add(new List<Color>());
			minimapViewed.Add(new List<bool>());
		}

		for(int i = 0; i < mapGrid.Count; ++i) {
			for(int j = 0; j < mapGrid[0].Count; ++j) {
				if(mapGrid[i][j] == new Color(1,1,1)) { //white
					blockGrid[i].Add(BlockType.Empty);
					minimap[i].Add(new Color(1,1,1));
					minimapViewed[i].Add(false);
				}
				else if(mapGrid[i][j] == new Color(0,0,0)) { //black
					blockGrid[i].Add(BlockType.Block);
					minimap[i].Add(new Color(0,0,0));
					minimapViewed[i].Add(false);
				}
				else if(gameinfo.GetEncounterColors().Contains(mapGrid[i][j])) { //black
					blockGrid[i].Add(BlockType.Door);
					minimap[i].Add(new Color(1,1,1));
					minimapViewed[i].Add(false);
				}
				else if(gameinfo.GetSignColors().Contains(mapGrid[i][j])) { //black
					blockGrid[i].Add(BlockType.Sign);
					minimap[i].Add(new Color(1,1,1));
					minimapViewed[i].Add(false);
				}
				else {
					blockGrid[i].Add(BlockType.Empty);
					minimap[i].Add(new Color(1,1,1));
					minimapViewed[i].Add(false);
				}


				if(mapGrid[i][j] == playerSpawn) {
					minimap[i][j] = new Color(1,0,0);
					minimapViewed[i][j] = true;
					cameraX = j;
					cameraY = i;
					lastCameraX = cameraX;
					lastCameraY = cameraY;
				}
				if(mapGrid[i][j] == gameinfo.GetEndingColor()) {
					blockGrid[i][j] = BlockType.Exit;
					minimap[i][j] = new Color(0,1,0);
					minimapViewed[i][j] = true;
					gameinfo.SetEnding(i, j);
				}
				for(int k = 0; k < gameinfo.GetEncounters().Count; ++k) {
					if(gameinfo.GetEncounters()[k].GetColor().Equals(mapGrid[i][j])) {
						gameinfo.GetEncounters()[k].SetCoords(i,j);
					}
				}
				for(int k = 0; k < gameinfo.GetSigns().Count; ++k) {
					if(gameinfo.GetSigns()[k].GetColor().Equals(mapGrid[i][j])) {
						gameinfo.GetSigns()[k].SetCoords(i,j);
					}
				}
			}
		}
		minimapTex = new Texture2D(minimap.Count, minimap[0].Count);
		Color[] texColors = minimapTex.GetPixels();
		for(int i = 0; i < texColors.Length; ++i) {
			texColors[i] = new Color(255, 255, 255);
		}
		minimapTex.SetPixels(texColors);
		minimapTex.filterMode = FilterMode.Point;
		MovementStuff();
		UpdateMinimap();
		minimapImage.gameObject.SetActive(false);		
		minimapImage.gameObject.SetActive(true);
		DisplayWalls();
	}

	void LoadSpriteset() {
		Sprite[] current = gameinfo.getSpriteset();
		//depth 0
		depth_0[0].sprite = current[7];
		depth_0[1].sprite = current[7];
		depth_0[2].sprite = current[7];
		depth_0[3].sprite = current[2];
		depth_0[4].sprite = current[2];
		
		depth_0_doors[0].sprite = current[15];
		depth_0_doors[1].sprite = current[15];
		depth_0_doors[2].sprite = current[15];
		depth_0_doors[3].sprite = current[10];
		depth_0_doors[4].sprite = current[10];
		
		depth_0_exits[0].sprite = current[23];
		depth_0_exits[1].sprite = current[23];
		depth_0_exits[2].sprite = current[23];
		depth_0_exits[3].sprite = current[18];
		depth_0_exits[4].sprite = current[18];

		//depth 1
		depth_1[0].sprite = current[4];
		depth_1[1].sprite = current[4];
		depth_1[2].sprite = current[6];
		depth_1[3].sprite = current[4];
		depth_1[4].sprite = current[4];
		depth_1[5].sprite = current[1];
		depth_1[6].sprite = current[1];
		depth_1[7].sprite = current[1];
		depth_1[8].sprite = current[1];
		
		depth_1_doors[0].sprite = current[12];
		depth_1_doors[1].sprite = current[12];
		depth_1_doors[2].sprite = current[14];
		depth_1_doors[3].sprite = current[12];
		depth_1_doors[4].sprite = current[12];
		depth_1_doors[5].sprite = current[9];
		depth_1_doors[6].sprite = current[9];
		depth_1_doors[7].sprite = current[9];
		depth_1_doors[8].sprite = current[9];
		
		depth_1_exits[0].sprite = current[20];
		depth_1_exits[1].sprite = current[20];
		depth_1_exits[2].sprite = current[22];
		depth_1_exits[3].sprite = current[20];
		depth_1_exits[4].sprite = current[20];
		depth_1_exits[5].sprite = current[17];
		depth_1_exits[6].sprite = current[17];
		depth_1_exits[7].sprite = current[17];
		depth_1_exits[8].sprite = current[17];

		//depth 2
		depth_2[0].sprite = current[3];
		depth_2[1].sprite = current[3];
		depth_2[2].sprite = current[3];
		depth_2[3].sprite = current[3];
		depth_2[4].sprite = current[3];
		depth_2[5].sprite = current[3];
		depth_2[6].sprite = current[3];
		depth_2[7].sprite = current[0];
		depth_2[8].sprite = current[0];
		depth_2[9].sprite = current[0];
		depth_2[10].sprite = current[0];
		depth_2[11].sprite = current[0];
		depth_2[12].sprite = current[0];
		
		depth_2_doors[0].sprite = current[11];
		depth_2_doors[1].sprite = current[11];
		depth_2_doors[2].sprite = current[11];
		depth_2_doors[3].sprite = current[11];
		depth_2_doors[4].sprite = current[11];
		depth_2_doors[5].sprite = current[11];
		depth_2_doors[6].sprite = current[11];
		depth_2_doors[7].sprite = current[8];
		depth_2_doors[8].sprite = current[8];
		depth_2_doors[9].sprite = current[8];
		depth_2_doors[10].sprite = current[8];
		depth_2_doors[11].sprite = current[8];
		depth_2_doors[12].sprite = current[8];
		
		depth_2_exits[0].sprite = current[19];
		depth_2_exits[1].sprite = current[19];
		depth_2_exits[2].sprite = current[19];
		depth_2_exits[3].sprite = current[19];
		depth_2_exits[4].sprite = current[19];
		depth_2_exits[5].sprite = current[19];
		depth_2_exits[6].sprite = current[19];
		depth_2_exits[7].sprite = current[16];
		depth_2_exits[8].sprite = current[16];
		depth_2_exits[9].sprite = current[16];
		depth_2_exits[10].sprite = current[16];
		depth_2_exits[11].sprite = current[16];
		depth_2_exits[12].sprite = current[16];
	}

	void UpdateMinimap() {
		minimapViewed[cameraY][cameraX] = true;
		minimap[lastCameraY][lastCameraX] = new Color(1,1,1);
		minimap[cameraY][cameraX] = new Color(1,0,0);
		for(int i = 0; i < minimap.Count; ++i) {
			for(int j = 0; j < minimap[i].Count; ++j) {
				if(minimapViewed[i][j]) {
					minimapTex.SetPixel(i,j,minimap[i][j]);
				}
				else {
					minimapTex.SetPixel(i,j, frameBackground);
				}
			}
		}
		minimapTex.Apply();
		minimapImage.material.SetTexture("_MainTex", minimapTex);
		lastCameraX = cameraX;
		lastCameraY = cameraY;
	}
	
	// Update is called once per frame
	void Update () {
		if(typing.GetBattleState() != InputManager.BattleState.Started && typing.GetBattleState() != InputManager.BattleState.Gameovered
		 && typing.GetSignState() != InputManager.SignState.Started && !typing.levelFinished()) {			
			if(Input.GetButtonDown("Right")) {
				++dir;
				if(dir > Direction.West) {
					dir = Direction.North;
				}
				compass.text = directionText[(int) dir];
				CheckEncounters();
				MovementStuff();
				DisplayWalls();
			}
			if(Input.GetButtonDown("Left")) {
				--dir;
				if(dir < Direction.North) {
					dir = Direction.West;
				}
				compass.text = directionText[(int) dir];
				CheckEncounters();
				MovementStuff();
				DisplayWalls();
			}
			if(Input.GetButtonDown("Forward")) {
				int[] forwardXMovements = new int[] {0,1,0,-1};
				int[] forwardYMovements = new int[] {-1,0,1,0};
				int potentialCameraX = forwardXMovements[(int) dir];
				int potentialCameraY = forwardYMovements[(int) dir];
				if(isBlockAtPointRelative(potentialCameraX, potentialCameraY) != BlockType.Block) {
					if(isBlockAtPointRelative(potentialCameraX, potentialCameraY) == BlockType.Door) {
						gameinfo.door();
					}
					else gameinfo.move();
					cameraX += potentialCameraX;
					cameraY += potentialCameraY;
					UpdateMinimap();
					MovementStuff();
				}
				else if(isBlockAtPointRelative(potentialCameraX, potentialCameraY) == BlockType.Block) {
					gameinfo.mistake();
				}
				CheckEncounters();
				DisplayWalls();
				
			}
			if(Input.GetButtonDown("Backward")) {
				int[] forwardXMovements = new int[] {0,-1,0,1};
				int[] forwardYMovements = new int[] {1,0,-1,0};
				int potentialCameraX = forwardXMovements[(int) dir];
				int potentialCameraY = forwardYMovements[(int) dir];
				if(isBlockAtPointRelative(potentialCameraX, potentialCameraY) != BlockType.Block) {
					if(isBlockAtPointRelative(potentialCameraX, potentialCameraY) == BlockType.Door) {
						gameinfo.door();
					}
					else gameinfo.move();
					cameraX += potentialCameraX;
					cameraY += potentialCameraY;
					UpdateMinimap();
					MovementStuff();
				}
				else if(isBlockAtPointRelative(potentialCameraX, potentialCameraY) == BlockType.Block) {
					gameinfo.mistake();
				}
				CheckEncounters();
				DisplayWalls();
			}
		}	
		if(Input.GetButtonDown("Space") && gameinfo.isEnding(cameraY, cameraX)) {
			gameinfo.AdvanceFloor();
			LoadMap(true);
			typing.AdvanceCalc();
		}

	}

	void MovementStuff() {
		dust.Stop();
		dust.Play();
		fog.Stop();
		fog.Play();
		Sprite[] current = gameinfo.getSpriteset();
		++floorSprite;
		if(floorSprite > 2) floorSprite = 0;
		floor.sprite = current[floorSprite+24];
	}

	public void ResetGameover() {
		typing.GameoverRecover();
		LoadMap(false);
	}

	void CheckEncounters() {
		if(!gameinfo.isEnding(cameraY, cameraX)) {
			Encounter nextEncounter = gameinfo.encounterAt(cameraY, cameraX);
			Encounter nextHint = null;
			Sign nextSign = null;
			if(dir == Direction.North) {
				nextSign = gameinfo.signAt(cameraY-1, cameraX); 
				nextHint = gameinfo.encounterAt(cameraY-1, cameraX);
			}				
			else if(dir == Direction.East) {
				nextSign = gameinfo.signAt(cameraY, cameraX+1);
				nextHint = gameinfo.encounterAt(cameraY, cameraX+1);
			}
			else if(dir == Direction.West) {
				nextSign = gameinfo.signAt(cameraY, cameraX-1);
				nextHint = gameinfo.encounterAt(cameraY, cameraX-1);
			}
			else if(dir == Direction.South){
				nextSign = gameinfo.signAt(cameraY+1, cameraX);
				nextHint = gameinfo.encounterAt(cameraY+1, cameraX);
			}
			if(nextEncounter != null && !nextEncounter.isFinished()) {
				typing.BeginEncounter(nextEncounter);
			}
			else if(nextSign != null && !nextSign.isFinished()) {
				typing.BeginSign(nextSign);
			}
			else if(nextHint != null && !nextHint.isFinished()) {
				typing.BeginHint(nextHint.hint);
			}
			else {
				typing.ClearText();
			}
		}
		else {
			typing.CalcTotal();
		}
	}

	void DisplayWalls() {
		//depth 0
		depth_0_render(0, (isBlockAtPointRelative(rotatePointX(-1,-1), rotatePointY(-1, -1))));
		depth_0_render(1, isBlockAtPointRelative(rotatePointX(0, -1), rotatePointY(0, -1)));
		depth_0_render(2, isBlockAtPointRelative(rotatePointX(1, -1), rotatePointY(1, -1)));
		depth_0_render(3, isBlockAtPointRelative(rotatePointX(-1, 0), rotatePointY(-1, 0)));
		depth_0_render(4, isBlockAtPointRelative(rotatePointX(1, 0), rotatePointY(1, 0)));
		//depth 1
		depth_1_render(0, isBlockAtPointRelative(rotatePointX(-2, -2), rotatePointY(-2, -2)));
		depth_1_render(1, isBlockAtPointRelative(rotatePointX(-1, -2), rotatePointY(-1, -2)));
		depth_1_render(2, isBlockAtPointRelative(rotatePointX(0, -2), rotatePointY(0, -2)));
		depth_1_render(3, isBlockAtPointRelative(rotatePointX(1, -2), rotatePointY(1, -2)));
		depth_1_render(4, isBlockAtPointRelative(rotatePointX(2, -2), rotatePointY(2, -2)));
		depth_1_render(5, isBlockAtPointRelative(rotatePointX(-2, -1), rotatePointY(-2, -1)));
		depth_1_render(6, isBlockAtPointRelative(rotatePointX(-1, -1), rotatePointY(-1, -1)));
		depth_1_render(7, isBlockAtPointRelative(rotatePointX(1, -1), rotatePointY(1, -1)));
		depth_1_render(8, isBlockAtPointRelative(rotatePointX(2, -1), rotatePointY(2, -1)));
		//depth 2
		depth_2_render(0, isBlockAtPointRelative(rotatePointX(-3, -3), rotatePointY(-3, -3)));
		depth_2_render(1, isBlockAtPointRelative(rotatePointX(-2, -3), rotatePointY(-2, -3)));
		depth_2_render(2, isBlockAtPointRelative(rotatePointX(-1, -3), rotatePointY(-1, -3)));
		depth_2_render(3, isBlockAtPointRelative(rotatePointX(0, -3), rotatePointY(0, -3)));
		depth_2_render(4, isBlockAtPointRelative(rotatePointX(1, -3), rotatePointY(1, -3)));
		depth_2_render(5, isBlockAtPointRelative(rotatePointX(2, -3), rotatePointY(2, -3)));
		depth_2_render(6, isBlockAtPointRelative(rotatePointX(3, -3), rotatePointY(3, -3)));
		depth_2_render(7, isBlockAtPointRelative(rotatePointX(-3, -2), rotatePointY(-3, -2)));
		depth_2_render(8, isBlockAtPointRelative(rotatePointX(-2, -2), rotatePointY(-2, -2)));
		depth_2_render(9, isBlockAtPointRelative(rotatePointX(-1, -2), rotatePointY(-1, -2)));
		depth_2_render(10, isBlockAtPointRelative(rotatePointX(1, -2), rotatePointY(1, -2)));
		depth_2_render(11, isBlockAtPointRelative(rotatePointX(2, -2), rotatePointY(2, -2)));
		depth_2_render(12, isBlockAtPointRelative(rotatePointX(3, -2), rotatePointY(3, -2)));
	}

	void depth_0_render(int index, BlockType block) {
		depth_0[index].gameObject.SetActive(block == BlockType.Block);
		depth_0_doors[index].gameObject.SetActive(block == BlockType.Door);
		depth_0_signs[index].gameObject.SetActive(block == BlockType.Sign);
		depth_0_exits[index].gameObject.SetActive(block == BlockType.Exit);
	}
	void depth_1_render(int index, BlockType block) {
		depth_1[index].gameObject.SetActive(block == BlockType.Block);
		depth_1_doors[index].gameObject.SetActive(block == BlockType.Door);
		depth_1_signs[index].gameObject.SetActive(block == BlockType.Sign);
		depth_1_exits[index].gameObject.SetActive(block == BlockType.Exit);
	}
	void depth_2_render(int index, BlockType block) {
		depth_2[index].gameObject.SetActive(block == BlockType.Block);
		depth_2_doors[index].gameObject.SetActive(block == BlockType.Door);
		depth_2_signs[index].gameObject.SetActive(block == BlockType.Sign);
		depth_2_exits[index].gameObject.SetActive(block == BlockType.Exit);
	}

	int rotatePointX(int x, int y) {
		float theta = 0;
		if(dir == Direction.North) {
			theta = 0;
		}
		if(dir == Direction.East) {
			theta = 90;
		}
		if(dir == Direction.South) {
			theta = 180;
		}
		if(dir == Direction.West) {
			theta = 270;
		}
		theta *=Mathf.PI/180;
		int newX = (int) Mathf.RoundToInt(x*Mathf.Cos(theta) - y*Mathf.Sin(theta));
		return newX;
	}
	int rotatePointY(int x, int y) {
		float theta = 0;
		if(dir == Direction.North) {
			theta = 0;
		}
		if(dir == Direction.East) {
			theta = 90;
		}
		if(dir == Direction.South) {
			theta = 180;
		}
		if(dir == Direction.West) {
			theta = 270;
		}
		theta *=Mathf.PI/180;
		int newY = (int) Mathf.RoundToInt(x*Mathf.Sin(theta) + y*Mathf.Cos(theta));
		return newY;
	}

	BlockType isBlockAtPointRelative(int x, int y) {
		int pointX = cameraX + x;
		int pointY = cameraY + y;
		if(pointX < 0 || pointY < 0 || pointY > mapGrid.Count - 1 || pointX > mapGrid[0].Count - 1) {
			return BlockType.Block;
		}
		return blockGrid[pointY][pointX];
	}
}
