using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/* ***************************** **
**   written by Florian Siemer
**        July 2015
** ***************************** */

public class HighscoreEntry : MonoBehaviour {
	public GameObject highscore;
	public GameObject inputField;
	public GameObject name;
	
	private List<int> topTenScore = new List<int>();
	private List<string> topTenNames = new List<string>();
	private int topTenCount = 10;
	private bool entryFinished = false;
	private int candidateScore = 0;
	private int candidateAt = -1;
	
	// components:
	private Main compoMain;
	private Text highscoreText;
	private Text nameText;
	
	// ******************************************** START/ UPDATE ********************************************
	void Awake() {
		// find components:
		compoMain = Camera.main.GetComponent<Main>();
		highscoreText = highscore.GetComponent<Text>();
		nameText = name.GetComponent<Text>();
	}
	
	void Start() {
		for(int i=0; i<topTenCount; ++i) {
			topTenScore.Add(  PlayerPrefs.GetInt("tts"+i, 0)  ); 
			topTenNames.Add(  PlayerPrefs.GetString("ttn"+i, "-")  ); 
		}
	}
	
	// ******************************************** HIGHSCORE ********************************************
	public void CheckHighscore(int newScore) {
		for(int i=0; i<topTenScore.Count; ++i) {
			if(newScore > topTenScore[i]) {
				candidateScore = newScore;
				candidateAt = i;
				
				return;
			}
		}
		
		// score too low:
		entryFinished = true;
		ShowHighscore();
	}
	
	private void ShowHighscore() {
		inputField.SetActive(false); // hide input field
		
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		
		sb.Append("Highscore:\n\n");
		for(int i=0; i<topTenCount; ++i) {
			sb.Append("#");
			sb.Append((i+1));
			sb.Append(": ");
			sb.Append(topTenNames[i]);
			sb.Append(", <color=#884400ff>");
			sb.Append(topTenScore[i]);
			sb.Append("</color> points\n");
		}
		sb.Append("\nPress escape to restart");
		
		highscoreText.text = sb.ToString();
	}
	
	public void EnterHighscoreName() {
		if(nameText.text.Length <= 0 || candidateAt < 0 || entryFinished) return;
		entryFinished = true;
		
		topTenScore.Insert(candidateAt, candidateScore);
		topTenNames.Insert(candidateAt, nameText.text);
		
		SaveHighscore();
		ShowHighscore();
	}
	
	private void SaveHighscore() {
		for(int i=0; i<topTenCount; ++i) {
			PlayerPrefs.SetInt("tts"+i, topTenScore[i]); 
			PlayerPrefs.SetString("ttn"+i, topTenNames[i]); 
		}
	}
	
}
