using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomVariables;
using System;

namespace CustomVariables
{
//public enum Goal {Default, DefaultInclude, Previous, Intensify, Close, FrameShare, Medium, Long, LowAngle, HighAngle, ReverseCheck};

public enum Side {Left, Right};
public enum ClassType {CameraShot, FrameShare, OverShoulder};

	public class CameraShot {

		//for GUI
		public bool isEditing = false;

		public string goal;

		public Vector3 sidemarker; //Can be {left, right}
		public float distanceFromTarget;
		public float height; // 0 is dead on - Can be 5 or -5
		public float orbitAngle;
		public float shotDuration;
		public float biasX;
		public string actor;
		public string oppositeActor; //For Frame Including, Overshoulder
		public bool isOpp;
		public string editorID;

		//Used to the above variables to calculate the following
		//Position in the scene relative to actor
		public Vector3 CamPos;
		public Quaternion CamRot;

		protected CameraShot()
		{

		}

		public virtual CameraShot ReOrient(string Aname)
		{
			return new CameraShot(this.goal, this.sidemarker, this.distanceFromTarget, this.height, this.orbitAngle, this.biasX, Aname);
		} 

		//No Actor calculations required
		//Called when manually setting shot
		public CameraShot(Vector3 pos, Quaternion rot)
		{
			isEditing = true;
			CamPos = pos;
			CamRot = rot;
			isOpp = false;

			//set shot ID for editor
            Guid gg = Guid.NewGuid();
			editorID = gg.ToString();
	
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
			oppositeActor = "none";
			isOpp = false;

	   		//DEFAULT VALUE
	   		shotDuration = 1.5f;
			
			//CALCULATE CAMPOS
			GameObject targetObj = GameObject.Find(actor);

			if (targetObj == null) {
				Debug.LogError ("CANNOT FIND ACTOR in CameraShot Constuctor");
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
			CamRot = Quaternion.LookRotation(targPos - CamPos);
			
			//APPLY BIAS SHIFT
			cam.transform.position = CamPos;

			cam.transform.position += Vector3.forward * biasX;
			option1 = cam.transform.position;

			cam.transform.position = CamPos;
			//Apply Bias shift to marker2
			cam.transform.position += Vector3.forward * -biasX;
			option2 = cam.transform.position; 
			
			CamPos = GetFarthest (sidemarker, option1, option2);  
			
			UnityEngine.Object.DestroyImmediate (cam);
	}


  }
} //end of namespace