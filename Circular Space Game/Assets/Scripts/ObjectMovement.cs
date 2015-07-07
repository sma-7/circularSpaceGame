using UnityEngine;
using System.Collections;

/* ***************************** **
**   written by Florian Siemer
**        July 2015
** ***************************** */

public class ObjectMovement : MonoBehaviour {
	// public vars:
	public float speed = 20f;
	public float moveZ = -10f;
	public float angleDegree = 0;
	public float rotateZ = 0;
	public bool killOutOfBounds = true;
	public bool killNearCenter = false;
	
	// private vars:
	private float moveX = 0;
	private float moveY = 0;
	private float epsylon = 0.2f;
	private float outOfBoundsX = 250f;
	private float outOfBoundsY = 150f;
	
	// components:
	private Transform tform;
	private Main compoMain;
	
	// ******************************************** START/ UPDATE ********************************************
	void Awake() {
		// find components:
		tform = transform;
		compoMain = Camera.main.GetComponent<Main>();
		
		UpdateAngleDegree(angleDegree);
	}
	
	// Update is called once per frame
	void Update() {
		MoveObject(Time.deltaTime);
		RotateObject(Time.deltaTime);
	}
	
	// ******************************************** GENERAL ********************************************
	public void UpdateAngleDegree(float newAngleDegree) {
		angleDegree = newAngleDegree;
		float radians = StaticFunctions.DegreeToRadians(angleDegree);
		moveX = Mathf.Cos(radians) * speed;
		moveY = Mathf.Sin(radians) * speed;
	}
	
	public void UpdateAngleRadians(float newAngle) {
		float radians = newAngle;
		moveX = Mathf.Cos(radians) * speed;
		moveY = Mathf.Sin(radians) * speed;
	}
	
	public void MoveObject(float deltaTime) {
		float newX = tform.position.x + moveX * deltaTime;
		float newY = tform.position.y + moveY * deltaTime;
		float newZ = tform.position.z + moveZ * deltaTime;
		
		if(IsOutOfGameSpace(newX, newY)) return;
		
		Vector3 newPos = new Vector3(newX, newY, newZ);
		tform.position = newPos;
	}
	
	public void RotateObject(float deltaTime) {
		if(rotateZ == 0) return;
		
		Vector3 newRot = new Vector3(tform.eulerAngles.x, tform.eulerAngles.y, tform.eulerAngles.z + rotateZ * deltaTime);
		tform.eulerAngles = newRot;
	}
	
	private bool IsOutOfGameSpace(float newX, float newY) {
		// kill near center:
		if(killNearCenter) {
			if(newX-epsylon < 0 && newX+epsylon > 0 && newY-epsylon < 0 && newY+epsylon > 0) {
				compoMain.RemoveObject(gameObject);
				return true;
			}
		}
		
		// kill out of bounds:
		if(killOutOfBounds) {
			if((moveX > 0 && newX >= outOfBoundsX) || (moveX < 0 && newX <= -outOfBoundsX) || (moveY > 0 && newY >= outOfBoundsY) || (moveY < 0 && newY <= -outOfBoundsY)) {
				compoMain.RemoveObject(gameObject);
				return true;
			}
		}
		
		return false;
	}
}
