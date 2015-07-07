using UnityEngine;
using System.Collections;

/* ***************************** **
**   written by Florian Siemer
**        July 2015
** ***************************** */

public class Main : MonoBehaviour {	
	// public:
	public GameObject ingameParent;
	public GameObject starParent;
	public GameObject livesDisplay;
	public GameObject scoreDisplay;
	public GameObject canvas;
	public bool gameActive = true;
	
	// stars:
	private float loopStarInterval = 0.15f;
	private float loopEnemyMinInterval = 2.6f;
	private float loopEnemyMaxInterval = 4.4f;
	private int starsPerTick = 5;
	private int enemiesMinPerTick = 2;
	private int enemiesMaxPerTick = 3;
	private int score = 0;
	private int lives = 5;
	
	// components:
	private Pool compoPool;
	private HighscoreEntry compoHS;
	private TextMesh compoScoreTextMesh;
	private TextMesh compoLivesTextMesh;
	
	// ******************************************** START/ UPDATE ********************************************
	void Awake() {
		// find components:
		compoPool = Camera.main.GetComponent<Pool>();
		compoHS = Camera.main.GetComponent<HighscoreEntry>();
		compoScoreTextMesh = scoreDisplay.GetComponent<TextMesh>();
		compoLivesTextMesh = livesDisplay.GetComponent<TextMesh>();
		
		Application.targetFrameRate = 90;
	}
	
	void Start() {
		gameActive = true;
		canvas.SetActive(false); // hide canvas
		
		// emulate StarLoop for 8 seconds:
		InitializeStarsForSeconds(8);
		
		Physics.IgnoreLayerCollision(8,  8,  true); // ignore player vs player
		Physics.IgnoreLayerCollision(9,  9,  true); // ignore enemy vs enemy
		
		UpdateScoreText();
		UpdateLivesText();
		
		// start loops:
		StartCoroutine(StarLoop());
		StartCoroutine(EnemySpawnLoop());
	}
	
	void Update() {
		if(gameActive) return;
		
		if(Input.GetKeyDown("enter") || Input.GetKeyDown("return")) compoHS.EnterHighscoreName();
		if(Input.GetKeyDown("escape")) RestartGame();
	}
	
	// ******************************************** LOOPS ********************************************
	IEnumerator StarLoop() {
		while(true) {
			yield return new WaitForSeconds(loopStarInterval); // wait
			
			for(int i=0; i<starsPerTick; ++i) CreateStar(0);
		}
	}
	
	IEnumerator EnemySpawnLoop() {
		while(gameActive) {
			float wait = Random.Range(loopEnemyMinInterval, loopEnemyMaxInterval);
			
			yield return new WaitForSeconds(wait); // wait
			
			if(!gameActive) break;
			
			int enemyCount = Random.Range(enemiesMinPerTick, enemiesMaxPerTick+1); // +1, because int excludes the last element
			SpawnEnemies(enemyCount);
			
			// recude enemy interval time:
			if(loopEnemyMinInterval > 1f) loopEnemyMinInterval -= 0.1f;
			if(loopEnemyMaxInterval > 2f) loopEnemyMaxInterval -= 0.15f;
		}
	}
	
	// ******************************************** OBJECTS ********************************************
	// create object raw, ignore pooling:
	public GameObject CreateObjRaw(GameObject parent_obj, string prefab_name, Vector3 position) {
		GameObject new_obj = null;
		
		try {
			new_obj = (GameObject) MonoBehaviour.Instantiate(  Resources.Load(prefab_name, typeof(GameObject))  );
		}
		catch(System.SystemException e) {
			print("[ERROR] Can't instantiate '"+prefab_name+"': "+e);
			return null;
		}
		
		ObjectSetParentAndPosition(new_obj, parent_obj, position);
		
		new_obj.name = prefab_name;
		
		return new_obj;
	}
	
	// create object, respect pooling:
	public GameObject CreateObj(GameObject parent_obj, string prefab_name, Vector3 position) {
		if(compoPool) {
			GameObject obj = compoPool.Create(prefab_name);
			if(obj != null) {
				obj.SetActive(true);
				
				ObjectSetParentAndPosition(obj, parent_obj, position);
				
				return obj;
			}
		}
		
		return CreateObjRaw(parent_obj, prefab_name, position);
	}
	
	// remove object, respect pooling:
	public void RemoveObject(GameObject obj) {
		if(compoPool && compoPool.Remove(obj)) return;
		
		Destroy(obj);
	}
	
	private void ObjectSetParentAndPosition(GameObject obj, GameObject parent_obj, Vector3 position) {
		obj.transform.position = position;
		
		// add child:
		if(parent_obj != null) obj.transform.parent = parent_obj.transform;
	}
	
	// ******************************************** GENERAL ********************************************
	public void PlayerDies() {
		lives -= 1;
		UpdateLivesText();
		
		if(lives <= 0) GameOver();
	}
	
	private void GameOver() {
		gameActive = false;
		canvas.SetActive(true); // show canvas
		
		compoHS.CheckHighscore(score);
	}
	
	private void RestartGame() {
		Application.LoadLevel(Application.loadedLevelName);
	}
	
	// ******************************************** TEXT/ SCORE/ LIVES ********************************************
	public void IncreaseScore(int bonus) {
		if(!gameActive) return;
		
		score += bonus;
		UpdateScoreText();
	}
	
	private void UpdateScoreText() {
		compoScoreTextMesh.text = score.ToString();
	}
	
	private void UpdateLivesText() {
		if(lives > 0) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for(int i=0; i<lives; ++i) sb.Append("* ");
			compoLivesTextMesh.text = sb.ToString();
		}
		else compoLivesTextMesh.text = "";
	}
	
	// ******************************************** STARS ********************************************
	private void CreateStar(float moveTime) {
		float randomZ = Random.Range(100f, 200f);
		Vector3 createPosition = new Vector3(0, 0, randomZ);
		
		GameObject newStar = CreateObj(starParent, "Star", createPosition);
		
		if(moveTime <= 0) return;
		
		ObjectMovement compoStar = newStar.GetComponent<ObjectMovement>();
		if(compoStar) compoStar.MoveObject(moveTime);
	}
	
	// InitializeStarsForSeconds emulates the StarLoop() for a given amount of time:
	private void InitializeStarsForSeconds(float sec) {
		int emulateSteps = (int)(sec/loopStarInterval);
		
		if(emulateSteps <= 0) return;
		
		for(int k=0; k<emulateSteps; ++k) {
			float moveTime = (float)k * loopStarInterval;
			
			for(int i=0; i<starsPerTick; ++i) CreateStar(moveTime);
		}
	}
	
	// ******************************************** ENEMIES ********************************************
	private void SpawnEnemies(int count) {
		if(count <= 0) return;
		
		float randomAngle = Random.Range(0.01f, 360f);
		
		for(int i=0; i<count; ++i) {
			Vector3 createPosition = new Vector3(0, 0, 0);
			
			GameObject newEnemy = CreateObj(ingameParent, "EnemyShip", createPosition);
			
			float newAngle = randomAngle + (float)i*30;
			
			ShipMovement compoShip = newEnemy.GetComponent<ShipMovement>();
			if(compoShip) compoShip.UpdateAngleDegree(newAngle);
		}
	}
}
