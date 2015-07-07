using UnityEngine;
using System.Collections;

/* ***************************** **
**   written by Florian Siemer
**        July 2015
** ***************************** */

public class PlayerInput : MonoBehaviour {
	public bool InputLeft() {
		if(Input.GetKey("left")) return true;
		
		return false;
	}
	
	public bool InputRight() {
		if(Input.GetKey("right")) return true;
		
		return false;
	}
	
	public bool Fire() {
		if(Input.GetKey("space")) return true;
		if(Input.GetMouseButton(0)) return true;
		
		return false;
	}
}
