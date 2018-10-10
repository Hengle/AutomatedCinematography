using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using CustomVariables;
using UnityEngine.Audio;
using UnityEngine.Animations;

//Connected to RunTimeManager
[ExecuteInEditMode]
public class CineCamWindow : EditorWindow 
{
 Object[] AudioList = new Object[100];
 Object[] AnimationList = new Object[100];

	//EDITOR VARIABLES
	CameraShot E_shot;
	int E_shot_Dindex;
	int E_shot_Gindex;
	RunTimeManager runtime_manager;

	//True -- EditMode Panel
	//False -- SaveNewShot Panel
	static bool CustomizerPanel;

	//PREVIEW UTILITY
	private PreviewRenderUtility previewRenderer;
	int width = 1024;
	int height = 768;

	[MenuItem("Window/CineCam")]
	static void init()
	{ 
		CustomizerPanel = true;

		CineCamWindow window = (CineCamWindow)EditorWindow.GetWindow(typeof(CineCamWindow));
		window.titleContent.tooltip = "Dialog Camera tool";
		window.minSize = new Vector2(1000f, 100f);
		window.autoRepaintOnSceneChange = true;
		window.Show();
	}

     Vector2 scrollPosition = Vector2.zero;


	void OnGUI()
	{
       scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true,  GUILayout.Height(430));
      
