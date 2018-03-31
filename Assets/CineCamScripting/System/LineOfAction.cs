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

	public GameObject markerRight;
	public GameObject markerLeft;


	Vector3 SideMarker;
	//Sets left and right markers
	//markers determine 
	//	1) where camera orbits
	//  2) biasX is - or +
	public void SetSide(Side s)
	{

		markerRight = new GameObject ();
		markerLeft = new GameObject ();

		List<string> actors = GetComponent<ScriptParser> ().actors;

		//Setting sides supports only 2 characters at this point
		if(actors.Count == 2)
		{
			GameObject targetObj1 = GameObject.Find(actors[0]);
			GameObject targetObj2 = GameObject.Find(actors[1]);
			float actorDistance = Vector3.Distance (targetObj1.transform.position, targetObj2.transform.position);
			Vector3 actorADirN =  (targetObj2.transform.position - targetObj1.transform.position).normalized;
			Vector3 MidPoint = targetObj1.transform.position + (actorADirN * (actorDistance / 2));

			Vector3 PDirRight = Quaternion.AngleAxis(90, Vector3.up) * actorADirN;
			Vector3 PDirLeft = Quaternion.AngleAxis(-90, Vector3.up) * actorADirN;

			markerRight.transform.position = MidPoint + (PDirRight * 10);
			markerLeft.transform.position = MidPoint + (PDirLeft * 10);

			if (s == Side.Right) {
				SideMarker = markerRight.transform.position;
			} else {
				SideMarker = markerLeft.transform.position;
			}
		}
	} 

	//Called from Database->getShotList
	public Vector3 getSide()
	{
		return SideMarker;
	} 
}
