using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using CustomVariables;
using UnityEngine.Audio;

//GET CAMERA FOV FROM DialogCam


public class PreviewWindow : EditorWindow 
{
private PreviewRenderUtility previewRenderer;
int width = 1024;
int height = 768;
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

	[MenuItem("Window/CineCamV2")]
	static void init()
	{ 
		
		PreviewWindow window = (PreviewWindow)EditorWindow.GetWindow(typeof(PreviewWindow));
		window.titleContent.tooltip = "Mesh Preview Editor";
		window.autoRepaintOnSceneChange = true;
		window.Show();

	}

	void OnGUI()
	{
		if(Selection.activeGameObject == null)
		{
			EditorGUILayout.LabelField("No game object selected");
			return; //Do nothing if no game object selected;
			
		}
		var meshFilter = Selection.activeGameObject.GetComponent<MeshFilter>();
		var meshRenderer = Selection.activeGameObject.GetComponentInParent<MeshRenderer>();

		GameObject selectedObject = Selection.activeGameObject;

		if(selectedObject.GetComponent<MeshFilter>() == null || selectedObject.GetComponentInParent<MeshRenderer>() == null)
		{
			EditorGUILayout.LabelField("Game Object does not contain the required components");
			return;
		}

		if(previewRenderer == null)
		{
			Initialize();
		}

        DrawSelectedMesh(selectedObject);
		
		EditorGUILayout.LabelField("Selected: " + Selection.activeGameObject.name);
	
	}

	private void DrawSelectedMesh(GameObject selectedObj)
	{
		
		var boundaries = new Rect(0, 0, width, height);
		previewRenderer.BeginPreview(boundaries, GUIStyle.none);


		previewRenderer.DrawMesh(selectedObj.GetComponent<MeshFilter>().mesh, selectedObj.transform.position, selectedObj.transform.localScale, selectedObj.transform.rotation, selectedObj.GetComponentInParent<MeshRenderer>().material, 0, null ,null, false);

		/*foreach(Transform child in selectedObj.transform)
		{
           previewRenderer.DrawMesh(child.GetComponent<MeshFilter>().mesh, child.transform.position, child.transform.localScale, child.transform.rotation, selectedObj.GetComponentInParent<MeshRenderer>().material, 0, null ,null, false);
		} */
		
		
		
		previewRenderer.camera.Render();
		var render = previewRenderer.EndPreview();
		
		GUI.DrawTexture(new Rect(0, 0, width/3, height/3), render);
	
	}
}
