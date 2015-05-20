using UnityEngine;
using System.Collections;

public class IniciateFade : MonoBehaviour {

	public static void fade (string Scene, Color col, float damp)
	{
		GameObject init = new GameObject ();
		init.name = "Fader";
		//init.AddComponent<Fader>;
		Fader scr=init.GetComponent<Fader>();
		
	}
}
