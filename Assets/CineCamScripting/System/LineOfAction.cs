using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomVariables;

//SUPPORTS 2 ACTORS IN THE SCENE

public class LineOfAction : MonoBehaviour {


	Dictionary<string, Side> actorSideDict;
	public Side SceneSide;
	//used in CameraShot class to access 
	public static bool isSceneLeft;


	//Input main Camera Side into thing
	//Make opposite actor the opposite side
	public void associateActorsWithSide(Side s)
	{
	  SceneSide = s;

	  //accessed in CamerShot constructor - affects biasX
	  isSceneLeft = (SceneSide == Side.Left);

	  List<string> actors = GetComponent<ScriptParser> ().actors;
	  actorSideDict = new Dictionary<string, Side> ();
	 
		actorSideDict.Add (actors[0], s);

		//Flip sides
		if (s == Side.Left) {
			s = Side.Right;
		} 
		else if (s == Side.Right) {
			s = Side.Left; 
		}

		actorSideDict.Add (actors [1], s); 
	} 

	//Called from Database->getShotList
	public Side getSide(string name)
	{
		return actorSideDict [name];
	} 
}
