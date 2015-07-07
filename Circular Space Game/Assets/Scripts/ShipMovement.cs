using UnityEngine;
using System.Collections;

/* ***************************** **
**   written by Florian Siemer
**        July 2015
** ***************************** */

public class ShipMovement : MonoBehaviour {
	// public vars:
	public bool playerInput = true;
	public float circleDistanceX = 5f;
	public float circleDistanceY = 5f;
	public float speed = 2f; // radians
	public float startAngleDegree = 270; // degree
	public float baseShotCooldown = 0.2f;
	public string shotPrefabName;
	public int scoreBonus = 100;
	
	// private vars:
	private float angle; // radians
	private bool moveLeft = false;
	private bool moveRight = false;
	private bool fire = false;
	private float shotCD = 0;
	private bool isDead = false;
	
	// components:
	private Transform tform;
	private Main compoMain;
	private PlayerInput compoInput;
	
	// ******************************************** START/ UPDATE ********************************************
	void Awake() {
		// find components:
		tform = transform;
		compoMain = Camera.main.GetComponent<Main>();
		compoInput = GetComponent<PlayerInput>();
	}
	
	void OnEnable() {
		isDead = false;
		
		// initialize:
		angle = StaticFunctions.DegreeToRadians(startAngleDegree);
		Move(0); // move to the start position (angle)
		
		// ini cooldown:
		if(tag == "Enemy") shotCD = Random.Range(baseShotCooldown/10, baseShotCooldown);
		else shotCD = baseShotCooldown;
		
		// no player input -> initialize enemy AI:
		if(!compoInput) IniEnemyAI();
		
		StartCoroutine(FadeIn(0.6f));
	}
	
	void Update() {
		// game over -> Kill():
		if(!compoMain.gameActive) {
			Kill();
			return;
		}
		
		CheckPlayerInput();
		UpdateMovement(Time.deltaTime);
		FireShots(Time.deltaTime);
	}
	
	// ******************************************** MOVEMENT ********************************************	
	private void UpdateMovement(float deltaTime) {
		// no movement if both directions are false or true:
		if(moveLeft == moveRight) return;
		
		Move(deltaTime);
	}
	
	private void Move(float deltaTime) {
		float directionFactor = (moveRight) ? 1 : -1;
		float changeAngle = speed * directionFactor * deltaTime;
		
		angle += changeAngle;
		
		float newX = Mathf.Cos(angle) * circleDistanceX;
		float newY = Mathf.Sin(angle) * circleDistanceY;
		
		Vector3 newPos = new Vector3(newX, newY, tform.position.z);
		tform.position = newPos;
	}
	
	public void UpdateAngleDegree(float newAngleDegree) {
		angle = StaticFunctions.DegreeToRadians(newAngleDegree);
		Move(0);
	}
	
	// ******************************************** SHIP ********************************************
	public void Kill() {
		if(isDead) return;
		
		isDead = true;
		moveLeft = false;
		moveRight = false;
		fire = false;
		
		StartCoroutine(FadeOutAndRemove(0.3f));
	}
	
	IEnumerator FadeIn(float duration) {
		float time = duration;
		
		while(time > 0) {
			time -= Time.deltaTime;
			
			tform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time/duration);
			
			yield return 1;
		}
	}
	
	IEnumerator FadeOutAndRemove(float duration) {
		float time = duration;
		
		while(time > 0) {
			time -= Time.deltaTime;
			
			tform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time/duration);
			
			yield return 1;
		}
		
		compoMain.RemoveObject(gameObject);
	}
	
	// ******************************************** SHOTS ********************************************
	private void FireShots(float deltaTime) {
		// decrease cooldown:
		shotCD -= deltaTime;
		
		if(!compoMain || !fire || shotCD > 0 || isDead) return;
		
		shotCD = baseShotCooldown; // apply cooldown
		
		GameObject newShot = compoMain.CreateObj(compoMain.ingameParent, shotPrefabName, tform.position);
		
		ObjectMovement compoShotMove = newShot.GetComponent<ObjectMovement>();
		if(compoShotMove) compoShotMove.UpdateAngleRadians(angle);
	}
	
	// ******************************************** COLLISION ********************************************
	void OnTriggerEnter(Collider other) {
		if(isDead) return;
		if(other.tag != "Shot") return;
		
		BeenHitByShot(other);
	}
	
	private void BeenHitByShot(Collider other) {
		if(tag == "Enemy") {
			Kill();
			compoMain.RemoveObject(other.gameObject);
			compoMain.IncreaseScore(scoreBonus);
		}
		else if(tag == "Player") {
			compoMain.RemoveObject(other.gameObject);
			compoMain.PlayerDies();
		}
	}
	
	// ******************************************** PLAYER INPUT ********************************************
	private void CheckPlayerInput() {
		if(!compoInput || isDead) return;
		
		moveLeft = compoInput.InputLeft();
		moveRight = compoInput.InputRight();
		fire = compoInput.Fire();
	}
	
	// ******************************************** ENEMY AI ********************************************
	private void IniEnemyAI() {
		moveRight = true;
		fire = true;
	}
}
