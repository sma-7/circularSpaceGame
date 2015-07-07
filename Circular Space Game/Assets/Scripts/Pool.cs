using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ***************************** **
**   written by Florian Siemer
**        July 2015
** ***************************** */

public class Pool : MonoBehaviour {
	// public vars:
	public string[] poolPaths = new string[0];
	public int[] poolSizes = new int[0];
	public GameObject parentPool;
	
	// private vars:
	private int defaultPoolSize = 30;
	private List<Queue<GameObject>> Qs = new List<Queue<GameObject>>();
	private Vector3 defaultPosition = new Vector3(-1000, -1000, 0);
	
	// components:
	private Main compoMain;
	
	// ******************************************** START/ UPDATE ********************************************
	void Awake() {
		// find components:
		compoMain = Camera.main.GetComponent<Main>();
		
		// create before Start():
		if(poolPaths.Length > 0) {
			// create a pool for every prefab in 'poolPaths':
			for(int i=0; i<poolPaths.Length; ++i) {
				Queue<GameObject> newQ = new Queue<GameObject>();
				Qs.Add(newQ);
				
				int pSize = defaultPoolSize;
				if(i < poolSizes.Length) pSize = poolSizes[i];
				
				IniPool(poolPaths[i], newQ, pSize);
			}
		}
	}
	
	// ******************************************** POOL MANAGEMENT ********************************************
	private void IniPool(string prefabName, Queue<GameObject> Q, int pSize) {
		if(pSize < 1) return;
		
		for(int i=0; i<pSize; ++i) {
			GameObject new_obj = compoMain.CreateObjRaw(parentPool, prefabName, defaultPosition);
			new_obj.SetActive(false);
			Q.Enqueue(new_obj);
		}
	}
	
	private GameObject IncreasePoolByOne(int i) {
		if(i < 0 || i >= poolPaths.Length) return null;
		
		string prefabName = poolPaths[i];
		GameObject new_obj = compoMain.CreateObjRaw(parentPool, prefabName, defaultPosition);
		new_obj.SetActive(false);
		
		return new_obj;
	}
	
	private int GetPoolIndex(string prefabName) {
		int index = -1;
		
		for(int i=0; i<poolPaths.Length; ++i) {
			++index;
			
			if(poolPaths[i] == prefabName) return index;
		}
		
		return -1;
	}
	
	// ******************************************** OBJECT MANAGEMENT ********************************************
	public GameObject Create(string prefabName) {
		int index = GetPoolIndex(prefabName);
		if(index < 0 || index >= poolPaths.Length) return null;
		
		Queue<GameObject> Q = Qs[index];
		GameObject obj;
		
		if(Q.Count <= 0) obj = IncreasePoolByOne(index);
		else obj = Q.Dequeue();
		
		return obj;
	}
	
	public bool Remove(GameObject obj) {
		if(obj == null) return false;
		
		int index = GetPoolIndex(obj.name);
		if(index < 0 || index >= poolPaths.Length) return false;
		
		Queue<GameObject> Q = Qs[index];
		
		obj.SetActive(false);
		obj.transform.parent = parentPool.transform;
		Q.Enqueue(obj);
		
		return true;
	}
}
