using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomVariables;
using System;

public class FrameShare : CameraShot 
{

		//Focused on 2 Actors
		//FrameSharing
		public override CameraShot ReOrient(string Aname)
		{
			return new FrameShare(this.goal, this.sidemarker, this.distanceFromTarget, this.height, this.orbitAngle,this.biasX, Aname, this.actor);
		} 


		public FrameShare(string g, Vector3 m, float dist, float h, float o, float bx, string a, string oppositeA) : base()
		{
			//set shotID for editor
			Guid gg = Guid.NewGuid();
			editorID = gg.ToString();

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
				return;
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
			UnityEngine.Object.DestroyImmediate (cam);
		}


}
