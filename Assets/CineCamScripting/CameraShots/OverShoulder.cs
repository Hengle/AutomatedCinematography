using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomVariables;
using System;

public class OverShoulder : CameraShot 
{


	public override CameraShot ReOrient(string Aname)
	{
		return new OverShoulder(this.goal, this.sidemarker, this.distanceFromTarget, this.height, this.orbitAngle, this.biasX, Aname, this.actor);
	}


	public OverShoulder(string g, Vector3 m, float dist, float h, float o, float bx, string a, string opactor) : base()
	{
		//set shot ID for editor
         Guid gg = Guid.NewGuid();
		editorID = gg.ToString();

		goal = g;
		sidemarker = m;
		distanceFromTarget = dist;
		height = h;
		orbitAngle = o;
		actor = a;
		biasX = bx;
		oppositeActor = opactor;

	   	//DEFAULT VALUE
	   	shotDuration = 1.5f;

		/*//if MainSide is Left change to NEG
		if (LineOfAction.isSceneLeft) {
			biasX = -bx;
		} */
			
		//CALCULATE CAMPOS
		GameObject targetObj = GameObject.Find(oppositeActor);
		GameObject targetObj2 = GameObject.Find(actor);

		

		if (targetObj == null) {
			Debug.LogError ("CANNOT FIND ACTOR in CameraShot Constuctor");
			return;
		}
		if (targetObj2 == null) {
			Debug.LogError ("CANNOT FIND OPPOSITEACTOR in CameraShot Constuctor");
			return;
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
		CamRot = Quaternion.LookRotation(targetObj2.transform.position - CamPos);

		//Apply Bias Shift
		/*cam.transform.localPosition += Vector3.right * biasX;
		option1 = cam.transform.position;
		//reset
		cam.transform.position = CamPos;
		cam.transform.localPosition += Vector3.right * -biasX;
		option2 = cam.transform.position; 
		CamPos = GetClosest (sidemarker, option1, option2); */

		UnityEngine.Object.DestroyImmediate (cam);
	
	}


}
