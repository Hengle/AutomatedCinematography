using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomVariables;



[System.Serializable]
//Controls the flow of everything at runtime
public class RunTimeManager : MonoBehaviour {

	public ScriptParser parser;
	ShotDatabase database;
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
	public string[] options = new string[] {
		"FrameShare", "Default", "HighAngle", "LowAngle", "Previous"
	};

	//FOR GOAL GUI
	public Dictionary<string, int> goalDict;


	// Use this for initialization
	void Start () {


		mainCam = GameObject.FindGameObjectWithTag ("MainCamera");

		CameraSide = Side.Left;

		LOA_decider = GetComponent<LineOfAction>();
		database = GetComponent<ShotDatabase> ();
		parser = GetComponent<ScriptParser> ();
		dialogSequence = parser.parseScript();



		//Assign Shots to Dialogues correlating with goals
		ResetShots ();

	}

	public void PlayScene()
	{
		ResetShots ();
		StopAllCoroutines ();
		StartCoroutine(PlayPreViz());
	}


	//Called when user sets new goals/side in editor and needs
	//to reassign shots & UPDATE
	//SENARIO: User updates goals/side of dialogue and needs shots need to be reassigned for dialogue
	public void ResetShots()
	{
		//FOR GUI
		//total number of shots in scene
		int NumShotsScene = 0;

		print ("ResetShots");
		LOA_decider.SetSide (CameraSide);

		List<CameraShot> shots;
		for (int i = 0; i < dialogSequence.Count; i++) {

			//GOAL = PREVIOUS 
			if (dialogSequence [i].goals [0] == "Previous" && i > 0) {
				shots = new List<CameraShot> ();
				int lastIndex = dialogSequence [i - 1].shotSequence.Count - 1;
				shots.Add (dialogSequence [i - 1].shotSequence[lastIndex]);
			} 
				
			else {
				//Gets list of finalized shot for each goal
				shots = database.getShotList (dialogSequence [i].goals, dialogSequence [i].ActorID);
			}
			dialogSequence [i].SetSequence (shots);
			NumShotsScene += dialogSequence [i].shotSequence.Count;
		}
			
		//make dictionary for GUI
		//associate string with index for popup
		//TODO CALL WHEN USER ADDS SHOT
		goalDict = new Dictionary<string, int>();
		for (int i = 0; i < options.Length; i++)
		{
			goalDict.Add (options [i], i); 
		} 
	}
		
		
	IEnumerator PlayPreViz()
	{
		int CurrentDialogueIndex = 0;

		while(CurrentDialogueIndex < dialogSequence.Count) 
		{
			//UPDATE BOTTOM UI
			actorIDText.text = dialogSequence [CurrentDialogueIndex].ActorID;
			dialogueText.text = dialogSequence [CurrentDialogueIndex].dialogText;


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
}
