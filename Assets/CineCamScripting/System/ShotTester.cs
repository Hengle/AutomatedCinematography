using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//attached to camera
public class ShotTester : MonoBehaviour {

	public GameObject target;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		//float distanceFromTarget = Vector3.Distance(target.transform.position, transform.position);
		//Debug.Log (distanceFromTarget);

		transform.LookAt (target.transform.position);

	}
}
