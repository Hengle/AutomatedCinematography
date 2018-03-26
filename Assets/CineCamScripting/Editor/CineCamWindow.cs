using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using CustomVariables;

//Connected to RunTimeManager

public class CineCamWindow : EditorWindow 
{
	/*
	string myString = "Hello World";
	bool groupEnabled;
	bool myBool = true;
	float myFloat = 1.2f;
	string SetCam = "SetCameras";*/

	[MenuItem("Window/CineCam")]
	static void init()
	{ 
		//Get exisiting open window or if none, make a new one
		CineCamWindow window = (CineCamWindow)EditorWindow.GetWindow(typeof(CineCamWindow));
		//myString = EditorGUILayout.TextField ("Text Field", myString);

		window.Show (); 

	}

	void OnGUI()
	{
		try
		{
			GameObject managerRef = GameObject.Find ("AutoCineManager");
			List<Dialogue> dialogList = managerRef.GetComponent<RunTimeManager> ().dialogSequence;


			//List Actors
			GUILayout.BeginArea (new Rect (10, 10, 100, 500));
			GUILayout.Label ("Actor", EditorStyles.boldLabel);
			for(int i = 0; i < dialogList.Count; i++)
			{
				GUILayout.Label (dialogList[i].ActorID);
			}
			GUILayout.EndArea ();

			//List DialogeLines
			GUILayout.BeginArea(new Rect(140, 10, 100, 500));
			GUILayout.Label ("Line", EditorStyles.boldLabel);
			for(int i = 0; i < dialogList.Count; i++)
			{
				GUILayout.Label (dialogList[i].dialogText);
			}
			GUILayout.EndArea ();

			//List Goals as drop down menus
			/*for(int i = 0; i < dialogList.Count; i++)
			{
				GUILayout.BeginHorizontal("box");
				List<Goal> goalList = managerRef.GetComponent<RunTimeManager>().dialogSequence;
				EditorGUILayout.DropdownButton("hello");
				GUILayout.EndArea ();
			} */

			GUILayout.BeginArea(new Rect(250, 10, 100, 100));

			if(GUILayout.Button("PreViz Sequence"))
			{
				managerRef.GetComponent<RunTimeManager>().PlayScene();
			}

			GUILayout.EndArea();


		}
		catch {
			Debug.Log ("reference is null");
		}



		/*
		ManagerReference = GameObject.Find("AutoCineManager");
		GUILayout.Label (Selection.activeGameObject.name);
		myString = EditorGUILayout.TextField ("Text Field", myString);

		//Button
		if (GUILayout.Button (SetCam)) 
		{
			ManagerReference.GetComponent<RunTimeManager> ().ResetShots ();
		}

		groupEnabled = EditorGUILayout.BeginToggleGroup ("Optimal Settings", groupEnabled);
		myBool = EditorGUILayout.Toggle ("Toggle", myBool);
		myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
		EditorGUILayout.EndToggleGroup ();*/

	}

}

