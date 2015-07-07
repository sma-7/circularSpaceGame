using UnityEngine;
using System.Collections;

/* ***************************** **
**   written by Florian Siemer
**        July 2015
** ***************************** */

public class Star : MonoBehaviour {
	// components:
	private ObjectMovement compoMove;
	
	void Awake() {
		// find components:
		compoMove = GetComponent<ObjectMovement>();
	}
	
	void OnEnable() {
		if(compoMove) compoMove.UpdateAngleDegree(  Random.Range(0.01f, 360f)  );
	}
}
