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
			RunTimeManager runtime_manager = managerRef.GetComponent<RunTimeManager> ();

			List<Dialogue> dialogList = runtime_manager.dialogSequence;


			using (var horizontalScope = new GUILayout.HorizontalScope("box"))
			{
				GUILayout.Label ("Actor", EditorStyles.boldLabel);
				GUILayout.Label ("Line", EditorStyles.boldLabel);
			}

			int GUIshot_counter = 0;
			for(int i = 0; i < dialogList.Count; i++)
			{
				using (var horizontalScope = new GUILayout.HorizontalScope("box"))
					{
				     GUILayout.Label (dialogList[i].ActorID);
					 GUILayout.Label (dialogList[i].dialogText);

					for(int j = 0; j < dialogList[i].goals.Count; j++)
					{
						int index;
						index = EditorGUILayout.Popup(runtime_manager.goalDict[dialogList[i].goals[j]], runtime_manager.options);
						dialogList[i].goals[j] = runtime_manager.options[index];



						if(GUILayout.Button("X"))
						{
							if(dialogList[i].goals.Count > 1)
							{
								dialogList[i].goals.RemoveAt(j);
							}
						}
						GUIshot_counter++;
					}


					if(GUILayout.Button("Add Goal"))
					{
						dialogList[i].goals.Add("Default");
					} 
				}
			}



		
			using (var horizontalScope = new GUILayout.HorizontalScope("box"))
			{
				if(GUILayout.Button("PreViz Sequence"))
				{
					managerRef.GetComponent<RunTimeManager>().PlayScene();
				}
			}





			/*
			//List Actors
			GUILayout.Label ("Actor", EditorStyles.boldLabel);
			for(int i = 0; i < dialogList.Count; i++)
			{
				GUILayout.Label (dialogList[i].ActorID);
			}

			//List DialogeLines
	
			GUILayout.Label ("Line", EditorStyles.boldLabel);
			for(int i = 0; i < dialogList.Count; i++)
			{
				GUILayout.Label (dialogList[i].dialogText);
			} */
		
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

