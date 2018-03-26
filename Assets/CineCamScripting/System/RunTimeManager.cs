using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomVariables;



[System.Serializable]
//Controls the flow of everything at runtime
public class RunTimeManager : MonoBehaviour {

	ScriptParser parser;
	ShotDatabase database;
	LineOfAction LOA_decider;

	//Looking at Left or Right side
	public  Side CameraSide;

	[SerializeField]
	public List<Dialogue> dialogSequence;


	public Text dialogueText;
	public Text actorIDText;

	GameObject mainCam;


	// Use this for initialization
	void Start () {

		//Reference the camera
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera");

		//Default is Left Side
		CameraSide = Side.Right;

		//Reference of Line of Action Decider
		LOA_decider = GetComponent<LineOfAction>();
		//Reference to database
		database = GetComponent<ShotDatabase> ();
		//Parse the script & populate dialogue list
		parser = GetComponent<ScriptParser> ();
		dialogSequence = parser.parseScript();

		//Assign Shots to Dialogues correlating with goals
		ResetShots ();


		//TESTING
		PlayScene();

	}

	public void PlayScene()
	{
		StopAllCoroutines ();
		StartCoroutine(PlayPreViz());
	}


	//Called when user sets new goals/side in editor and needs
	//to reassign shots & UPDATE
	//SENARIO: User updates goals/side of dialogue and needs shots need to be reassigned for dialogue
	public void ResetShots()
	{
		print ("ResetShots");
		//Looking at left or right
		LOA_decider.associateActorsWithSide (CameraSide);

		List<CameraShot> shots;
		//Assign Shots to dialogue sequence
		for (int i = 0; i < dialogSequence.Count; i++) {

			//if Goal is PREVIOUS and not the first dialogue
			if (dialogSequence [i].goals [0] == Goal.Previous && i > 0) {
				shots = new List<CameraShot> ();
				//Get last shot of previous dialogue
				int lastIndex = dialogSequence [i - 1].shotSequence.Count - 1;
				shots.Add (dialogSequence [i - 1].shotSequence[lastIndex]);
			} 
			//Recalculates shot position with new goal
			else {
				shots = database.getShotList (dialogSequence [i].goals, dialogSequence [i].ActorID);
			}

			dialogSequence [i].SetSequence (shots);
		}  
	}


	//TODO EDITOR_METHOD
	//ADD + REMOVE GOALS
	//FOR EACH GOAL
	//***SET DURATION
	//**SET X Y OFFSET
	//**SET POS & ROT
	//FOR EACH DIALOGUE
	//OBJECT OF IMPORTANCE?
		

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
