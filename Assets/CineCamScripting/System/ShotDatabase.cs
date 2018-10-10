using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomVariables;

[ExecuteInEditMode]
[System.Serializable]
public class ShotDatabase : MonoBehaviour {

	//KEY maps to shot list
	//KEY = GOAL
	[SerializeField]
	public Dictionary<string, ShotDefinition> defLibrary;

    public class ShotDefinition
	{
		public ClassType ClassName;
		public string goal;
		public float dist;
		public float H;
		public float O;
		public float bX;

		public ShotDefinition(ClassType cl, string go, float d, float h, float o, float b)
		{
			ClassName = cl;
			goal = go;
			dist = d;
			H = h;
			O = o;
			bX = b;
		}
	}

	//Look up the goal, recalculate around the actor
	public CameraShot ShotGet(string goal, string actor)
	{
		Vector3 marker = GetComponent<LineOfAction>().getSide();
		
		if(defLibrary[goal].ClassName == ClassType.CameraShot){
			return new CameraShot(goal, marker, defLibrary[goal].dist, defLibrary[goal].H, defLibrary[goal].O, defLibrary[goal].bX, actor);
		}
		else if(defLibrary[goal].ClassName == ClassType.FrameShare){
			return new FrameShare(goal, marker, defLibrary[goal].dist, defLibrary[goal].H, defLibrary[goal].O, defLibrary[goal].bX, actor, findOpposite(actor));
		}

		else if(defLibrary[goal].ClassName == ClassType.OverShoulder){
			return new OverShoulder(goal, marker, defLibrary[goal].dist, defLibrary[goal].H, defLibrary[goal].O, defLibrary[goal].bX, actor, findOpposite(actor));
		}
		else{
			    Debug.Log("SHOULD NOT GET HERE IN SHOTGET");
				return null;
			}
	}

	public void AddShotToLibrary(ClassType type, string goal, float Dist, float H, float O, float Bx)
	{
		if(type == ClassType.CameraShot)
		{
			defLibrary.Add(goal, new ShotDefinition(ClassType.CameraShot, goal, Dist, H, O, Bx));
		}
		else if(type == ClassType.FrameShare)
		{
			defLibrary.Add(goal, new ShotDefinition(ClassType.FrameShare, goal, Dist, H, O, Bx));
		}
		else if(type == ClassType.OverShoulder)
		{
			defLibrary.Add(goal, new ShotDefinition(ClassType.OverShoulder, goal, Dist, H, O, Bx));
		}
	}

	public void SetUpDatabase()
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

		defLibrary = new Dictionary<string, ShotDefinition>();
		defLibrary.Add("Default", new ShotDefinition (ClassType.CameraShot, "Default", 2.0f, 0.0f, 30.0f, -0.5f));
		defLibrary.Add("Default/Mid", new ShotDefinition(ClassType.CameraShot, "Default/Mid", 1.5f, 0.0f, 30.0f, -0.5f));
		defLibrary.Add("Default/Close", new ShotDefinition(ClassType.CameraShot, "Default/Close", 1.5f, 0.0f, 30.0f, -0.5f));
		defLibrary.Add("Default/XtremeClose", new ShotDefinition(ClassType.CameraShot, "Default/XtremeClose", 0.5f, 0.0f, 30.0f, -0.5f));
		defLibrary.Add ("HighAngle", new ShotDefinition (ClassType.CameraShot,"HighAngle",  2.0f, 4.0f, 45.0f, 0.0f));
		defLibrary.Add ("LowAngle", new ShotDefinition (ClassType.CameraShot, "LowAngle", 2.0f, -1.0f, 45.0f, 0.0f));
		defLibrary.Add("Parallel", new ShotDefinition (ClassType.CameraShot, "Parallel", 2.0f, 0.0f, 90.0f, 0.0f)); 

		defLibrary.Add("FrameShare", new ShotDefinition (ClassType.FrameShare, "FrameShare", 5.0f, 1.0f, 0.0f, 0.0f));
		defLibrary.Add("OverShoulder", new ShotDefinition (ClassType.OverShoulder, "OverShoulder", 2.14f, 0.1f, 140.0f, 0.0f));

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
}
