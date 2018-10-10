using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomVariables;

/*
 * Contains the following:
 * ActorID - which character
 * DialogText - text of the dailog
 * CameraShots - Shots or Shot of the dialog
*/

[System.Serializable]
public class Dialogue {


	//Should be the same length as shotSequence...
	public List<string> goals;

	public string dialogText;
	public string ActorID;

	public AudioClip clip;
	//Calculate shot time from audio clip length?
	public float shotSeconds;



	public List<CameraShot> shotSequence;


	//Constructor
	//ONLY TAKES 1 GOAL AT A TIME
	//TODO
	public Dialogue(string dialog, string id, List<string> gs)
	{
		dialogText = dialog;
		ActorID = id;
		goals = gs;
		shotSequence = new List<CameraShot> ();
	}
		
	public void SetSequence(List<CameraShot> camlist)
	{
		shotSequence = camlist;

	}
}
