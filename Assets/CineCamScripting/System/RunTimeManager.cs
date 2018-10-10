using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Animations;
using UnityEngine.UI;
using CustomVariables;



[System.Serializable]
[ExecuteInEditMode]
//Controls the flow of everything at runtime
public class RunTimeManager : MonoBehaviour {


	public ScriptParser parser;
	public ShotDatabase database;
	LineOfAction LOA_decider;

	//Looking at Left or Right side
	public  Side CameraSide;

	[SerializeField]
	public List<Dialogue> dialogSequence;

	public Text dialogueText;
	public Text actorIDText;

	GameObject mainCam;

	public Dictionary<int, string> ShotDropDown;
	//used in dropdown menu
	public string[] options;

	//FOR GOAL GUI
	public Dictionary<string, int> goalDict;


	// Use this for initialization
	public void Awake() {
	    
	    Debug.Log("EDITOR MODE AWAKE");
		options = new string[] {"Default", "Default/Mid", "Default/Close", "Default/XtremeClose", "HighAngle", "LowAngle", "OverShoulder", "Parallel", "FrameShare"};

		mainCam = GameObject.FindGameObjectWithTag ("MainCamera");

		//CameraSide = Side.Left;

		LOA_decider = GetComponent<LineOfAction>();
		database = GetComponent<ShotDatabase> ();
		parser = GetComponent<ScriptParser> ();
		dialogSequence = parser.parseScript();

		//make dictionary for GUI
		//associate string with index for popup
		//TODO CALL WHEN USER ADDS SHOT IN CUSTOM
		goalDict = new Dictionary<string, int>();
		for (int i = 0; i < options.Length; i++)
		{
			goalDict.Add (options [i], i); 
		} 

		//Assign Shots to Dialogues correlating with goals
		database.SetUpDatabase();
		SetUpShots();
	}

	public void PlayScene()
	{
		StopAllCoroutines ();
		StartCoroutine(PlayPreViz());
	}

	//Called on Initial
	public void SetUpShots()
	{
		print ("ResetShots");

		LOA_decider.SetSide (CameraSide);

		for (int i = 0; i < dialogSequence.Count; i++) 
		{
			for(int j = 0; j < dialogSequence[i].goals.Count; j++)
			{
				CameraShot shot = database.ShotGet(dialogSequence[i].goals[j], dialogSequence[i].ActorID);
				dialogSequence[i].shotSequence.Add(shot);
			}
		}
	}

	//Change goal of a Shot
	public void UpdateShot(int D_index, int G_index, string goal)
	{
		dialogSequence[D_index].shotSequence[G_index] = database.ShotGet(goal, dialogSequence[D_index].ActorID);
		dialogSequence[D_index].goals[G_index] = goal;
	}

	public void AddShot(int D_index, string goal)
	{
		dialogSequence[D_index].shotSequence.Add(database.ShotGet(goal, dialogSequence[D_index].ActorID));
		dialogSequence[D_index].goals.Add(goal);
	}	
		
    //Plays the timeline sequence
	IEnumerator PlayPreViz()
	{
		int CurrentDialogueIndex = 0;

		Debug.Log(dialogSequence.Count);
		while(CurrentDialogueIndex < dialogSequence.Count) 
		{
			//UPDATE BOTTOM UI

			//This breaks everything
			//actorIDText.text = dialogSequence [CurrentDialogueIndex].ActorID;
			//dialogueText.text = dialogSequence [CurrentDialogueIndex].dialogText;

			//UPDATE CAMERA POSITION
			for(int shotIndex = 0; shotIndex < dialogSequence[CurrentDialogueIndex].shotSequence.Count; shotIndex++)
			{
				mainCam.transform.position = dialogSequence[CurrentDialogueIndex].shotSequence[shotIndex].CamPos;
				mainCam.transform.rotation = dialogSequence [CurrentDialogueIndex].shotSequence[shotIndex].CamRot;
				yield return new WaitForSeconds (dialogSequence [CurrentDialogueIndex].shotSequence [shotIndex].shotDuration);
			}
			CurrentDialogueIndex++;
		}
	}

	public void ExportShotsToTimeline()
	{	

	}
}