		try
		{
			GameObject managerRef = GameObject.Find ("AutoCineManager");
			runtime_manager = managerRef.GetComponent<RunTimeManager> ();
			List<Dialogue> dialogList = runtime_manager.dialogSequence;

            //HEADERS
			GUIStyle labelStyle = new GUIStyle();
			labelStyle.fontSize = 20;
			labelStyle.fontStyle = FontStyle.Bold;
			
			using (var horizontalScope = new GUILayout.HorizontalScope("box"))
			{
				GUILayout.BeginHorizontal("box");
				GUILayout.Label ("Actor", labelStyle, GUILayout.Width(100));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal("box");
				GUILayout.Label ("Dialogue", labelStyle, GUILayout.Width(200));
				GUILayout.EndHorizontal();

				//Shots Label and pulldown menu
				using (var verticalScope = new GUILayout.VerticalScope())
				{
					//SHOT LABEL
					GUILayout.BeginHorizontal("box");
					GUILayout.Label ("Shots", labelStyle, GUILayout.Width(300));
					GUILayout.EndHorizontal();

					//Dropdown SHOT Label
					Side tmp = runtime_manager.CameraSide;
					runtime_manager.CameraSide = (Side)EditorGUILayout.EnumPopup("Camera Side", runtime_manager.CameraSide);
					//If changed reset all shots
					if(tmp != runtime_manager.CameraSide)
					{
						//Clear shots
						runtime_manager.StopAllCoroutines();
						runtime_manager.Awake();
						//Then reset shots
						//runtime_manager.SetUpShots();
						Debug.Log("RESET ALL BOII");
					}   
				}

				GUILayout.BeginHorizontal("box");
				GUILayout.Label ("Audio / Animation", labelStyle, GUILayout.Width(280));
				GUILayout.EndHorizontal();
			} 

			for(int i = 0; i < dialogList.Count; i++)
			{
				using (var horizontalScope = new GUILayout.HorizontalScope())
					{
					//Actor Name
					GUILayout.BeginHorizontal("box");
				     GUILayout.Label (dialogList[i].ActorID, GUILayout.Width(100));
					 GUILayout.EndHorizontal();
					 //Dialogue Text
					 GUILayout.BeginHorizontal("box");
					 GUILayout.Label (dialogList[i].dialogText, GUILayout.Width(200));
					 GUILayout.EndHorizontal();

				    //ADD Button
					if(GUILayout.Button("Add Cut", GUILayout.Width(100), GUILayout.Height(100)))
					{
						runtime_manager.AddShot(i, "Default");
					}

                    using (var verticalScope = new GUILayout.VerticalScope())
					{
					//Goal buttons
					for(int j = 0; j < dialogList[i].goals.Count; j++)
					{
						int index;
						GUILayout.BeginHorizontal("box");
						index = EditorGUILayout.Popup(runtime_manager.goalDict[dialogList[i].goals[j]], runtime_manager.options, GUILayout.Width(80));
						
						//check if it's been updated
						if(dialogList[i].goals[j] != runtime_manager.options[index])
						{
							dialogList[i].goals[j] = runtime_manager.options[index];
							runtime_manager.UpdateShot(i, j, dialogList[i].goals[j]);

						}
						//update goal from popupmenu
						dialogList[i].goals[j] = runtime_manager.options[index]; 
						

						if(GUILayout.Button("Edit", GUILayout.Width(40), GUILayout.Height(15)))
						{
							dialogList[i].shotSequence[j].isEditing = true;
							SetShotEditor( dialogList[i].shotSequence[j], i, j );

							Debug.Log(dialogList[i].goals[j]);
						}
						
						GUILayout.Label ("Opp", GUILayout.Width(30));
						bool changed;
						changed = EditorGUILayout.Toggle(dialogList[i].shotSequence[j].isOpp, GUILayout.Width(20));
						
						//ORIENT TO OPPOSITE ACTOR
						if(changed &&  !dialogList[i].shotSequence[j].isOpp)
						{
							string opposite = runtime_manager.database.findOpposite(dialogList[i].shotSequence[j].actor);    
							dialogList[i].shotSequence[j] = dialogList[i].shotSequence[j].ReOrient(opposite);
						}

						dialogList[i].shotSequence[j].isOpp = changed; 


						if(GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
						{
							if(dialogList[i].goals.Count > 1)
							{
								dialogList[i].goals.RemoveAt(j);
								dialogList[i].shotSequence.RemoveAt(j);
								//CLEAR SHOTEDITOR
								E_shot = null;
								E_shot_Dindex = -1;
								E_shot_Gindex = -1;
							}
						}
						GUILayout.EndHorizontal();
					   }
				      }

				  //AUDIO/ANIMATION FIELDS
				        EditorGUILayout.BeginVertical();

						GUILayout.Label ("Character Audio", EditorStyles.boldLabel);
						AudioList[i] = EditorGUILayout.ObjectField(AudioList[i], typeof(AudioClip), false);
						
						GUILayout.Label ("Character Animation", EditorStyles.boldLabel);
						AnimationList[i] = EditorGUILayout.ObjectField(AnimationList[i], typeof(Animation), false);
        				
						EditorGUILayout.EndVertical();
				}
		}
    GUILayout.EndScrollView();

	
		//SHOT EDITOR PANEL
		GUIStyle labelStyle2 = new GUIStyle();
		labelStyle2.fontSize = 10;
		labelStyle2.fontStyle = FontStyle.Bold;

    	EditorGUILayout.BeginVertical();
		EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);
		
         using (var areaScope = new GUILayout.AreaScope(new Rect(10, 515, 240, 200)))
        {

			GUIStyle labelStyyle = new GUIStyle();
			labelStyyle.fontSize = 15;
			labelStyyle.fontStyle = FontStyle.Bold;

			GUILayout.Label("CUSTOMIZE SHOT", labelStyyle);

			//SAVE (Overwrite mode)
			if(CustomizerPanel)
			{
					GUILayout.Label("Shot: " + E_shot.goal, EditorStyles.boldLabel);
			
					E_shot.orbitAngle = EditorGUILayout.FloatField("OrbitAngle", E_shot.orbitAngle);
					E_shot.height = EditorGUILayout.FloatField("Height", E_shot.height);
					E_shot.biasX = EditorGUILayout.FloatField("X_Bias", E_shot.biasX);
					E_shot.distanceFromTarget = EditorGUILayout.FloatField("Distance", E_shot.distanceFromTarget);
				

				//SAVE -- CREATE NEW BUTTON -- BUTTON LAYOUT
				using (var horizontalScope = new GUILayout.VerticalScope())
				{			
				  if(GUILayout.Button("Overwrite Shot"))
			  	{
				  Vector3 mark = E_shot.sidemarker;
                	 //check if NOT FRAMESHARE

				  	Debug.Log(E_shot.goal);
				 	if(E_shot.oppositeActor == "none")
				 	{
						E_shot = new CameraShot(E_shot.goal, E_shot.sidemarker, E_shot.distanceFromTarget, E_shot.height, E_shot.orbitAngle, E_shot.biasX, E_shot.actor);
				 	}
				 	else if(E_shot.goal == "OverShoulder")
				 	{
						 Debug.Log("Got here OS");
					 	E_shot = new OverShoulder(E_shot.goal, E_shot.sidemarker, E_shot.distanceFromTarget, E_shot.height, E_shot.orbitAngle, E_shot.biasX, E_shot.actor, E_shot.oppositeActor);
				 	} 
				 	else
				 	{
						Debug.Log("Got here FS");
						E_shot = new FrameShare(E_shot.goal, E_shot.sidemarker, E_shot.distanceFromTarget, E_shot.height, E_shot.orbitAngle, E_shot.biasX, E_shot.actor, E_shot.oppositeActor);
				 	}	 
					//Send Shot specs over
					SaveShot(E_shot);
			  	}
			
			  	if(GUILayout.Button("CREATE NEW SHOT"))
			  	{
					  CustomizerPanel = false;
                 	//Create new Shot
			  	}
				}
			}

			//Create New Shot Panel
			else
			{
				using (var horizontalScope = new GUILayout.HorizontalScope())
				{
					if(GUILayout.Button("CREATE"))
					{

						//DropDown Menu to base shot off of
						//Goal Name
						//Other Properiies
					}

					if(GUILayout.Button("Back"))
					{
						CustomizerPanel = true;
					}
				}
			}
        }


		EditorGUILayout.EndVertical();

		 	  //END OF GUI BUTTONS PREVIZ AND EXPORT
		     
			GUILayout.BeginVertical("box");
			if(GUILayout.Button("PreViz Sequence"))
			{
				managerRef.GetComponent<RunTimeManager>().PlayScene();
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical("box");
			if(GUILayout.Button("Export to Timeline"))
			{
				managerRef.GetComponent<RunTimeManager>().ExportShotsToTimeline();
			}
			GUILayout.EndVertical(); 

		} //end of try
		catch {
			Debug.Log ("reference is null");
		}
		//END OF PREVIZ BUTTONS

  /*=============PREVIEW RENDER UTILITY SHOT ============= */

		if(previewRenderer == null){
			Initialize();
		}

		//Draw every actor in the scene
		var boundaries = new Rect(0, 0, width, height);
		previewRenderer.BeginPreview(boundaries, GUIStyle.none);

		for(int i = 0; i < runtime_manager.parser.actors.Count; i++)
		{
		  GameObject gameobj = GameObject.Find(runtime_manager.parser.actors[i]);


		
		  Debug.Log(i + " actor");
		  DrawSelectedMesh(gameobj);
		}

		//Collected every actor, draw on scene
		previewRenderer.camera.Render();
		var render = previewRenderer.EndPreview();
		GUI.DrawTexture(new Rect(515, 550, width/3, height/3), render);

  /* ======================================================*/
	} //OnGui END
public void Initialize()
{

    //In game Camera Render Resolutions
	width = 1024;
	height  = 768;

    //Camera Settins
	previewRenderer = new PreviewRenderUtility();
	previewRenderer.camera.backgroundColor = Color.gray;
    previewRenderer.cameraFieldOfView = 60;
	previewRenderer.camera.farClipPlane = 1000;
	previewRenderer.camera.nearClipPlane = 0.3f;
	previewRenderer.camera.depth = -1;
	previewRenderer.camera.transform.position = GameObject.Find("Main Camera").transform.position;
	previewRenderer.camera.transform.rotation = GameObject.Find("Main Camera").transform.rotation; 
}
public static Quaternion QuaternionFromMatrix(Matrix4x4 m) 
{ 
	return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1)); 
}

