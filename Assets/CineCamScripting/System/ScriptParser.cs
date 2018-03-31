using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using CustomVariables;

[System.Serializable]
public class ScriptParser : MonoBehaviour {

	//List of Unique Actors
	public List<string> actors;

	//Takes a textfile of a scipt
	//parses the dialogue and actors and returns a list of Dialogue objects
	public List<Dialogue> parseScript ()
	{
		actors = new List<string> ();

		string path = "Assets/Resources/script.txt";
		StreamReader TextReader = new StreamReader(path);

		List<Dialogue> dialogueSequence = new List<Dialogue> ();

		string line = "";
        //While there are still lines left in file
		while((line = TextReader.ReadLine()) != null)
	    {
			//Delimeter is the : char
			string[] inputArray = line.Split (new char[] { ':' }, System.StringSplitOptions.None);

			//If Script contains Goal description
			List<string> goalList = new List<string> ();
			string goalResult;
			if (inputArray.Length == 3) {
				//Split the Goal input by ' , '
				string[] goalSplitArray = inputArray [2].Split (new char[] { ',' }, System.StringSplitOptions.None);

				for (int i = 0; i < goalSplitArray.Length; i++) {
					goalResult = checkGoal(goalSplitArray [i]);
					goalList.Add (goalResult);
				}
			} else {
				goalList.Add ("Default");
			}
				
			Dialogue dialog = new Dialogue (inputArray [1], inputArray [0], goalList);
			dialogueSequence.Add (dialog);

			//Add actors from script to the actor list
			if(!actors.Contains(inputArray[0])){
				actors.Add (inputArray [0]);
			}
		}
		TextReader.Close ();
		return dialogueSequence;
   	  }
		
	//Check if goal is Defined
	//If not, return Default
	public string checkGoal(string goal)
	{

		string[] possibleGoals = new string[] {
			"Default", "DefaultInclude", "Close",
			"FrameShare", "Long", "HighAngle",
			"Medium", "ReverseCheck", "Previous",
			"Intensify"
		};
		goal = goal.Replace(" ", string.Empty);

		bool goalInList = false;
		for (int i = 0; i < possibleGoals.Length; i++) {
			if (goal == possibleGoals [i]) {
				goalInList = true;
			}
		}

		if (goalInList) {
			return goal;
		} else {
			return "Default";
		}
	}



   }
