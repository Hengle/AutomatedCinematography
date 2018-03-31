using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomVariables;

namespace CustomVariables
{
//public enum Goal {Default, DefaultInclude, Previous, Intensify, Close, FrameShare, Medium, Long, LowAngle, HighAngle, ReverseCheck};

public enum Side {Left, Right};

	public class SideDir{
		public Side side;
		public Vector3 dir;
		SideDir(Side s, Vector3 d)
		{
			dir = d;
			side = s;
		}
	}


	public class CameraShot {
    
		public string goal;
		public Vector3 sidemarker; //Can be {left, right}
		public float distanceFromTarget;
		public float height; // 0 is dead on - Can be 5 or -5
		public float orbitAngle;
		public float shotDuration;
		public float biasX;
		public string actor;
		public string oppositeActor; //For Frame Including

		//Used to the above variables to calculate the following
		//Position in the scene relative to actor
		public Vector3 CamPos;
		public Quaternion CamRot;

		//No Actor calculations required
		//Called when manually setting shot
		public CameraShot(Vector3 pos, Quaternion rot)
		{
			CamPos = pos;
			CamRot = rot;
		}

		//For 
		public Vector3 GetClosest(Vector3 m, Vector3 o1, Vector3 o2)
		{
			if (Vector3.Distance (m, o1) < Vector3.Distance (m, o2)) {
				return o1;
			} else {
				return o2;
			}
		}

		//For Bias Calculation
		public Vector3 GetFarthest(Vector3 m, Vector3 o1, Vector3 o2)
		{
			if (Vector3.Distance (m, o1) > Vector3.Distance (m, o2)) {
				return o1;
			} else {
				return o2;
			}
		}

    	//Focused on only 1 Actor not frame sharing
    	public CameraShot(string g, Vector3 m, float dist, float h, float o, float bx, string a)
		{
	    	goal = g;
			sidemarker = m;
			distanceFromTarget = dist;
			height = h;
			orbitAngle = o;
			actor = a;
			biasX = bx;
			oppositeActor = "none";

	   		//DEFAULT VALUE
	   		shotDuration = 1.5f;

			/*//if MainSide is Left change to NEG
			if (LineOfAction.isSceneLeft) {
				biasX = -bx;
			} */
			
			//CALCULATE CAMPOS
			GameObject targetObj = GameObject.Find(actor);

			if (targetObj == null) {
				Debug.LogError ("CANNOT FIND ACTOR in CameraShot Constuctor");
			}

			Vector3 targPos = targetObj.transform.position;
			CamPos = targPos;
			Vector3 forwardN = (targetObj.transform.forward).normalized;
			CamPos = CamPos + (forwardN *  distanceFromTarget);

			//ADJUST HIEGHT -> GO UP OR DOWN
			CamPos = new Vector3(CamPos.x, CamPos.y + height, CamPos.z);

			//CALCULATE CAMROT
			GameObject cam = new GameObject();
			cam.transform.position = CamPos;


			cam.transform.RotateAround (targPos, Vector3.up, orbitAngle);
			Vector3 option1 = cam.transform.position;
			//reset
			cam.transform.position = CamPos;
			cam.transform.RotateAround (targPos, Vector3.up, -orbitAngle);
			Vector3 option2 = cam.transform.position;

			CamPos = GetClosest(sidemarker, option1, option2);
	    
			//Look Directly at Target
			CamRot = Quaternion.LookRotation(targPos - CamPos);

			//Apply Bias Shift
			/*cam.transform.localPosition += Vector3.right * biasX;
			option1 = cam.transform.position;
			//reset
			cam.transform.position = CamPos;
			cam.transform.localPosition += Vector3.right * -biasX;
			option2 = cam.transform.position; 
			CamPos = GetClosest (sidemarker, option1, option2); */

			UnityEngine.Object.Destroy (cam);
	}


		//Focused on only 1 Actor
		public CameraShot(string g, Vector3 m, float dist, float h, float o, float bx, string a, string oppositeA)
		{
			goal = g;
			sidemarker = m;
			distanceFromTarget = dist;
			height = h;
			orbitAngle = o;
			actor = a;
			biasX = bx;
			oppositeActor = oppositeA;

			//DEFAULT VALUE
			shotDuration = 1.5f;

			GameObject targetObj1 = GameObject.Find(actor);
			GameObject targetObj2 = GameObject.Find(oppositeActor); 
			GameObject cam = new GameObject ();

			if (targetObj1 == null || targetObj2 == null) {
				Debug.LogError ("CANNOT FIND ACTORS in CameraShot Constuctor");
			}


			cam.transform.position = targetObj1.transform.position;
			float actorDistance = Vector3.Distance(targetObj1.transform.position, targetObj2.transform.position);
		    //GET DIRECTION OF ACTOR1 TOWARDS ACTOR2 - V1
			Vector3 actorADirN =  (targetObj2.transform.position - targetObj1.transform.position).normalized;
			//MIDPOINT IN actorDirN DIRECTION
			Vector3 MidPoint = targetObj1.transform.position + (actorADirN * (actorDistance / 2));
			//CALCULATE PERP VECTOR 
			//Rotate ActorADirN 90 degrees on y axsis
			Vector3 PDir1 = Quaternion.AngleAxis(90, Vector3.up) * actorADirN;
			Vector3 PDir2 = Quaternion.AngleAxis(-90, Vector3.up) * actorADirN;
	
			Vector3 option1 = MidPoint + (PDir1 * distanceFromTarget);
			Vector3 option2 = MidPoint + (PDir2 * distanceFromTarget);

			CamPos = GetClosest (sidemarker, option1, option2);

			//ADJUST HIEGHT -> GO UP OR DOWN
			CamPos = new Vector3(CamPos.x, CamPos.y + height, CamPos.z);
			cam.transform.position = CamPos;

			//Orbit around Midpoint
			cam.transform.RotateAround (MidPoint, Vector3.up, -orbitAngle);

			//Look At MidPoint
			Vector3 groupDirN = (MidPoint - cam.transform.position).normalized;
			Quaternion rotation = Quaternion.LookRotation(groupDirN);
			cam.transform.rotation = rotation;

			CamPos = cam.transform.position;
			CamRot = cam.transform.rotation;
			UnityEngine.Object.Destroy (cam);
		}			
  }
} //end of namespace