using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomVariables;

[System.Serializable]
public class ShotDatabase : MonoBehaviour {

	//KEY maps to shot list
	//KEY = GOAL
	[SerializeField]
	public Dictionary<string, List<CameraShot>> shotsLibrary;

	//INPUT: List of Goals, ActorID saying Dialogue
	//OUTPUT: List of finalized shots correlating to goal
	//Called from ResetShots()
	//TODO SHOT RANKING - OCCLUSION
	public List<CameraShot> getShotList(List<string> goals, string actor)
	{
		CalculateShotsAround (actor);

		//Instantiate list
		List<CameraShot> finalizedShotList = new List<CameraShot> ();

		//For every GOAL in the DIALOGUE
		for (int i = 0; i < goals.Count; i++) {
			
			//Get list of every possible position for that GOAL
			List<CameraShot> shotList = shotsLibrary [goals [i]];
			//Do Occulsiion Ranking
			finalizedShotList.Add(shotList[0]);
		}
		//Return list finalized shot for every goal
		return finalizedShotList;
	}

	//ASSOCIATE GOALS WITH SHOTS
	//Defines shot positions for every GOAL
	public void CalculateShotsAround(string actor)
	{

		/*Camera Shot Paramters
		  --G - Type of shot
		  --Marker - Left or Right
		  --Dist - Distance from target
		  --H - Height change, used for high and low angles
		  --O - Degrees of orbit around target
		  --Bx - Bias shift on X axsis
		  --A - Actor that is target being looked at
		  --Op - Opposite Actor that is included in shot

        */

		//Recalculate Shots with side for specific actor
		LineOfAction LOA_decider = GetComponent<LineOfAction>();
		Vector3 marker = LOA_decider.getSide ();

		shotsLibrary = new Dictionary<string, List<CameraShot>> ();
		//Camera Shot(SIDE, DISTANCE, HEIGHT, OrbitAngle)
		//DEFAULT
		List<CameraShot> defaultList = new List<CameraShot> ();
		defaultList.Add (new CameraShot ("Default", marker, 2.0f, 0.0f, 30.0f, -0.5f, actor));
		defaultList.Add (new CameraShot ("Default", marker, 2.0f, 0.0f, 45.0f, 0.0f, actor));
		shotsLibrary.Add("Default", defaultList); 

		List<CameraShot> highAngleList = new List<CameraShot> ();
		highAngleList.Add (new CameraShot ("HighAngle", marker, 2.0f, 4.0f, 45.0f, 0.0f, actor));
		highAngleList.Add (new CameraShot ("HighAngle", marker, 2.0f, 1.0f, 0.0f, 0.0f, actor));
		shotsLibrary.Add ("HighAngle", highAngleList);

		List<CameraShot> lowAngleList = new List<CameraShot> ();
		highAngleList.Add (new CameraShot ("LowAngle", marker, 2.0f, 4.0f, 45.0f, 0.0f, actor));
		highAngleList.Add (new CameraShot ("LowAngle", marker, 2.0f, 1.0f, 0.0f, 0.0f, actor));
		shotsLibrary.Add ("LowAngle", lowAngleList);

		List<CameraShot> frameShareList = new List<CameraShot> ();
		frameShareList.Add( new CameraShot ("FrameShare", marker, 5.0f, -1.0f, 45.0f, 0.0f, actor, findOpposite (actor)));
		frameShareList.Add( new CameraShot ("FrameShare", marker, 2.0f, 1.0f, 0.0f, 0.0f, actor, findOpposite (actor)));
		shotsLibrary.Add ("FrameShare", frameShareList);
	}


	//IF 2 ACTORS IN Scene finds opposite actor for include shots
	public string findOpposite(string actor)
	{
		//IF THERE ARE 2 ACTORS IN Scene
		List<string> actorsList = GetComponent<ScriptParser> ().actors;
		if (actorsList.Count == 2) {
			if (actor == actorsList [0]) {
				return actorsList [1];
			} else {
				return actorsList [0];
			}
		} else {
			//TODO: SHOOT RAYCAST FORWARD SEE WHO IT HITS
			//Returns original actor
			return actor;
		}
	}

	//FOR SAVING AND ADDING SHOTS 
	public void AddShot(string NAME, Vector3 pos, Quaternion rot)
	{
		//Goal thing = Goal (NAME);
	}

}