public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix) {
    Vector3 scale;
    scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
    scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
    scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
    return scale;
}

public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix) {
    Vector3 translate;
    translate.x = matrix.m03;
    translate.y = matrix.m13;
    translate.z = matrix.m23;
    return translate;
}

    //Draw all actors at all time
	private void DrawSelectedMesh(GameObject selectedObj)
	{
		previewRenderer.DrawMesh(selectedObj.GetComponent<MeshFilter>().mesh, selectedObj.transform.position, selectedObj.transform.localScale, selectedObj.transform.rotation, selectedObj.GetComponentInParent<MeshRenderer>().material, 0, null ,null, false);

		foreach(Transform child in selectedObj.transform)
		{
			Matrix4x4 m = child.localToWorldMatrix;
			Quaternion rot = QuaternionFromMatrix(m);
			Vector3 pos = ExtractTranslationFromMatrix(ref m);
			Vector3 sca = ExtractScaleFromMatrix(ref m);
           	previewRenderer.DrawMesh(child.GetComponent<MeshFilter>().mesh, pos, sca, rot, selectedObj.GetComponent<MeshRenderer>().material, 0, null ,null, false);
		} 	
	}

	void SetShotEditor(CameraShot shot, int D, int G)
	{
		E_shot = shot;
		E_shot_Dindex = D;
		E_shot_Gindex = G;
	}

	void SaveShot(CameraShot ss)
	{
		runtime_manager.dialogSequence[E_shot_Dindex].shotSequence[E_shot_Gindex] = ss;

	}

	void AddNewShotToDatabase(CameraShot ss)
	{

	}
}

