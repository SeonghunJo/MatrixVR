using UnityEngine;
using System.Collections;

public class MainGUI : MonoBehaviour {

	public GameObject[] planets;
	private bool dispHelp,hideGui;
	private GameObject displayedPlanet;
	// Use this for initialization
	void Start () {
		dispHelp = false;
		hideGui = false;
		displayedPlanet = Instantiate(planets[0],transform.position,planets[0].transform.rotation) as GameObject;
		displayedPlanet.transform.parent=transform;
	}
	void Update(){
		if(Input.GetKeyUp(KeyCode.F1)){dispHelp = !dispHelp;}
		if(Input.GetKeyUp(KeyCode.F2)){hideGui = !hideGui;}
	}	
	
	void OnGUI(){
		if(!hideGui){
			GUILayout.BeginArea(new Rect(Screen.width-180, 20,150,500));
			GUILayout.BeginVertical();
			GUILayout.Label("Press F2 to hide GUI");
			for(int i = 0; i<planets.Length; i++){
				if(GUILayout.Button(planets[i].name)){
					Destroy(displayedPlanet);
					displayedPlanet = Instantiate(planets[i],transform.position,planets[i].transform.rotation) as GameObject;
					displayedPlanet.transform.parent=transform;
					
				}
			}
			if(GUILayout.Button("Quit")){
				Application.Quit();
			}
			GUILayout.EndVertical();
			
			
			GUILayout.EndArea();
			
			GUILayout.BeginArea(new Rect(30, 20,250,600));
			GUILayout.BeginVertical();
			
			if(dispHelp){
				GUILayout.Label("Press F1 to hide help\n");
				GUILayout.Label("CAMERA FLYBY COMMANDS :");
				GUILayout.Label("Space: Move forward");
				GUILayout.Label("C: Move backward");
				GUILayout.Label("Up arrow: Move upward");
				GUILayout.Label("Down arrow: Move downward");
				GUILayout.Label("Left arrow: Move Left");
				GUILayout.Label("Right arrow: Move Right");
				GUILayout.Label("Q: Turn left");
				GUILayout.Label("D: Turn right");
				GUILayout.Label("Z: Turn up");
				GUILayout.Label("S: Turn down");
				GUILayout.Label("A: Rotate left");
				GUILayout.Label("E: Rotate right");
				GUILayout.Label("Del: Rotate Sun left");
				GUILayout.Label("Page Down: Rotate Sun right");
				GUILayout.Label("Home: Rotate Sun top");
				GUILayout.Label("End: Rotate Sun bottom");
				GUILayout.Label("Mouse scroll wheel: Zoom-In/Zoom-Out");
				GUILayout.Label("Left Click: Rotate Sun");
				GUILayout.Label("Right click: Rotate Earth");
				
				
			}
			else{
				GUILayout.Label("Press F1 to show help");
				
			}
			
			GUILayout.EndVertical();
			
			
			GUILayout.EndArea();
		}
	}
}
