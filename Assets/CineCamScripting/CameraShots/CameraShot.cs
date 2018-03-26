using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomVariables;

namespace CustomVariables
{
public enum Goal {Default, DefaultInclude, Previous, Intensify, Close, FrameShare, Medium, Long, LowAngle, HighAngle, ReverseCheck};

public enum Side {Left, Right};


	public class CameraShot {
    
		public Goal goal;
		public Side camside; //Can be {left, right}
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
		//Default Shot


		//No Actor calculations required
		//Called when manually setting shot
		public CameraShot(Vector3 pos, Quaternion rot)
		{
			CamPos = pos;
			CamRot = rot;
		}


    	//Focused on only 1 Actor not frame sharing
    	public CameraShot(Goal g, Side s, float dist, float h, float o, float bx, string a)
		{
	    	goal = g;
			camside = s;
			distanceFromTarget = dist;
			height = h;
			orbitAngle = o;
			actor = a;
			biasX = bx;
			oppositeActor = "none";

	   		//DEFAULT VALUE
	   		shotDuration = 1.5f;

			//if MainSide is Left change to NEG
			if (LineOfAction.isSceneLeft) {
				biasX = -bx;
			}

			//DIRECTION TOWARDS CAMSIDE_MARKER
			if (camside == Side.Left) {
				orbitAngle = -o;
		    } 
			
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
			CamPos = cam.transform.position;
	    
			//Look Directly at Target
			CamRot = Quaternion.LookRotation(targPos - CamPos);

			//Apply Bias Shift
			cam.transform.localPosition += Vector3.right * biasX;
			CamPos = cam.transform.position;


			UnityEngine.Object.Destroy (cam);
	}


		//Focused on only 1 Actor
		public CameraShot(Goal g, Side s, float dist, float h, float o, float bx, string a, string oppositeA)
		{
			goal = g;
			camside = s;
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
			//GO HALF-DISTANCE IN actorDirN DIRECTION
			cam.transform.position = targetObj1.transform.position + (actorADirN * (actorDistance / 2));
			//CALCULATE PERP VECTOR 
			//Rotate ActorADirN 90 degrees on y axsis
			Vector3 PDir = Quaternion.AngleAxis(90, Vector3.up) * actorADirN;
			//IF LEFT = -PV.X
			//GO DISTANCEFROMTARGET IN PV DIRECTION
			cam.transform.position = cam.transform.position + (PDir * distanceFromTarget);
			//Inverse PDirN by Rotating 180 on y AXIS
			Vector3 PDirInverse =Quaternion.AngleAxis(-180, Vector3.up) * PDir;
			Quaternion rotation = Quaternion.LookRotation(PDirInverse);
			cam.transform.rotation = rotation;

			CamPos = cam.transform.position;
			CamRot = cam.transform.rotation;
			UnityEngine.Object.Destroy (cam);
		}			
  }
} //end of namespace